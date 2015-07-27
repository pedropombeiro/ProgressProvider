namespace DeveloperInTheFlow.ProgressProvider
{
    using System.Linq;

    /// <summary>
    ///     Represents additional information about a <see cref="HierarchicalProgress{TMessage}"/> instance.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class ProgressInfo<TMessage>
        where TMessage : class
    {
        #region Fields

        /// <summary>
        ///     <see langword="true"/> if the operation is supposed to block the UI from user interaction.
        /// </summary>
        private readonly bool isBlockingUi;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressInfo{TMessage}"/> class.
        /// </summary>
        /// <param name="progress">
        ///     The progress operation represented by this information object.
        /// </param>
        /// <param name="blocksUi">
        ///     <see langword="true"/> if the operation is supposed to block the UI from user interaction.
        /// </param>
        public ProgressInfo(
            HierarchicalProgress<TMessage> progress,
            bool blocksUi)
        {
            this.Progress = progress;
            this.isBlockingUi = blocksUi;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether the progress is supposed to block the UI from user interaction.
        /// </summary>
        public bool IsBlockingUi
        {
            get
            {
                return this.isBlockingUi || this.Progress.ActiveChildProgressInfos.Any(x => x.IsBlockingUi);
            }
        }

        /// <summary>
        ///     Gets or sets the last reported status.
        /// </summary>
        public IProgressReport<TMessage> LastReportedStatus { get; set; }

        /// <summary>
        ///     Gets the progress operation represented by this information object.
        /// </summary>
        public HierarchicalProgress<TMessage> Progress { get; private set; }

        #endregion
    }
}