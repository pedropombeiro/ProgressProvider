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
        ///     The handler which will get called when the object is disposed.
        /// </summary>
        private readonly Action<IProgress<IProgressReport>> disposeHandler;

        /// <summary>
        ///     The underlying progress object which will handle progress reporting.
        /// </summary>
        private readonly IProgress<IProgressReport> progress;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HierarchicalProgress"/> class.
        /// </summary>
        /// <param name="reportHandler">
        ///     The handler which will get called for each reported value.
        /// </param>
        /// <param name="disposeHandler">
        ///     The handler which will get called when the object is disposed.
        /// </param>
        public HierarchicalProgress(
            Action<IProgress<IProgressReport>, IProgressReport> reportHandler, 
            Action<IProgress<IProgressReport>> disposeHandler)
            : base(true)
        {
            this.progress = new Progress<IProgressReport>(value => reportHandler(this, value));
            this.disposeHandler = disposeHandler;
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

        #region Explicit Interface Methods

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<IProgressReport>.Report(IProgressReport value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (value.ProgressValue.HasValue && this.ActiveChildProgressInfos.Any())
            {
                throw new NotSupportedException();
            }

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
                // Dispose of managed resources
                this.disposeHandler(this);
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