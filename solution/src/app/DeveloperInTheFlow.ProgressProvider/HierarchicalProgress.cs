namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.Linq;

    /// <summary>
    ///     Provides a hierarchical IProgress{T} that invokes callbacks for each reported progress value and a final callback for when it is disposed.
    /// </summary>
    /// <remarks>
    /// TODO : [lmbpop0 02.12.13 15:16] Test this.
    /// </remarks>
    public class HierarchicalProgress : HierarchicalProgressBase, 
                                        IHierarchicalProgress<IProgressReport>
    {
        #region Fields

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
        public HierarchicalProgress(Action<IProgress<IProgressReport>, IProgressReport> reportHandler, 
                                    Action<IProgress<IProgressReport>> disposeHandler)
            : base(true)
        {
            this.progress = new Progress<IProgressReport>(value => reportHandler(this, value));
            this.disposeHandler = disposeHandler;
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

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<IProgressReport>.Report(IProgressReport value)
        {
            if (value.ProgressValue.HasValue && this.ActiveChildProgressInfos.Any())
            {
                throw new NotSupportedException();
            }

            this.progress.Report(value);
        }

        #endregion
    }
}