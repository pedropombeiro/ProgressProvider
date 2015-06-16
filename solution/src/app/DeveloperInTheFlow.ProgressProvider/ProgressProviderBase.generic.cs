namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    ///     The base implementation for a <see cref="IProgressProvider{IProgressReport}"/>.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public abstract class ProgressProviderBase<TMessage> : HierarchicalProgressBase<TMessage>, 
                                                           IProgressProvider<IProgressReport<TMessage>>
        where TMessage : class
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressProviderBase{TMessage}"/> class. 
        /// </summary>
        /// <param name="keepProgressListOrdered">
        ///     <see langword="true"/> if the <see cref="HierarchicalProgressBase{TMessage}.activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </param>
        /// <param name="progressReportFactory">
        ///     The factory used to create <see cref="IProgressReport{TMessage}"/> instances.
        /// </param>
        protected ProgressProviderBase(bool keepProgressListOrdered, 
                                       IProgressReportFactory<TMessage> progressReportFactory)
            : base(keepProgressListOrdered, progressReportFactory)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressProviderBase{TMessage}"/> class. 
        /// </summary>
        /// <param name="keepProgressListOrdered">
        ///     <see langword="true"/> if the <see cref="HierarchicalProgressBase{TMessage}.activeChildProgressInfos"/> list should be kept ordered with the most recently updated child progress on the end of the list.
        /// </param>
        protected ProgressProviderBase(bool keepProgressListOrdered)
            : base(keepProgressListOrdered, new GenericProgressReportFactory())
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
        public IHierarchicalProgress<IProgressReport<TMessage>>[] ActiveChildProgressOperations
        {
            get
            {
                return this.ActiveChildProgressInfos
                           .Select(x => (IHierarchicalProgress<IProgressReport<TMessage>>)x.Progress)
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
        public abstract IProgressReport<TMessage> Status { get; protected set; }

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
        public override IHierarchicalProgress<IProgressReport<TMessage>> CreateProgress(bool blocksUi)
        {
            var childProgress = base.CreateProgress(blocksUi);

            childProgress.Report(this.ProgressReportFactory.Create());

            return childProgress;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Re-computes the aggregate <see cref="Status"/> from the operations currently in progress.
        /// </summary>
        protected override void OnStatusChanged()
        {
            this.IsOperationInProgress = this.ActiveChildProgressInfos.Any();
            this.IsUiBlocked = this.IsOperationInProgress && this.ActiveChildProgressInfos.Any(info => info.IsBlockingUi);

            this.Status = this.CalculateAggregateProgressReport();
        }

        #endregion

        /// <summary>
        ///     The factory which create <see cref="ProgressReport{TMessage}"/> instances.
        /// </summary>
        private class GenericProgressReportFactory : IProgressReportFactory<TMessage>
        {
            #region Public Methods and Operators

            public IProgressReport<TMessage> Create(
                TMessage message, 
                double progressValue, 
                double progressMaximumValue, 
                ProgressState state)
            {
                return new ProgressReport<TMessage>(message, progressValue, progressMaximumValue, state);
            }

            public IProgressReport<TMessage> Create(
                TMessage message, 
                ProgressState state)
            {
                return new ProgressReport<TMessage>(message, state);
            }

            /// <summary>
            ///     Creates a new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </summary>
            /// <returns>
            ///     A new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
            /// </returns>
            public IProgressReport<TMessage> Create()
            {
                return new ProgressReport<TMessage>();
            }

            #endregion
        }
    }
}