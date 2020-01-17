
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NerdyMishka.Extensions.Logging;
using Quartz;

namespace NerdyMishka.Jobs 
{

    public abstract class Job : Quartz.IJob
    {
        protected Stopwatch StopWatch { get; set; }

        protected ITelemetryClient TelemetryClient { get; set; }

        protected bool Refire { get; set; } = false;

        protected void TrackStart(
            IJobExecutionContext context, 
            Dictionary<string, string> properties = null, 
            Dictionary<string, double> metrics = null)
        {
            properties = properties ?? new Dictionary<string, string>();
            properties.Add("firedAt", context.FireTimeUtc.ToString("o"));
            properties.Add("refireCount", context.RefireCount.ToString());
            properties.Add("jobName", context.JobDetail.JobType.Name);
            properties.Add("jobType", context.JobDetail.JobType.FullName);
            this.TelemetryClient?.TrackEvent("Jobs/JobStarted", properties, metrics);
        }

        protected void TrackException(Exception exception)
        {
            this.TelemetryClient?.TrackException(exception);
            this.TelemetryClient?.Flush();
        } 

        protected void TrackEnd(
            IJobExecutionContext context, 
            Dictionary<string, string> properties = null, 
            Dictionary<string, double> metrics = null
        )
        {
            properties = properties ?? new Dictionary<string, string>();
            properties.Add("firedAt", context.FireTimeUtc.ToString("o"));
            properties.Add("refireCount", context.RefireCount.ToString());
            properties.Add("jobName", context.JobDetail.JobType.Name);
            properties.Add("jobType", context.JobDetail.JobType.FullName);

            this.TelemetryClient?.TrackEvent($"Jobs/JobFinished", properties, metrics);
        }

        public virtual async Task Execute(IJobExecutionContext context)
        {
            
            this.StopWatch = new Stopwatch();
            var stopwatch = new Stopwatch();
            try {
                stopwatch.Start();
                await this.PerformJobAsync(context);
            } catch(JobExecutionException ex) {
                throw ex; 
            } catch(Exception ex) {
                this.TrackException(ex);
                throw new JobExecutionException(ex, this.Refire);
            } finally {
                stopwatch.Stop();
                this.TelemetryClient?
                    .GetMetric("Jobs", context.JobDetail.JobType.Name, "ElaspedMilliseconds")?
                    .TrackValue(stopwatch.ElapsedMilliseconds);
            }
        }

        public abstract Task PerformJobAsync(IJobExecutionContext context);

    }
}