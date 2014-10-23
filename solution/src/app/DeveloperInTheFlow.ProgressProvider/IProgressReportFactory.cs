namespace DeveloperInTheFlow.ProgressProvider
{
    /// <summary>
    ///   Defines the contract for the factory responsible for the creation of <see cref="IProgressReport{TMessage}"/>s.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public interface IProgressReportFactory<TMessage>
        where TMessage : class
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
        IProgressReport<TMessage> Create(
            TMessage message,
            double progressValue,
            double progressMaximumValue,
            ProgressState state);

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
        IProgressReport<TMessage> Create(
            TMessage message,
            ProgressState state);

        /// <summary>
        ///     Creates a new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
        /// </summary>
        /// <returns>
        ///     A new instance of a class implementing <see cref="IProgressReport{TMessage}"/>.
        /// </returns>
        IProgressReport<TMessage> Create();

        #endregion
    }
}