using System;
using System.Globalization;
using System.Management.Automation.Host;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Management.Automation
{
    public class AppHost : PSHost
    {
        private readonly Guid hostId = Guid.NewGuid();
        private readonly AppHostUI hostUI;
        private readonly CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        private readonly CultureInfo cultureUiInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;
        private  static Version s_version;

        private string name;

        public AppHost(AppHostUI hostUI)
        {
            this.hostUI = hostUI;
            this.name = this.hostUI.Options.ApplicationName + "_PowerShellHost";
        }

        public AppHost(IConsole console, ILogger logger, AppHostOptions options = null)
        {
            
            this.hostUI = new AppHostUI(console, logger, options);
            this.name = this.hostUI.Options.ApplicationName + "_PowerShellHost";
        }

        public override CultureInfo CurrentCulture => this.cultureInfo;

        public override CultureInfo CurrentUICulture => this.cultureUiInfo;

        public override Guid InstanceId => this.hostId;

        public int ExitCode { get; private set; } = -1;

        public Exception LastException  {get; set; }

        public bool HasStandardErrorData  => this.hostUI.HasStandardErrorData;

        public override string Name => this.name;
        public override PSHostUserInterface UI => this.hostUI;

        public override Version Version 
        {
            get{
                if(s_version == null) {
                    var asm = typeof(System.Management.Automation.PSCustomObject).Assembly;
                    var attrs = asm.GetCustomAttributes(typeof(System.Reflection.AssemblyVersionAttribute), false);
                    if(attrs != null && attrs.Length > 0)
                    {
                        var first = (System.Reflection.AssemblyVersionAttribute)attrs[0];
                        s_version = new Version(first.Version);
                    }

                    s_version = new Version(6, 0);
                }

                return s_version;
            }
        }

        public override void EnterNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void ExitNestedPrompt()
        {
            throw new NotImplementedException();
        }

        public override void NotifyBeginApplication()
        {
            
        }

        public override void NotifyEndApplication()
        {
            
        }

        public override void SetShouldExit(int exitCode)
        {
            this.ExitCode = exitCode;
        }
    }
}