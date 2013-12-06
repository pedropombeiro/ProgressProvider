namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///     Extension methods for the <see cref="IProgress{T}"/> interface.
    /// </summary>
    public static class ProgressExtensions
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Reports a progress update.
        /// </summary>
        /// <param name="progress">
        ///     The object being extended.
        /// </param>
        /// <param name="message">
        ///     The message of the updated progress.
        /// </param>
        public static void Report(
            this IProgress<IProgressReport> progress, 
            string message)
        {
            progress.Report(new ProgressReport(message));
        }

        /// <summary>
        ///     Reports a progress update.
        /// </summary>
        /// <param name="progress">
        ///     The object being extended.
        /// </param>
        /// <param name="message">
        ///     The message of the updated progress.
        /// </param>
        /// <param name="progressValue">
        ///     The current progress from [0, <paramref name="progressMaximumValue"/>].
        /// </param>
        /// <param name="progressMaximumValue">
        ///     The maximum progress value.
        /// </param>
        public static void Report(
            this IProgress<IProgressReport> progress, 
            string message, 
            double progressValue, 
            double progressMaximumValue)
        {
            progress.Report(new ProgressReport(message, progressValue, progressMaximumValue));
        }

        /// <summary>
        ///     Reports a progress update.
        /// </summary>
        /// <param name="progress">
        ///     The object being extended.
        /// </param>
        /// <param name="message">
        ///     The message of the updated progress.
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
        public static void Report(
            this IProgress<IProgressReport> progress, 
            string message, 
            double progressValue, 
            double progressMaximumValue, 
            ProgressState state)
        {
            progress.Report(new ProgressReport(message, progressValue, progressMaximumValue, state));
        }

        /// <summary>
        ///     Reports a progress update.
        /// </summary>
        /// <param name="progress">
        ///     The object being extended.
        /// </param>
        /// <param name="message">
        ///     The message of the updated progress.
        /// </param>
        /// <param name="state">
        ///     The state of the long-running operation.
        /// </param>
        public static void Report(
            this IProgress<IProgressReport> progress, 
            string message, 
            ProgressState state)
        {
            progress.Report(new ProgressReport(message, state));
        }

        #endregion
    }
}