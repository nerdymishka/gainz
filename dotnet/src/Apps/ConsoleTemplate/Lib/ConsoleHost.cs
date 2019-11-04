using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting.Internal;
using System.Linq;

namespace NerdyMishka.Extensions.Hosting.Console 
{

    public class ConsoleHost : IConsoleHost, IAsyncDisposable
    {
       
        private ApplicationLifetime applicationLifetime;
        private ILogger<ConsoleHost> logger;
        private IHostLifetime hostLifetime;
        private HostOptions options;

        private IEnumerable<IHostedService> hostedServices;


        public ConsoleHost(
            IServiceProvider services,
            IHostApplicationLifetime applicationLifetime,
            ILogger<ConsoleHost> logger,
            IHostLifetime hostLifetime, 
            IOptions<HostOptions> options) {

            this.applicationLifetime = applicationLifetime as ApplicationLifetime;
            
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            if(this.applicationLifetime == null)
                throw new ArgumentNullException(nameof(applicationLifetime));
            
            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            if(hostLifetime == null)
                throw new ArgumentNullException(nameof(hostLifetime));

            if(options == null || options.Value == null)
                throw new ArgumentNullException(nameof(options));

            this.Services = services;
            this.logger = logger;
            this.hostLifetime = hostLifetime;
            this.options = options.Value;
        }

        public int ExitCode { get; protected set; }
        public AggregateException Exception { get; protected set; }

        public IServiceProvider Services { get; protected set; }

        public void Dispose()
        {
            this.DisposeAsync()
                .GetAwaiter()
                .GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            switch (Services)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.logger.Starting();

            using var combinedCancellationTokenSource = 
                CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken, 
                    applicationLifetime.ApplicationStopping);
            
            var combinedCancellationToken = combinedCancellationTokenSource.Token;

            await this.hostLifetime.WaitForStartAsync(combinedCancellationToken);

            combinedCancellationToken.ThrowIfCancellationRequested();
            this.hostedServices = Services.GetService<IEnumerable<IHostedService>>();

            foreach (var hostedService in this.hostedServices)
            {
                // Fire IHostedService.Start
                await hostedService.StartAsync(combinedCancellationToken).ConfigureAwait(false);
            }

            // Fire IHostApplicationLifetime.Started
            this.applicationLifetime?.NotifyStarted();

           this.logger.Started();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            this.logger.Stopping();

            using (var cts = new CancellationTokenSource(this.options.ShutdownTimeout))
            using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken))
            {
                var token = linkedCts.Token;
                // Trigger IHostApplicationLifetime.ApplicationStopping
                this.applicationLifetime?.StopApplication();

                IList<Exception> exceptions = new List<Exception>();
                if (this.hostedServices != null) // Started?
                {
                    var consoleExceptions = new List<Exception>();
                    foreach (var hostedService in this.hostedServices.Reverse())
                    {
                        token.ThrowIfCancellationRequested();
                        try
                        {
                            await hostedService
                                .StopAsync(token)
                                .ConfigureAwait(false);

                            if(hostedService is IConsoleHostedService)
                            {
                                var consoleHostedService = (IConsoleHostedService)hostedService;

                                if(this.ExitCode == 0)
                                {
                                    this.ExitCode = consoleHostedService.ExitCode;
                                }

                                if(consoleHostedService.Exception != null)
                                {
                                    var innerExceptions = consoleHostedService.Exception.InnerExceptions;
                                    if(innerExceptions != null && innerExceptions.Count > 0)
                                    {
                                        consoleExceptions.AddRange(innerExceptions);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptions.Add(ex);
                        }
                    }

                    if(consoleExceptions.Count > 0)
                    {
                       this.Exception = new AggregateException(consoleExceptions);
                    }
                }

                

                token.ThrowIfCancellationRequested();
                await this.hostLifetime.StopAsync(token);

                // Fire IHostApplicationLifetime.Stopped
                this.applicationLifetime?.NotifyStopped();

                if (exceptions.Count > 0)
                {
                    var ex = new AggregateException("One or more hosted services failed to stop.", exceptions);
                    this.logger.StoppedWithException(ex);
                    throw ex;
                }
            }

            this.logger.Stopped();
        }
    }
}