
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace NerdyMishka.Jobs
{
    public class JobsHostedService : IHostedService
    {
        private ISchedulerFactory factory;

        public IList<JobRegistration> JobRegistry { get; private set; }  = new List<JobRegistration>();

        public IScheduler JobScheduler { get; private set; }

        public IJobFactory JobFactory { get; private set; }

        public JobsHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<JobRegistration> jobs)
        {
            this.factory = schedulerFactory;
            this.JobFactory = jobFactory;
            this.JobRegistry = new List<JobRegistration>(jobs);
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            this.JobScheduler = await this.factory.GetScheduler(cancellationToken);
            this.JobScheduler.JobFactory = this.JobFactory;

            foreach(var registration in this.JobRegistry)
            {
                var job = CreateJob(registration);

                await this.JobScheduler.ScheduleJob(job, registration.Trigger, cancellationToken);
            }

            await this.JobScheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.JobScheduler?.Shutdown(cancellationToken);
        }

        
        private static IJobDetail CreateJob(JobRegistration registration)
        {
            var jobType = registration.JobType;
            return JobBuilder
                .Create(jobType)
                .WithIdentity(jobType.FullName)
                .WithDescription(jobType.FullName)
                .UsingJobData("configuration", registration.Json)
                .Build();
        }

       
    }
}