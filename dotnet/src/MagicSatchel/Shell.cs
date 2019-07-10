using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NerdyMishka
{
    public class Shell
    {

        public int Execute(
            string program, 
            IEnumerable<string> args = null,
            string workingDirectory = null,
            int? millisecondsToWait = null)
        {
            return Execute(program, (process) => {
                string arguments = null;
                if(args != null && args.Count() > 0) {
                    arguments = string.Join(" ", args);
                }

                if(!string.IsNullOrWhiteSpace(arguments)) {
                    process.StartInfo.Arguments = arguments;
                }

                if(!string.IsNullOrWhiteSpace(workingDirectory)) {
                    process.StartInfo.WorkingDirectory = workingDirectory;
                }
            }, millisecondsToWait);
        }


        public int Execute(
            string program, 
            Action<Process> modify = null,
            int? millisecondsToWait = null)
        {
            using(var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                if(process != null)
                    modify(process);

              
                process.Start();
           

                if(millisecondsToWait.HasValue)
                {
                    if(!process.WaitForExit(millisecondsToWait.Value))
                        return -1;

                    return process.ExitCode;
                }
                        
                    
                process.WaitForExit();
                return process.ExitCode;
            }
        }

         public Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args = null,
            string workingDirectory = null,
            int? millisecondsToWait = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteAsync(program, (process) => {
                    string arguments = null;
                    if(args != null && args.Count() > 0) {
                        arguments = string.Join(" ", args);
                    }

                    if(!string.IsNullOrWhiteSpace(arguments)) {
                        process.StartInfo.Arguments = arguments;
                    }

                    if(!string.IsNullOrWhiteSpace(workingDirectory)) {
                        process.StartInfo.WorkingDirectory = workingDirectory;
                    }
                },
                millisecondsToWait, 
                cancellationToken);
        }


        public Task<int> ExecuteAsync(
            string program, 
            Action<Process> modify,
            int? millisecondsToWait = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return new Task<int>(() => {

                using(var process = new Process())
                {
     
                    process.StartInfo = new ProcessStartInfo() {
                        FileName = program,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    if(process != null)
                        modify(process);

                
                    process.Start();

                    if(cancellationToken.IsCancellationRequested)
                    {
                        process.Kill();
                    }

                    if(millisecondsToWait.HasValue)
                    {
                        if(!process.WaitForExit(millisecondsToWait.Value))
                            return -1;

                        return process.ExitCode;
                    }
                        
                    
                    process.WaitForExit();
                    return process.ExitCode;
                }
            });
        }


        public Task<int> ExecuteAsync(
            string program, 
            string[] args = null,
            string workingDirectory = null,
            Action<Process> modify = null,
            int? millisecondsToWait = null, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return new Task<int>(() => {

                using(var process = new Process())
                {
                    string arguments = null;
                    if(args != null && args.Length > 0) {
                        arguments = string.Join(" ", args);
                    }

                    process.StartInfo = new ProcessStartInfo() {
                        FileName = program,
                        Arguments = arguments,
                        WorkingDirectory = workingDirectory,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    if(process != null)
                        modify(process);

                
                    process.Start();

                    if(cancellationToken.IsCancellationRequested)
                    {
                        process.Kill();
                    }

                    if(millisecondsToWait.HasValue)
                    {
                        if(!process.WaitForExit(millisecondsToWait.Value))
                            return -1;

                        return process.ExitCode;
                    }
                        
                    
                    process.WaitForExit();
                    return process.ExitCode;
                }
            });
        }

        public int Redirect(
            string program, 
            string[] args = null,
            string workingDirectory = null,
            TextWriter stdOut = null, 
            TextWriter stdError = null,
            int? millisecondsToWait = null)
        {
            using(var process = new Process())
            {
                string arguments = null;
                if(args != null && args.Length > 0) {
                    arguments = string.Join(" ", args);
                }

                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true 
                };

                process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdOut.WriteLine(e.Data);
                        stdOut.Flush();
                    }
                });

                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdError.WriteLine(e.Data);
                        stdError.Flush();
                    }
                });
            
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if(millisecondsToWait.HasValue)
                {
                    if(! process.WaitForExit(millisecondsToWait.Value))
                        return 1;

                    return process.ExitCode;
                }   
                
                process.WaitForExit();
                return process.ExitCode;
            }
        }


        public Task<int> RedirectAsync(
            string program, 
            Action<Process> modify,
            Action<string> stdWrite,
            Action<string> errorWrite,
            int? millisecondsToWait = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return new Task<int>(() => {
                using(var process = new Process())
                {
                

                    process.StartInfo = new ProcessStartInfo() {
                        FileName = program,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true 
                    };

                    if(modify != null)
                        modify(process);

                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdWrite(e.Data);
                        }
                    });

                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            errorWrite(e.Data);
                        }
                    });

                    if(cancellationToken.IsCancellationRequested)
                        return 1;
                
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if(millisecondsToWait.HasValue)
                    {
                        if(!process.WaitForExit(millisecondsToWait.Value))
                            return 1;

                        return process.ExitCode;
                    }
                   
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }, cancellationToken);
        }

        public Task<int> RedirecAsync(
            string program, 
            IEnumerable<string> args = null,
            TextWriter stdOut = null, 
            TextWriter stdError = null,
            string workingDirectory = null,
            int? millisecondsToWait = null,
            CancellationToken cancellationToken = default(CancellationToken)
        ) {
            return RedirectAsync(program, (process) => {
                    string arguments = null;
                    if(args != null && args.Count() > 0) {
                        arguments = string.Join(" ", args);
                    }

                    if(!string.IsNullOrWhiteSpace(arguments)) {
                        process.StartInfo.Arguments = arguments;
                    }

                    if(!string.IsNullOrWhiteSpace(workingDirectory)) {
                        process.StartInfo.WorkingDirectory = workingDirectory;
                    }

        
                }, 
                stdOut ?? Console.Out,
                stdError ?? Console.Error,
                millisecondsToWait,
                cancellationToken);
        }


         public Task<int> RedirectAsync(
            string program, 
            Action<Process> modify,
            TextWriter stdOut, 
            TextWriter stdError,
            int? millisecondsToWait = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
          

            return new Task<int>(() => {
                using(var process = new Process())
                {
                    
                    process.StartInfo = new ProcessStartInfo() {
                        FileName = program,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true 
                    };

                    if(modify != null)
                        modify(process);

                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdOut.WriteLine(e.Data);
                            stdOut.Flush();
                            
                        }
                    });

                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdError.WriteLine(e.Data);
                            stdError.Flush();
                           
                        }
                    });

                    if(cancellationToken.IsCancellationRequested)
                        return 1;
                
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if(millisecondsToWait.HasValue)
                    {
                        if(!process.WaitForExit(millisecondsToWait.Value))
                            return 1;

                        return process.ExitCode;
                    }

                    process.WaitForExit();
                    return process.ExitCode;
                }
            }, cancellationToken);
        }
    }
}