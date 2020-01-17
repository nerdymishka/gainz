
using System;

namespace NerdyMishka.Jobs
{
    public class JobRegistration
    {
        public Type JobType { get; set; }

        public Quartz.ITrigger Trigger { get; set; }

        public string Json { get; set; }
    }
}