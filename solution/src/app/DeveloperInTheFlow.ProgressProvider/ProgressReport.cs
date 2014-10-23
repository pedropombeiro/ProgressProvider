namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///     Implements a status report for a long-running operation to <see cref="IProgress{T}"/>.
    /// </summary>
    [Obsolete("Use generic version of IProgressReport")]
    public class ProgressReport : ProgressReport<string>,
                                  IProgressReport
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        public ProgressReport()
            : base(string.Empty)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message describing the status of the long-running operation.
        /// </param>
        public ProgressReport(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        /// <param name="state">
        ///     The state of the long-running operation.
        /// </param>
        public ProgressReport(ProgressState state)
            : base(state)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message describing the status of the long-running operation.
        /// </param>
        /// <param name="state">
        ///     The state of the long-running operation.
        /// </param>
        public ProgressReport(
            string message,
            ProgressState state)
            : base(message, state)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
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
        public ProgressReport(
            string message,
            double progressValue,
            double progressMaximumValue)
            : base(message, progressValue, progressMaximumValue)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
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
        public ProgressReport(
            string message,
            double progressValue,
            double progressMaximumValue,
            ProgressState state)
            : base(message, progressValue, progressMaximumValue, state)
        {
        }

        #endregion
    }
}