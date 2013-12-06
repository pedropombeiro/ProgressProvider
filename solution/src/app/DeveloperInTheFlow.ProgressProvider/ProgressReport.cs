namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///     Implements a status report for a long-running operation to <see cref="IProgress{T}"/>.
    /// </summary>
    public class ProgressReport : IProgressReport
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        public ProgressReport()
        {
            this.Message = string.Empty;
            this.State = ProgressState.Indeterminate;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message describing the status of the long-running operation.
        /// </param>
        public ProgressReport(string message)
        {
            this.Message = message;
            this.State = ProgressState.Indeterminate;
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
        public ProgressReport(string message, 
                              ProgressState state)
        {
            this.Message = message;
            this.State = state;
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
        public ProgressReport(string message, 
                              double progressValue, 
                              double progressMaximumValue)
            : this(message, progressValue, progressMaximumValue, ProgressState.Normal)
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
        public ProgressReport(string message, 
                              double progressValue, 
                              double progressMaximumValue, 
                              ProgressState state)
        {
            if (double.IsNaN(progressValue) || double.IsInfinity(progressValue))
            {
                throw new ArgumentException();
            }

            this.Message = message;
            this.State = state;
            this.ProgressValue = progressValue;
            this.ProgressMaximumValue = progressMaximumValue;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the message describing the status of the long-running operation.
        /// </summary>
        [CanBeNull]
        public string Message { get; private set; }

        /// <summary>
        ///     Gets the maximum progress value. Used when <see cref="IProgressReport.ProgressValue"/> is defined.
        /// </summary>
        public double ProgressMaximumValue { get; private set; }

        /// <summary>
        ///     Gets the current progress value from [0, 1]. <see langword="null"/> if the <see cref="IProgressReport.State"/> is <see cref="ProgressState.Indeterminate"/>.
        /// </summary>
        [CanBeNull]
        public double? ProgressValue { get; private set; }

        /// <summary>
        ///     Gets the state of the long-running operation.
        /// </summary>
        public ProgressState State { get; private set; }

        #endregion
    }
}