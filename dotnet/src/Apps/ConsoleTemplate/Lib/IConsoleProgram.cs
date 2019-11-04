

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Extensions.Hosting.Console
{
    public interface IConsoleProgram
    {
        Task<int> ExecuteAsync(IHostContext context, CancellationToken cancellationToken = default);
    }

    public class DelegateConsoleProgram : IConsoleProgram
    {
        private Func<IHostContext, int> execute;

        public DelegateConsoleProgram(Func<IHostContext, int> execute)
        {
            this.execute = execute;
        }

        public Task<int> ExecuteAsync(IHostContext context, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => execute(context), cancellationToken);
        }
    }
}