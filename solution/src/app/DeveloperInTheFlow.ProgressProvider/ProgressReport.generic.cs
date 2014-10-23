namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///     Implements a status report for a long-running operation to <see cref="IProgress{T}"/>.
    /// </summary>
    /// <typeparam name="TMessage">
    ///     The message type.
    /// </typeparam>
    public class ProgressReport<TMessage> : IProgressReport<TMessage>
        where TMessage : class
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
        /// </summary>
        public ProgressReport()
        {
            this.Message = default(TMessage);
            this.State = ProgressState.Indeterminate;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message describing the status of the long-running operation.
        /// </param>
        public ProgressReport(TMessage message)
            : this(message, ProgressState.Indeterminate)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
        /// </summary>
        /// <param name="state">
        ///     The state of the long-running operation.
        /// </param>
        public ProgressReport(ProgressState state)
            : this(null, state)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
        /// </summary>
        /// <param name="message">
        ///     The message describing the status of the long-running operation.
        /// </param>
        /// <param name="state">
        ///     The state of the long-running operation.
        /// </param>
        public ProgressReport(
            TMessage message,
            ProgressState state)
        {
            this.Message = message;
            this.State = state;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
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
            TMessage message,
            double progressValue,
            double progressMaximumValue)
            : this(message, progressValue, progressMaximumValue, ProgressState.Normal)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProgressReport{TMessage}"/> class.
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
            TMessage message,
            double progressValue,
            double progressMaximumValue,
            ProgressState state)
            : this(message, state)
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
        public TMessage Message { get; private set; }

        /// <summary>
        ///     Gets the maximum progress value. Used when <see cref="IProgressReport{TMessage}.ProgressValue"/> is defined.
        /// </summary>
        public double ProgressMaximumValue { get; private set; }

        /// <summary>
        ///     Gets the current progress value from [0, <see cref="ProgressMaximumValue"/>]. <see langword="null"/> if the <see cref="IProgressReport{TMessage}.State"/> is <see cref="ProgressState.Indeterminate"/>.
        /// </summary>
        public double? ProgressValue { get; private set; }

        /// <summary>
        ///     Gets the state of the long-running operation.
        /// </summary>
        public ProgressState State { get; private set; }

        #endregion
    }
}