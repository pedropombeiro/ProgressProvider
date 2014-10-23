namespace DeveloperInTheFlow.ProgressProvider
{
    /// <summary>
    ///     Describes the state of a progress operation.
    /// </summary>
    public enum ProgressState
    {
        /// <summary>
        ///     No operation is in progress.
        /// </summary>
        None,

        /// <summary>
        ///     The operation in progress is running normally and the length is undetermined.
        /// </summary>
        Indeterminate,

        /// <summary>
        ///     The operation in progress is running normally and the length is determined.
        /// </summary>
        Normal,

        /// <summary>
        ///     The operation in progress has experienced an error.
        /// </summary>
        Error,
    }
}