
using Quartz;
using Quartz.Spi;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace NerdyMishka.Jobs
{
    public class JobFactory : IJobFactory
    {
        private IServiceProvider serviceProvider;

        public JobFactory(IServiceProvider provider)
        {
            this.serviceProvider = provider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var type = bundle.JobDetail.JobType;
            return (IJob)this.serviceProvider.GetRequiredService(type);
        }

        public void ReturnJob(IJob job)
        {
            if(job is IDisposable)
                ((IDisposable)job).Dispose();
        }
    }
}