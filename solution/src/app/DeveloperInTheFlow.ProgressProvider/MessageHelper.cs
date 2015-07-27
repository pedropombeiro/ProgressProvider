namespace DeveloperInTheFlow.ProgressProvider
{
    /// <summary>
    ///     Contains helper methods to work with generic messages.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     Specifies the type of the progress report value.
    /// </typeparam>
    public static class MessageHelper<TMessage>
        where TMessage : class
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Verifies if the <paramref name="message"/> is null or in the case it is a string if it's empty.
        /// </summary>
        /// <param name="message">
        ///     The message to verify if it's null or empty.
        /// </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="message"/> is null, or in the case it is a string is empty, otherwise <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(TMessage message)
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