using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace NerdyMishka.Extensions.Hosting.Console 
{
    public class ConsoleHostedService : IConsoleHostedService
    {
        public int ExitCode { get; internal protected set; }

        private Task task { get; set; }
        private CancellationTokenSource source;
        public AggregateException Exception { get; internal protected set; }
        private IConsoleProgram program;
        private IServiceProvider services;
        private IHostEnvironment environment;

        private IHostApplicationLifetime lifetime;

        private IConsoleArguments arguments;

        private ConsoleHostOptions options;
    
        public ConsoleHostedService(
            IConsoleProgram program, 
            IServiceProvider services, 
            IHostEnvironment environment, 
            IHostApplicationLifetime lifetime,
            IOptions<ConsoleHostOptions> options,
            IConsoleArguments arguments)
        {
            this.program = program;
            this.services = services;
            this.environment = environment;
            this.lifetime = lifetime;
            this.arguments = arguments;
            this.options = options?.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            this.task = this.ExecuteAsync();
            if(this.task.Status == TaskStatus.Created)
                this.task.Start();
          

            return Task.CompletedTask;
        }

        protected async virtual Task ExecuteAsync()
        {
            this.source = new CancellationTokenSource();
            if(this.options != null && this.options.Timeout.HasValue)
                this.source = new CancellationTokenSource(this.options.Timeout.Value);
           
            var context = new HostContext(this.environment, this.services, this.arguments);
            int code = 1;

            try 
            {   
                code = await this.program.ExecuteAsync(context, this.source.Token);
            } 
            catch (Exception ex) 
            {
                this.Exception = new AggregateException(ex);
            } 
            finally 
            {
                this.lifetime?.StopApplication();
                this.ExitCode = code;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            if(this.task != null)
            {
                var executingTask = this.task;
                this.task = null;

                switch(executingTask.Status)
                {
                    case TaskStatus.Running:
                    case TaskStatus.WaitingForActivation:
                    case TaskStatus.WaitingForChildrenToComplete:
                    case TaskStatus.WaitingToRun:
                        this.source.Cancel(false);
                    break;
                }

                 if(executingTask.Exception != null && this.Exception == null)
                    this.Exception = executingTask.Exception;
            }

            return Task.CompletedTask;
        }
    }
}