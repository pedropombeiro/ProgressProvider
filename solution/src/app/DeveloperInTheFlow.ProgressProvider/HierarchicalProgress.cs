namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.Linq;
    using System.Threading;

    /// <summary>
    ///     Provides a hierarchical IProgress{T} that invokes callbacks for each reported progress value and a final callback for when it is disposed.
    /// </summary>
    public class HierarchicalProgress : HierarchicalProgressBase, 
                                        IHierarchicalProgress<IProgressReport>
    {
        #region Fields

        /// <summary>
        ///     The <see cref="CancellationTokenSource"/> for this progress operation. 
        ///     Clients can pass this value to the related operation in progress in order to monitor when the user has cancelled the operation.
        /// </summary>
        private readonly Lazy<CancellationTokenSource> cancellationTokenSource = new Lazy<CancellationTokenSource>(true);

        /// <summary>
        ///     The handler which will get called when the object is marked as completed.
        /// </summary>
        private readonly Action<IProgress<IProgressReport>> reportCompletedHandler;

        /// <summary>
        ///     The underlying progress object which will handle progress reporting.
        /// </summary>
        private readonly IProgress<IProgressReport> progress;

        /// <summary>
        ///     The last progress reported through <see cref="IProgress{T}.Report"/>.
        /// </summary>
        private IProgressReport lastProgressReport;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HierarchicalProgress"/> class.
        /// </summary>
        /// <param name="reportHandler">
        ///     The handler which will get called for each reported value.
        /// </param>
        /// <param name="reportCompletedHandler">
        ///     The handler which will get called when the object is marked as completed.
        /// </param>
        public HierarchicalProgress(
            Action<IProgress<IProgressReport>, IProgressReport> reportHandler, 
            Action<IProgress<IProgressReport>> reportCompletedHandler)
            : base(true)
        {
            this.progress = new Progress<IProgressReport>(value => reportHandler(this, value));
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
        void IProgress<IProgressReport>.Report(IProgressReport value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // If the status changed to ProgressState.Error and it is missing a progress value, then we take the progress value from the current state.
            if (value.State == ProgressState.Error && !value.ProgressValue.HasValue &&
                this.lastProgressReport != null && this.lastProgressReport.ProgressValue.HasValue)
            {
                var message = string.IsNullOrEmpty(value.Message)
                                  ? this.lastProgressReport.Message
                                  : value.Message;

                value = new ProgressReport(message, this.lastProgressReport.ProgressValue.Value, this.lastProgressReport.ProgressMaximumValue, value.State);
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

        #endregion
    }
}