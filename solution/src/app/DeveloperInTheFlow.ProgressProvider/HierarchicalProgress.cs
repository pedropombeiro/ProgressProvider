namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.Linq;
    using System.Threading;

    /// <summary>
    ///     Provides a hierarchical IProgress{T} that invokes callbacks for each reported progress value and a final callback for when it is disposed.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class HierarchicalProgress<TMessage> : HierarchicalProgressBase<TMessage>,
                                                  IHierarchicalProgress<IProgressReport<TMessage>>
        where TMessage : class
    {
        #region Fields

        /// <summary>
        ///     The <see cref="CancellationTokenSource"/> for this progress operation. 
        ///     Clients can pass this value to the related operation in progress in order to monitor when the user has cancelled the operation.
        /// </summary>
        private readonly Lazy<CancellationTokenSource> cancellationTokenSource = new Lazy<CancellationTokenSource>(true);

        /// <summary>
        ///     The underlying progress object which will handle progress reporting.
        /// </summary>
        private readonly IProgress<IProgressReport<TMessage>> progress;

        /// <summary>
        ///     The handler which will get called when the object is marked as completed.
        /// </summary>
        private readonly Action<IProgress<IProgressReport<TMessage>>> reportCompletedHandler;

        /// <summary>
        ///     The last progress reported through <see cref="IProgress{T}.Report"/>.
        /// </summary>
        private IProgressReport<TMessage> lastProgressReport;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HierarchicalProgress{TMessage}"/> class.
        /// </summary>
        /// <param name="reportHandler">
        ///     The handler which will get called for each reported value.
        /// </param>
        /// <param name="reportCompletedHandler">
        ///     The handler which will get called when the object is marked as completed.
        /// </param>
        /// <param name="progressReportFactory">
        ///     The factory used to create <see cref="IProgressReport{T}"/> instances.
        /// </param>
        public HierarchicalProgress(
            Action<IProgress<IProgressReport<TMessage>>, IProgressReport<TMessage>> reportHandler,
            Action<IProgress<IProgressReport<TMessage>>> reportCompletedHandler,
            IProgressReportFactory<TMessage> progressReportFactory)
            : base(true, progressReportFactory)
        {
            this.progress = new Progress<IProgressReport<TMessage>>(value => reportHandler(this, value));
            this.reportCompletedHandler = reportCompletedHandler;
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
                return this.cancellationTokenSource.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Reports the progress operation as completed (making it count as 100% progress value).
        /// </summary>
        public void ReportCompleted()
        {
            this.reportCompletedHandler(this);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        ///     Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<IProgressReport<TMessage>>.Report(IProgressReport<TMessage> value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // If the status changed to ProgressState.Error and it is missing a progress value, then we take the progress value from the current state.
            if (value.State == ProgressState.Error && !value.ProgressValue.HasValue &&
                this.lastProgressReport != null && this.lastProgressReport.ProgressValue.HasValue)
            {
                var message = this.IsNullOrEmpty(value.Message)
                                  ? this.lastProgressReport.Message
                                  : value.Message;

                value = this.ProgressReportFactory.Create(message, this.lastProgressReport.ProgressValue.Value, this.lastProgressReport.ProgressMaximumValue, value.State);
            }

            if (value.ProgressValue.HasValue && this.ActiveChildProgressInfos.Any())
            {
                throw new NotSupportedException();
            }

            this.lastProgressReport = value;
            this.progress.Report(value);
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"> <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReportCompleted();

                // Dispose of managed resources
            }

            // Dispose of native resources
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Recomputes the aggregate status from the child operations currently in progress.
        /// </summary>
        protected override void OnStatusChanged()
        {
            var status = this.CalculateAggregateProgressReport();

            if (status != null)
            {
                // Propagate the new aggregate to the parent progress
                this.progress.Report(status);
            }
        }

        /// <summary>
        ///     Verifies if the <paramref name="message"/> is null or in the case it is a string if it's empty.
        /// </summary>
        /// <param name="message">
        ///     The message to verify if it's null or empty.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="message"/> is null, or in the case it is a string is empty, otherwise <c>false</c>.
        /// </returns>
        private bool IsNullOrEmpty(TMessage message)
        {
            var stringMessage = message as string;

            if (stringMessage != null && string.IsNullOrEmpty(stringMessage))
            {
                return true;
            }

            return message == default(TMessage);
        }

        #endregion
    }
}