namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///   Defines the contract for the model responsible for describing a status report for a long-running operation to <see cref="IProgress{T}"/>.
    /// </summary>
    public interface IProgressReport
    {
        #region Public Properties

        /// <summary>
        ///     Gets the message describing the status of the long-running operation.
        /// </summary>
        string Message { get; }

        /// <summary>
        ///     Gets the maximum progress value. Used when <see cref="ProgressValue"/> is defined.
        /// </summary>
        double ProgressMaximumValue { get; }

        /// <summary>
        ///     Gets the current progress value from [0, <see cref="ProgressMaximumValue"/>]. <see langword="null"/> if the <see cref="State"/> is <see cref="ProgressState.Indeterminate"/>.
        /// </summary>
        double? ProgressValue { get; }

        /// <summary>
        ///     Gets the state of the long-running operation.
        /// </summary>
        ProgressState State { get; }

        #endregion
    }
}