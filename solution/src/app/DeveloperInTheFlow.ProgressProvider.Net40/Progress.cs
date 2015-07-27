namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///     Provides an IProgress{T} that invokes callbacks for each reported progress value.
    /// </summary>
    /// <typeparam name="T">
    ///     Specifies the type of the progress report value.
    /// </typeparam>
    /// <remarks>
    ///     This class is a copy of System.Progress{T} in .NET 4.5 and should eventually be replaced by it.
    /// </remarks>
    public sealed class Progress<T> : IProgress<T>
    {
        #region Fields

        /// <summary>
        /// The handler specified to the constructor.  This may be null.
        /// </summary>
        private readonly Action<T> handler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Progress{T}"/> class. 
        /// </summary>
        /// <param name="handler">
        /// A handler to invoke for each reported progress value.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="handler"/> is null (Nothing in Visual Basic).
        /// </exception>
        public Progress(Action<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            this.handler = handler;
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// Reports a progress update.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        void IProgress<T>.Report(T value)
        {
            this.OnReport(value);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reports a progress change.
        /// </summary>
        /// <param name="value">The value of the updated progress.</param>
        private void OnReport(T value)
        {
            this.handler(value);
        }

        #endregion
    }
}