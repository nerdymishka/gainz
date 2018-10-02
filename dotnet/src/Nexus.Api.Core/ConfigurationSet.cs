using System;

namespace Nexus.Api
{
    public class ConfigurationSet
    {
        public int? Id { get; set; }

        public string Name { get; set; } 

        public int? OperationalEnvironmentId { get; set; }

        public string OperationalEnvironmentName { get; set; }
    }

}