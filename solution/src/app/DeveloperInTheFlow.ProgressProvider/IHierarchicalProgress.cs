namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///   Defines the contract for the model representing a long-running operation supporting hierarchy.
    /// </summary>
    /// <remarks>
    ///   Disposing this object will unregister from its source.
    /// </remarks>
    /// <typeparam name="T">The type of progress update value.</typeparam>
    public interface IHierarchicalProgress<in T> : IProgress<T>, 
                                                   IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Creates and registers a child long running operation in order to provide feedback to the user.
        /// </summary>
        /// <param name="blocksUi">
        ///     <see langword="true"/> if the UI should be blocked while this child operation is active.
        /// </param>
        /// <returns>
        ///     A <see cref="IProgress{T}"/> object which allows manipulation of the progress of the operation.
        /// </returns>
        IHierarchicalProgress<T> CreateProgress(bool blocksUi);

        #endregion
    }
}