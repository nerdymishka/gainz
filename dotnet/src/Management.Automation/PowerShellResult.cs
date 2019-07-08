using System;

namespace NerdyMishka.Management.Automation
{
    public class PowerShellResult
    {
        public int ExitCode { get; set; }

        public bool HasStandardErrorData { get; set; } 

        public Exception LastException  { get; set; }

        public string Command { get; set; }
    }
}