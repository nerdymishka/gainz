
using System;
using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Management.Automation
{
    public class PowerShellService
    {
        private PSHost host;
        private InitialSessionState sessionState;
        private PowerShellOptions options;
        private ILogger logger;

        public PowerShellService(PSHost host, ILogger<PowerShellService> logger,  PowerShellOptions options = null)
        {
            this.host = host;
            this.logger = logger;

            this.options = options ?? new PowerShellOptions();
        }

        private InitialSessionState PrepareState()
        {
            var state = InitialSessionState.CreateDefault();

            if(this.options.EnvironmentVariables != null & this.options.EnvironmentVariables.Count > 0)
            {
                var knownKeys = System.Environment.GetEnvironmentVariables()
                    .Keys
                    .Cast<string>()
                    .Select(o => o.ToUpperInvariant())
                    .ToList();

                foreach(var key in this.options.EnvironmentVariables.Keys)
                {
                    var s = key.ToString().ToUpperInvariant();
                    if(knownKeys.Contains(s))
                    {
                        if(s == "PSMODULES" && this.options.AppendModulePath) {
                            continue;
                        }

                        if(s == "PATH" && this.options.AppendPath) {
                            continue;
                        }

                        if(this.options.OverwriteKnownVariables) {
                            state.EnvironmentVariables.Add(
                                new SessionStateVariableEntry(s, this.options.EnvironmentVariables[key], null));
                        }

                        continue;
                    }

                    state.EnvironmentVariables.Add(
                        new SessionStateVariableEntry(s, this.options.EnvironmentVariables[key], null));
                }
            }

            if(this.options.Variables != null && this.options.Variables.Count > 0)
            {
                foreach(var key in this.options.Variables.Keys) {
                    state.Variables.Add(
                        new SessionStateVariableEntry(
                            key.ToString(), this.options.Variables[key], null));
                }
            }

            state.AuthorizationManager = new AuthorizationManager(this.host.Name);
       

            return state;
        }

        public PowerShellResult Invoke()
        {
    
            var state = this.PrepareState();
            int exitCode = -1;
            Exception LastException = null;
            AppHost appHost = null;
            
            using (var runspace = RunspaceFactory.CreateRunspace(host, state))
            {
                runspace.Open();
                
                if(this.options.EnvironmentVariables != null)
                {
                    if(this.options.EnvironmentVariables.Count > 0)
                    {
                        var proxy = runspace.SessionStateProxy;
                        var env = this.options.EnvironmentVariables;
                        if(this.options.AppendModulePath && env.Contains("PSMODULE")) {
                            var paths = env["PSMODULE"];
                            var mod = proxy.GetVariable("PSModule");
                            proxy.SetVariable("PSModule", $"{paths};{mod}");
                        }
                       
                        if(this.options.AppendPath && env.Contains("PATHS")) {
                            var paths = env["PATHS"];
                            var mod = proxy.GetVariable("Path");
                            proxy.SetVariable("Path", $"{paths};{mod}");
                        }
                    }
                }

                using (var pipeline = runspace.CreatePipeline())
                {
                    pipeline.Output.DataReady += (sender, args) =>
                    {
                        PipelineReader<PSObject> reader = sender as PipelineReader<PSObject>;

                        if (reader != null)
                        {
                            while (reader.Count > 0)
                            {
                                host.UI.WriteLine(reader.Read().ToString());
                            }
                        }
                    };

                   
                    pipeline.Error.DataReady += (sender, args) =>
                    {
                        PipelineReader<object> reader = sender as PipelineReader<object>;

                        if (reader != null)
                        {
                            while (reader.Count > 0)
                            {
                                host.UI.WriteErrorLine(reader.Read().ToString());
                            }
                        }
                    };

                    try
                    {
                        pipeline.Invoke();
                    }
                    catch (RuntimeException ex)
                    {
                        var errorStackTrace = ex.StackTrace;
                        var record = ex.ErrorRecord;
                        if (record != null)
                        {
                            // not available in v1
                            //errorStackTrace = record.ScriptStackTrace;
                            var scriptStackTrace = record.GetType().GetProperty("ScriptStackTrace");
                            if (scriptStackTrace != null)
                            {
                                var scriptError = scriptStackTrace.GetValue(record, null).ToString();
                                if (!string.IsNullOrWhiteSpace(scriptError)) 
                                    errorStackTrace = scriptError;
                            }
                        }
                        var msg = ex.Message;
                        msg += Environment.NewLine + "[Stack]:" + errorStackTrace;
                        this.host.UI.WriteErrorLine(msg);
                    }
                    catch (Exception ex)
                    {
                        var msg = ex.Message;
                        msg += Environment.NewLine + "[Stack]:" + ex.StackTrace.ToString();

                        this.host.UI.WriteErrorLine(msg);
                    }
                   
                    
                    if(host is AppHost)
                    {
                        appHost = (AppHost)host;
                        exitCode = appHost.ExitCode;
                    }

                  

                    if (pipeline.PipelineStateInfo != null)
                    {
                        switch (pipeline.PipelineStateInfo.State)
                        {
                            // disconnected is not available unless the assembly version is at least v3
                            //case PipelineState.Disconnected:
                            case PipelineState.Running:
                            case PipelineState.NotStarted:
                            case PipelineState.Failed:
                            case PipelineState.Stopping:
                            case PipelineState.Stopped:
                                if(appHost != null)
                                {
                                    exitCode = appHost.ExitCode;
                                    if(exitCode == 0 || exitCode == -1)
                                        appHost.SetShouldExit(1);

                                    appHost.LastException = pipeline.PipelineStateInfo.Reason;
                                } else {
                                    host.SetShouldExit(1);
                                }
                                
                                break;
                            case PipelineState.Completed:
                               if(appHost != null)
                                {
                                    exitCode = appHost.ExitCode;
                                    if(exitCode == -1)
                                    {
                                        exitCode = 0;
                                        appHost.SetShouldExit(0);
                                    }
                                        
                                }
                                break;
                        }

                    }
                }
            }

            

            var result =  new PowerShellResult() {
                ExitCode = exitCode,
                LastException = LastException,
                HasStandardErrorData = appHost == null ? false : appHost.HasStandardErrorData
            };

            this.logger?.LogDebug("PowerShell @result", result);

            

            return result;
        }
    }
}