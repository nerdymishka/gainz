
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;


namespace NerdyMishka.Extensions.Hosting.Console
{
    public static class IHostExtensions
    {
        public static async Task<int> RunConsoleProgramAsync(this IHost host, CancellationToken token = default)
        {
            IConsoleHost host2 = host as IConsoleHost;
            if(host2 == null)
                throw new ArgumentException($"host must be of type {nameof(IConsoleHost)}", nameof(host));

            await host2.RunAsync(token);
            return host2.ExitCode;
        }

        public static int RunConsoleProgram(this IHost host, CancellationToken token = default)
        {
            IConsoleHost host2 = host as IConsoleHost;
            if(host2 == null)
                throw new ArgumentException($"host must be of type {nameof(IConsoleHost)}", nameof(host));

            host2.RunAsync(token)
                        .GetAwaiter()
                        .GetResult();

            return host2.ExitCode;
        }
    }
}