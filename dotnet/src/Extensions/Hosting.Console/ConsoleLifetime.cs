


using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace NerdyMishka.Extensions.Hosting.Console
{
     public class ConsoleLifetime : IHostLifetime, IDisposable
    {
        private readonly ManualResetEvent shutdownBlock = new ManualResetEvent(false);
        private CancellationTokenRegistration applicationStartedRegistration;
        private CancellationTokenRegistration applicationStoppingRegistration;

        public ConsoleLifetime(
            IOptions<ConsoleLifetimeOptions> options, 
            IHostEnvironment environment, 
            IHostApplicationLifetime applicationLifetime, 
            IOptions<HostOptions> hostOptions)
            : this(options, environment, applicationLifetime, hostOptions, NullLoggerFactory.Instance) { }

        public ConsoleLifetime(
            IOptions<ConsoleLifetimeOptions> options, 
            IHostEnvironment environment, 
            IHostApplicationLifetime applicationLifetime, 
            IOptions<HostOptions> hostOptions, 
            ILoggerFactory loggerFactory)
        {
            Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            HostOptions = hostOptions?.Value ?? throw new ArgumentNullException(nameof(hostOptions));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        private ConsoleLifetimeOptions Options { get; }

        private IHostEnvironment Environment { get; }

        private IHostApplicationLifetime ApplicationLifetime { get; }

        private HostOptions HostOptions { get; }

        private ILogger Logger { get; }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            if (!Options.SuppressStatusMessages)
            {
                this.applicationStartedRegistration = ApplicationLifetime.ApplicationStarted.Register(state =>
                {
                    ((ConsoleLifetime)state).OnApplicationStarted();
                },
                this);
                this.applicationStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state =>
                {
                    ((ConsoleLifetime)state).OnApplicationStopping();
                },
                this);
            }

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            System.Console.CancelKeyPress += OnCancelKeyPress;

            // Console applications start immediately.
            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            Logger.LogDebug("Application started. Press Ctrl+C to shut down.");
            Logger.LogDebug("Hosting environment: {envName}", Environment.EnvironmentName);
            Logger.LogDebug("Content root path: {contentRoot}", Environment.ContentRootPath);
        }

        private void OnApplicationStopping()
        {
            Logger.LogDebug("Application is shutting down...");
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            ApplicationLifetime.StopApplication();
            if(!this.shutdownBlock.WaitOne(HostOptions.ShutdownTimeout))
            {
                Logger.LogInformation("Waiting for the host to be disposed. Ensure all 'IHost' instances are wrapped in 'using' blocks.");
            }
            this.shutdownBlock.WaitOne();
            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/aspnet/AspNetCore/issues/6526
            System.Environment.ExitCode = 0;
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            ApplicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // There's nothing to do here
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.shutdownBlock.Set();

            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            System.Console.CancelKeyPress -= OnCancelKeyPress;

            this.applicationStartedRegistration.Dispose();
            this.applicationStoppingRegistration.Dispose();
        }
    }
}