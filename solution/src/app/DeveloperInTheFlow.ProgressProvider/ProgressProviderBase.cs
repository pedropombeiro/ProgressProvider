namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.Linq;
    using System.Threading;

    /// <summary>
    ///     The base implementation for a <see cref="IProgressProvider{IProgressReport}"/>.
    /// </summary>
    [Obsolete("Use generic version of IProgressReport")]
    public abstract class ProgressProviderBase : ProgressProviderBase<string>,
                                                 IProgressProvider<IProgressReport>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressProviderBase"/> class.
        /// </summary>
        /// <param name="keepProgressListOrdered">
        ///     <see langword="true"/> if the <see cref="HierarchicalProgressBase{TMessage}.activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </param>
        protected ProgressProviderBase(bool keepProgressListOrdered)
            : base(keepProgressListOrdered, new StringProgressReportFactory())
        {
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        ///     Gets the active child progress operations.
        /// </summary>
        IHierarchicalProgress<IProgressReport>[] IProgressProvider<IProgressReport>.ActiveChildProgressOperations
        {
            get
            {
// ReSharper disable once CoVariantArrayConversion
                return this.ActiveChildProgressOperations.Select(op => new HierarchicalProgressWrap(op)).ToArray();
            }
        }

        /// <summary>
        /// Gets the active status, if any.
        /// </summary>
        IProgressReport IProgressProvider<IProgressReport>.Status
        {
            get
            {
                return (IProgressReport)this.Status;
            }
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Creates and registers a long running operation in order to provide feedback to the user.
        /// </summary>
        /// <param name="blocksUi">
        ///     <see langword="true"/> if the UI should be blocked while this operation is active.
        /// </param>
        /// <returns>
        ///     A <see cref="IProgress{T}"/> object which allows manipulation of the progress of the operation.
        /// </returns>
        IHierarchicalProgress<IProgressReport> IProgressProvider<IProgressReport>.CreateProgress(bool blocksUi)
        {
            return new HierarchicalProgressWrap(this.CreateProgress(blocksUi));
        }

        #endregion

        /// <summary>
        ///   Wraps a IHierarchicalProgress{IProgressReport{TMessage}} to implement the non generic nested interface.
        /// </summary>
        private class HierarchicalProgressWrap : IHierarchicalProgress<IProgressReport>
        {
            #region Fields

            /// <summary>
            ///     The source which is wrapped.
            /// </summary>
            private readonly IHierarchicalProgress<IProgressReport<string>> source;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="HierarchicalProgressWrap"/> class.
            /// </summary>
            /// <param name="source">
            ///     The source to wrap.
            /// </param>
            public HierarchicalProgressWrap(IHierarchicalProgress<IProgressReport<string>> source)
            {
                this.source = source;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the <see cref="System.Threading.CancellationTokenSource"/> for this progress operation. 
            ///     Clients can pass this value to the related operation in progress in order to monitor when the user has cancelled the operation.
            /// </summary>
            public CancellationTokenSource CancellationTokenSource
            {
                get
                {
                    return this.source.CancellationTokenSource;
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
            ///     A <see cref="IProgress{T}"/> object which allows manipulation of the progress of the operation.
            /// </returns>
            public IHierarchicalProgress<IProgressReport> CreateProgress(bool blocksUi)
            {
                return new HierarchicalProgressWrap(this.source.CreateProgress(blocksUi));
            }

            /// <summary>
            ///   Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                this.source.Dispose();
            }

            /// <summary>
            /// Reports a progress update.
            /// </summary>
            /// <param name="value">The value of the updated progress.</param>
            public void Report(IProgressReport value)
            {
                this.source.Report(value);
            }

            /// <summary>
            ///     Reports the progress operation as completed (making it count as 100% progress value).
            /// </summary>
            public void ReportCompleted()
            {
                this.source.ReportCompleted();
            }

            #endregion
        }

        /// <summary>
        ///     The factory which create <see cref="ProgressReport"/> instances.
        /// </summary>
        private class StringProgressReportFactory : IProgressReportFactory<string>
        {
            #region Public Methods and Operators

            /// <summary>
            ///     Creates a new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </summary>
            /// <param name="message">
            ///     The message describing the status of the long-running operation.
            /// </param>
            /// <param name="progressValue">
            ///     The current progress from [0, <paramref name="progressMaximumValue"/>].
            /// </param>
            /// <param name="progressMaximumValue">
            ///     The maximum progress value.
            /// </param>
            /// <param name="state">
            ///     The state of the long-running operation.
            /// </param>
            /// <returns>
            ///     A new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </returns>
            public IProgressReport<string> Create(
                string message,
                double progressValue,
                double progressMaximumValue,
                ProgressState state)
            {
                return new ProgressReport(message, progressValue, progressMaximumValue, state);
            }

            /// <summary>
            ///     Creates a new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </summary>
            /// <param name="message">
            ///     The message describing the status of the long-running operation.
            /// </param>
            /// <param name="state">
            ///     The state of the long-running operation.
            /// </param>
            /// <returns>
            ///     A new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </returns>
            public IProgressReport<string> Create(
                string message,
                ProgressState state)
            {
                return new ProgressReport(message, state);
            }

            /// <summary>
            ///     Creates a new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </summary>
            /// <returns>
            ///     A new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </returns>
            public IProgressReport<string> Create()
            {
                return new ProgressReport();
            }

            #endregion
        }
    }
}