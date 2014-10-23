namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     Implements the base functionality of a service which maintains the state for a hierarchical progress operation.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public abstract class HierarchicalProgressBase<TMessage> : IDisposable
        where TMessage : class
    {
        #region Fields

        /// <summary>
        ///     The factory used to create <see cref="IProgressReport{TMessage}"/> instances.
        /// </summary>
        protected readonly IProgressReportFactory<TMessage> ProgressReportFactory;

        /// <summary>
        ///     Keeps a list of the operations in progress and not completed.
        /// </summary>
        private readonly List<ProgressInfo<TMessage>> activeChildProgressInfos = new List<ProgressInfo<TMessage>>();

        /// <summary>
        ///     Keeps a list of the operations in progress (both completed and not completed).
        /// </summary>
        private readonly List<ProgressInfo<TMessage>> childProgressInfos = new List<ProgressInfo<TMessage>>();

        /// <summary>
        ///     <see langword="true"/> if the <see cref="activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </summary>
        private readonly bool keepProgressListOrdered;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HierarchicalProgressBase{TMessage}"/> class.
        /// </summary>
        /// <param name="keepProgressListOrdered">
        ///     <see langword="true"/> if the <see cref="activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </param>
        /// <param name="progressReportFactory">
        ///     The factory used to create <see cref="IProgressReport{TMessage}"/> instances.
        /// </param>
        protected HierarchicalProgressBase(
            bool keepProgressListOrdered,
            IProgressReportFactory<TMessage> progressReportFactory)
        {
            this.keepProgressListOrdered = keepProgressListOrdered;
            this.ProgressReportFactory = progressReportFactory;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a list of the operations in progress and not completed.
        /// </summary>
        public List<ProgressInfo<TMessage>> ActiveChildProgressInfos
        {
            get
            {
                return this.activeChildProgressInfos;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates and registers a child long running operation in order to provide feedback to the user.
        /// </summary>
        /// <param name="blocksUi">
        ///     <see langword="true"/> if the UI should be blocked while this child operation is active.
        /// </param>
        /// <returns>
        ///     A <see cref="IProgress{IProgressReport}"/> object which allows manipulation of the progress of the operation.
        /// </returns>
        public virtual IHierarchicalProgress<IProgressReport<TMessage>> CreateProgress(bool blocksUi)
        {
            var childProgress = new HierarchicalProgress<TMessage>(this.OnReport, this.UnregisterProgress, this.ProgressReportFactory);
            var progressInfo = new ProgressInfo<TMessage>(childProgress, blocksUi);
            this.childProgressInfos.Add(progressInfo);
            this.activeChildProgressInfos.Add(progressInfo);
            return childProgress;
        }

        /// <summary>
        ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Recomputes the aggregate status from the child operations currently in progress.
        /// </summary>
        /// <returns>
        ///     The aggregate <see cref="IProgressReport{TMessage}"/> instance representing the state of this progress instance.
        /// </returns>
        protected IProgressReport<TMessage> CalculateAggregateProgressReport()
        {
            IProgressReport<TMessage> aggregateProgressReport = null;

            if (this.activeChildProgressInfos.Any())
            {
                if (this.childProgressInfos.All(x => x.LastReportedStatus != null))
                {
                    var allActiveHaveProgressValue = this.activeChildProgressInfos.All(x => x.LastReportedStatus.ProgressValue.HasValue);
                    var aggregateState = this.childProgressInfos.Max(x => x.LastReportedStatus.State);
                    var message = this.activeChildProgressInfos.Last().LastReportedStatus.Message;

// ReSharper disable once PossibleInvalidOperationException
                    if (allActiveHaveProgressValue)
                    {
                        var denominator = this.childProgressInfos.Sum(x => x.LastReportedStatus.ProgressMaximumValue);
                        var progressValue =
                            (from childProgressInfo in this.childProgressInfos
                             let status = childProgressInfo.LastReportedStatus
                             let maximumValue = status.ProgressMaximumValue
                             let isActive = this.activeChildProgressInfos.Contains(childProgressInfo)
                             let numerator = (isActive
                                                  ? status.ProgressValue.Value
                                                  : maximumValue)
                             select numerator / denominator)
                                .Sum();

                        aggregateProgressReport = this.ProgressReportFactory.Create(message, progressValue, 1.0, aggregateState);
                    }
                    else
                    {
                        aggregateProgressReport = this.ProgressReportFactory.Create(message, aggregateState);
                    }
                }
            }

            return aggregateProgressReport;
        }

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"> <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of managed resources
                this.childProgressInfos.Clear();
                this.activeChildProgressInfos.Clear();
            }

            // Dispose of native resources
        }

        /// <summary>
        ///     Handles progress reports.
        /// </summary>
        /// <param name="sender">
        ///     The sender progress object.
        /// </param>
        /// <param name="value">
        ///     The new value.
        /// </param>
        protected void OnReport(
            IProgress<IProgressReport<TMessage>> sender,
            IProgressReport<TMessage> value)
        {
            if (sender == null)
            {
                throw new ArgumentNullException("sender");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            ProgressInfo<TMessage> childProgressInfo;

            // The OnReport method is called from the message queue. It is possible that the progress operation has already been unregistered.
            if (this.TryGetOperationInfo(sender, out childProgressInfo))
            {
                // Put sender at the top of the pile
                if (this.keepProgressListOrdered)
                {
                    if (this.activeChildProgressInfos.Remove(childProgressInfo))
                    {
                        this.activeChildProgressInfos.Add(childProgressInfo);
                    }
                }

                childProgressInfo.LastReportedStatus = value;

                this.OnStatusChanged();
            }
        }

        /// <summary>
        ///     Called whenever the status of the progress operation needs to be recomputed.
        /// </summary>
        protected abstract void OnStatusChanged();

        /// <summary>
        ///     Tries to retrieve the <see cref="ProgressInfo{TMessage}"/> instance that represents <paramref name="childProgress"/>.
        /// </summary>
        /// <param name="childProgress">
        ///     The progress instance to retrieve information for.
        /// </param>
        /// <param name="info">
        ///     The variable which will receive the <see cref="ProgressInfo{TMessage}"/> in case the <paramref name="childProgress"/> instance is found.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> in case the <paramref name="childProgress"/> is known.
        /// </returns>
        protected bool TryGetOperationInfo(
            IProgress<IProgressReport<TMessage>> childProgress,
            out ProgressInfo<TMessage> info)
        {
            info = this.childProgressInfos.FirstOrDefault(o => object.ReferenceEquals(childProgress, o.Progress));

            return info != null;
        }

        /// <summary>
        ///     Unregisters a long running operation previously registered through <see cref="CreateProgress"/>.
        /// </summary>
        /// <param name="childProgress">
        ///     The <see cref="IProgress{T}"/> object previously registered through <see cref="CreateProgress"/>.
        /// </param>
        private void UnregisterProgress(IProgress<IProgressReport<TMessage>> childProgress)
        {
            ProgressInfo<TMessage> childProgressInfo;
            if (this.TryGetOperationInfo(childProgress, out childProgressInfo))
            {
                this.activeChildProgressInfos.Remove(childProgressInfo);

                if (!this.activeChildProgressInfos.Any())
                {
                    // If this was the last active progress, then we can clear all the operations we've been tracking.
                    this.childProgressInfos.Clear();
                }

                this.OnStatusChanged();
            }
        }

        #endregion
    }
}