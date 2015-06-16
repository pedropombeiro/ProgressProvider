namespace DeveloperInTheFlow.ProgressProvider
{
    using System;

    /// <summary>
    ///   Defines the contract for the model responsible for describing a status report for a long-running operation to <see cref="IProgress{T}"/>.
    /// </summary>
    [Obsolete("Use generic version of IProgressReport")]
    public interface IProgressReport : IProgressReport<string>
    {
    }
}