

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NerdyMisha.Extensions.Hosting;

namespace NerdyMishka.Extensions.Hosting
{
    public class ApplicationHost : IApplicationHost,  IAsyncDisposable
    {
        private IServiceCollection services;
        private IStartup startup;
        private IServiceProvider serviceProvider;

        private ExceptionDispatchInfo applicationServicesException;

        private IHostApplicationLifetime applicationLifetime;

        private HostedServiceExecutor hostedServiceExecutor;

        private IServiceProvider hostingServiceProvider;
        private IConfiguration configuration;
        private AggregateException exception;

        private ILogger logger =  NullLogger.Instance;
        private bool stopped = false;

        public IServiceProvider ServiceProvider => this.serviceProvider;

        public ApplicationHost(
            IServiceCollection services, 
            IServiceProvider hostingServiceProvider,
            ApplicationHostOptions options, 
            IConfiguration configuration,
            AggregateException exception) {

            this.services = services;
            this.configuration = configuration;
            this.exception = exception;
            this.hostingServiceProvider = hostingServiceProvider;
            
            services.AddSingleton<ApplicationLifetime>();
            services.AddSingleton(services
                => services.GetService<ApplicationLifetime>() as IHostApplicationLifetime);
        }

        public void Initialize()
        {
            try {
                this.DemandApplicationServices();

            } catch (Exception ex)
            {
                // EnsureApplicationServices may have failed due to a missing or throwing Startup class.
                if (this.serviceProvider == null)
                {
                    this.serviceProvider = this.services.BuildServiceProvider();
                }

                this.applicationServicesException = ExceptionDispatchInfo.Capture(ex);
            }
        }

        private AppRunDelegate BuildApplication()
        {
            try {
                this.applicationServicesException.Throw();
                var builder = this.serviceProvider.GetRequiredService<IApplicationBuilder>();
                builder.ApplicationServices = this.hostingServiceProvider;
               
                

                var filters = this.serviceProvider.GetService<IEnumerable<IStartupFilter>>();
                Action<IApplicationBuilder> configure = this.startup.Configure;
                foreach(var filter in filters.Reverse())
                {
                    configure = filter.Configure(configure);
                }

                configure(builder);

                return builder.Build();
              
            } catch (Exception ex)
            {

                var eh = this.serviceProvider.GetService<IApplicationExceptionHandler>();
                var l = this.serviceProvider.GetRequiredService<ILogger<ApplicationHost>>();
                l.ApplicationError(ex);

                

                return (context) => {
                    context["LastException"] = ex;
                    context.ExitCode = 1;

                    return eh.ExecuteAsync(context);
                };
            }
        }

        public void Start()
        {
            this.StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.logger = this.serviceProvider.GetRequiredService<ILoggerFactory>()
                .CreateLogger("NerdyMishka.Extensions.Hosting.Diagnostics");

            this.logger.Starting();

            var application = BuildApplication();
            this.applicationLifetime = this.serviceProvider.GetRequiredService<ApplicationLifetime>();
            this.hostedServiceExecutor = this.serviceProvider.GetRequiredService<HostedServiceExecutor>();

            await hostedServiceExecutor.StartAsync(cancellationToken)
                .ConfigureAwait(false);

            if(this.applicationLifetime is ApplicationLifetime)
                ((ApplicationLifetime)this.applicationLifetime).NotifyStarted();
        }

        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            if(this.stopped)
                return;

            this.stopped = true;

            this.logger.Shutdown();

            var timeoutToken = new CancellationTokenSource(1000).Token;
            if (!cancellationToken.CanBeCanceled)
            {
                cancellationToken = timeoutToken;
            }
            else
            {
                cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken).Token;
            }

            this.applicationLifetime?.StopApplication();

            if(this.hostedServiceExecutor != null)
                await this.hostedServiceExecutor.StopAsync(cancellationToken)
                    .ConfigureAwait(false);

            if(this.applicationLifetime is ApplicationLifetime)
                ((ApplicationLifetime)this.applicationLifetime).NotifyStopped();
        }


        private void DemandApplicationServices()
        {
            if(this.serviceProvider == null)
            {
                this.DemandStartup();
                this.serviceProvider = this.startup.ConfigureServices(this.services);
            }
        }

        private void DemandStartup()
        {
            if(this.startup != null)
            {
                return;
            }

            this.startup = this.hostingServiceProvider.GetService<IStartup>();

            if (this.startup == null)
            {
                throw new InvalidOperationException(
                    $"No application configured. Please specify startup via "+
                    "IWebHostBuilder.UseStartup, IWebHostBuilder.Configure, " +
                    $"injecting {nameof(IStartup)} or specifying the startup" +
                    $"assembly via Application:Startup "+
                    " in the web host configuration.");
            }
        }

        public void Dispose()
        {
            DisposeAsync().GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            if (!this.stopped)
            {
                try
                {
                    await StopAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.logger.ServerShutdownException(ex);
                }
            }

            await DisposeServiceProviderAsync(this.serviceProvider)
                .ConfigureAwait(false);
            await DisposeServiceProviderAsync(this.hostingServiceProvider)
                .ConfigureAwait(false);
        }

        private async ValueTask DisposeServiceProviderAsync(IServiceProvider serviceProvider)
        {
            switch (serviceProvider)
            {
                case IAsyncDisposable asyncDisposable:
                    await asyncDisposable.DisposeAsync();
                    break;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }
    }
}