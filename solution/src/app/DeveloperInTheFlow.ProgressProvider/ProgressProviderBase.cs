namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    ///     The base implementation for a <see cref="IProgressProvider{IProgressReport}"/>.
    /// </summary>
    public abstract class ProgressProviderBase : HierarchicalProgressBase, 
                                                 IProgressProvider<IProgressReport>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressProviderBase"/> class. 
        ///     Initializes a new instance of the <see cref="HierarchicalProgressBase"/> class.
        /// </summary>
        /// <param name="keepProgressListOrdered">
        ///     <see langword="true"/> if the <see cref="HierarchicalProgressBase.activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </param>
        protected ProgressProviderBase(bool keepProgressListOrdered)
            : base(keepProgressListOrdered)
        {
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public abstract event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the active child progress operations.
        /// </summary>
        public IHierarchicalProgress<IProgressReport>[] ActiveChildProgressOperations
        {
            get
            {
                return this.ActiveChildProgressInfos
                           .Select(x => (IHierarchicalProgress<IProgressReport>)x.Progress)
                           .ToArray();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether an operation is in progress (e.g. to determine if a progress bar should be shown).
        /// </summary>
        public abstract bool IsOperationInProgress { get; protected set; }

        /// <summary>
        ///     Gets or sets a value indicating whether any operation in progress is blocking the UI.
        /// </summary>
        public abstract bool IsUiBlocked { get; protected set; }

        /// <summary>
        ///     Gets or sets the active status, if any.
        /// </summary>
        public abstract IProgressReport Status { get; protected set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates and registers a long running operation in order to provide feedback to the user.
        /// </summary>
        /// <param name="blocksUi">
        ///     <see langword="true"/> if the UI should be blocked while this operation is active.
        /// </param>
        /// <returns>
        ///     A <see cref="IProgress{T}"/> object which allows manipulation of the progress of the operation.
        /// </returns>
        public override IHierarchicalProgress<IProgressReport> CreateProgress(bool blocksUi)
        {
            var childProgress = base.CreateProgress(blocksUi);

            childProgress.Report(new ProgressReport());

            return childProgress;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Recomputes the aggregate <see cref="Status"/> from the operations currently in progress.
        /// </summary>
        protected override void OnStatusChanged()
        {
            this.IsOperationInProgress = this.ActiveChildProgressInfos.Any();
            this.IsUiBlocked = this.IsOperationInProgress && this.ActiveChildProgressInfos.Any(info => info.IsBlockingUi);

            this.Status = this.CalculateAggregateProgressReport();
        }

        #endregion
    }
}