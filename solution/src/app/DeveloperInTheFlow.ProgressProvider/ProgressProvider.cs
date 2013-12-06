namespace DeveloperInTheFlow.ProgressProvider
{
    using System.ComponentModel;

    /// <summary>
    ///     The service responsible for managing the communication of long-running operations to the user.
    /// </summary>
    /// <remarks>
    /// TODO : [lmbpop0 02.12.13 16:09] Test this.
    /// </remarks>
    [NotifyPropertyChangedAspect]
    public class ProgressProvider : HierarchicalProgressBase, 
                                    IProgressProvider<IProgressReport>
    {
        #region Fields

        /// <summary>
        ///     The service responsible for the navigation between the full-sized screens of the app.
        /// </summary>
        private readonly INavigationService navigationService;

        /// <summary>
        ///     <see langword="true"/> if the <see cref="navigationService"/> has been blocked by us.
        /// </summary>
        private bool isNavigationServiceBlocked;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressProvider"/> class.
        /// </summary>
        /// <param name="navigationService">
        ///     The service responsible for the navigation between the full-sized screens of the app.
        /// </param>
        public ProgressProvider(INavigationService navigationService)
            : base(false)
        {
            this.navigationService = navigationService;
        }

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether an operation is in progress (e.g. to determine if a progress bar should be shown).
        /// </summary>
        [Notify]
        public bool IsOperationInProgress { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether any operation in progress is blocking the UI.
        /// </summary>
        [Notify]
        public bool IsUiBlocked { get; private set; }

        /// <summary>
        ///     Gets the active status, if any.
        /// </summary>
        [Notify]
        public IProgressReport Status { get; private set; }

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

            this.OnReport(childProgress, new ProgressReport());

            return childProgress;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Raises the <seealso cref="INotifyPropertyChanged.PropertyChanged" /> event.
        /// </summary>
        /// <param name="propertyName">
        ///     The name of the property which changed.
        /// </param>
        /// <remarks>
        ///     TODO: Rename to InvokePropertyChanged (also in Aspects)
        /// </remarks>
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     Recomputes the aggregate <see cref="Status"/> from the operations currently in progress and manages the status of <see cref="INavigationService.IsEnabled"/>.
        /// </summary>
        protected override void OnStatusChanged()
        {
            this.IsOperationInProgress = this.ActiveChildProgressInfos.Any();
            this.IsUiBlocked = this.IsOperationInProgress && this.ActiveChildProgressInfos.Any(info => info.IsBlockingUi);

            this.Status = this.CalculateAggregateProgressReport();

            if (this.IsUiBlocked != this.isNavigationServiceBlocked)
            {
                this.isNavigationServiceBlocked = this.IsUiBlocked;

                this.navigationService.Enable(!this.IsUiBlocked);
            }
        }

        #endregion
    }
}