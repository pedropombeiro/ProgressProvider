namespace DeveloperInTheFlow.ProgressProvider
{
    using System;
    using System.ComponentModel;

    /// <summary>
    ///     Defines the contract for the service responsible for managing the communication of long-running operations to the user.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of progress update value.
    /// </typeparam>
    public interface IProgressProvider<T> : INotifyPropertyChanged,
                                            IDisposable
    {
        #region Public Properties

        /// <summary>
        ///     Gets the active child progress operations.
        /// </summary>
        IHierarchicalProgress<T>[] ActiveChildProgressOperations { get; }

        /// <summary>
        ///     Gets a value indicating whether an operation is in progress (e.g. to determine if a progress bar should be shown).
        /// </summary>
        bool IsOperationInProgress { get; }

        /// <summary>
        ///     Gets a value indicating whether any operation in progress is blocking the UI.
        /// </summary>
        bool IsUiBlocked { get; }

        /// <summary>
        /// Gets the active status, if any.
        /// </summary>
        T Status { get; }

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
        IHierarchicalProgress<T> CreateProgress(bool blocksUi);

        #endregion
    }
}