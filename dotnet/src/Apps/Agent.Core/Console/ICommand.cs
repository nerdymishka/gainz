
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NerdyMishka.Console 
{
    /// <summary>
    /// Representings a possible command line action.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets a description on what the action does. This is used for
        /// help documentation. 
        /// </summary>
        /// <value></value>
        string Description { get; }

        string Noun { get; }

        IReadOnlyCollection<Type> ArgumentsTypes { get; }

        bool CanExecute(Type argumentType);

        /// <summary>
        /// Executes a command line action. 
        /// </summary>
        /// <param name="parameters">The parameters passed to the command.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>The last exit code.</returns>
        Task<int> ExecuteAsync(object parameters, CancellationToken token = default);
    }

}