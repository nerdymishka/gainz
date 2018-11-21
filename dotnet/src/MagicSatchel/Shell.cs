using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka
{
    public class Shell
    {

        public int Execute(
            string program, 
            string[] args = null,
            string workingDirectory = null,
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
                };

              
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
            string[] args = null,
            string workingDirectory = null,
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
            string[] args = null,
            string workingDirectory = null,
            TextWriter stdOut = null, 
            TextWriter stdError = null,
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
                        RedirectStandardOutput = true,
                        RedirectStandardError = true 
                    };

                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdOut.WriteLine(e.Data);
                            stdOut.Flush();
                            if(cancellationToken.IsCancellationRequested) {
                                process.Kill();
                            }
                        }
                    });

                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdError.WriteLine(e.Data);
                            stdError.Flush();
                            if(cancellationToken.IsCancellationRequested) {
                                process.Kill();
                            }
                        }
                    });

                    if(cancellationToken.IsCancellationRequested)
                        return 1;
                
                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    
                    if(cancellationToken.IsCancellationRequested)
                    {
                        process.Kill();
                        process.WaitForExit();
                        return 1;
                    }
                      

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