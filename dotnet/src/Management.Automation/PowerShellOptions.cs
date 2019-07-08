using System.Collections;
using System.Collections.Generic;

namespace NerdyMishka.Management.Automation
{
    public class PowerShellOptions
    {
        public IDictionary EnvironmentVariables { get; set; }

        public IDictionary Variables { get; set; }

        public bool AppendModulePath { get; set; } = true;

        public bool AppendPath { get; set; } = true;

        public bool OverwriteKnownVariables { get; set; } = false;

        
    }
}