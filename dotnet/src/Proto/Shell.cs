using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using NerdyMishka.Validation;

namespace NerdyMishka
{
    public static class Shell
    {

        public static int Execute(
            string program)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, null, null, null, null, null, null);
        }

        public static int Execute(
            string program, 
            string arguments)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            var args = ConvertArgsToArray(arguments);

            return Execute(program, args, null, null, null, null, null);
        }

        public static int Execute(
            string program, 
            IEnumerable<string> args,
            TextWriter stdOut,
            TextWriter stdError)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, null, null, stdOut, stdError, null);
        }
       
         public static int Execute(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            TextWriter stdOut,
            TextWriter stdError)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, workingDirectory, null, stdOut, stdError, null);
        }


        public static int Execute(
            string program, 
            Action<Process> modify,
            int millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, null, null, modify, null, null, millisecondsToWait);
        }


         public static int Execute(
            string program, 
            IEnumerable<string> args,
            string workingDirectory)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, workingDirectory, null, null, null, null);
        }


        public static int Execute(
            string program, 
            IEnumerable<string> args)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, null, null, null, null, null);
        }

        
        public static int Execute(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            int millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, workingDirectory, null, null, null, millisecondsToWait);
        }


        public static int Execute(
            string program, 
            IEnumerable<string> args,
            int millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return Execute(program, args, null, null, null, null, millisecondsToWait);
        }
        
         public static int Execute(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            Action<Process> modify,
            TextWriter stdOut, 
            TextWriter stdError, 
            int? millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            string arguments = null;
            if(args != null && args.Any()) {
                arguments = string.Join(" ", args);
            }

            if(!string.IsNullOrWhiteSpace(workingDirectory))
            {
                if(!Directory.Exists(workingDirectory))
                    throw new DirectoryNotFoundException($"Working Directory does not exist: {workingDirectory}");
            }

            using(var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = stdOut != null,
                    RedirectStandardError = stdError != null 
                };

                if(stdOut != null)
                {
                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdOut.WriteLine(e.Data);
                            stdOut.Flush();
                        }
                    });
                }

                if(stdError != null)
                {
                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdError.WriteLine(e.Data);
                            stdError.Flush();
                        }
                    });
                }       

                if(modify != null)
                    modify(process); 

            
                process.Start();
                if(stdOut != null)
                    process.BeginOutputReadLine();
                
                if(stdError != null)
                    process.BeginErrorReadLine();

                if(millisecondsToWait.HasValue)
                {
                    if(! process.WaitForExit(millisecondsToWait.Value))
                        return 1;
                } else {
                     process.WaitForExit();
                }  
                
               
                return process.ExitCode;
            }
        }

      
       

      

        public static Task<int> ExecuteAsync(
            string program, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, null, null, null, null, null, null, cancellationToken);
        }

         public static Task<int> ExecuteAsync(
            string program, 
            string arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            var args = ConvertArgsToArray(arguments);

            return ExecuteAsync(program, args, null, null, null, null, null, cancellationToken);
        }


        public static Task<int> ExecuteAsync(
            string program, 
            string arguments,
            TextWriter stdOut,
            TextWriter stdError,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var args = ConvertArgsToArray(arguments);
            
            return ExecuteAsync(program, args, null, null, stdOut, stdError, null, cancellationToken);
        }
       

        public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            TextWriter stdOut,
            TextWriter stdError,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, null, null, stdOut, stdError, null, cancellationToken);
        }
       
         public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            TextWriter stdOut,
            TextWriter stdError,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, workingDirectory, null, stdOut, stdError, null, cancellationToken);
        }


        public static Task<int> ExecuteAsync(
            string program, 
            Action<Process> modify,
            int millisecondsToWait, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, null, null, modify, null, null, millisecondsToWait, cancellationToken);
        }


         public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, workingDirectory, null, null, null, null, cancellationToken);
        }


        public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, null, null, null, null, null, cancellationToken);
        }

        
        public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            int millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, workingDirectory, null, null, null, millisecondsToWait, cancellationToken);
        }


        public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            int millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAsync(program, args, null, null, null, null, millisecondsToWait, cancellationToken);
        }

         public static Task<int> ExecuteAsync(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            Action<Process> modify,
            TextWriter stdOut, 
            TextWriter stdError,
            int? millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            cancellationToken.ThrowIfCancellationRequested();
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            string arguments = null;
            if(args != null && args.Any()) {
                arguments = string.Join(" ", args);
            }

            if(!string.IsNullOrWhiteSpace(workingDirectory))
            {
                if(!Directory.Exists(workingDirectory))
                    throw new DirectoryNotFoundException($"Working Directory does not exist: {workingDirectory}");
            }
            
            using(var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = stdOut != null,
                    RedirectStandardError = stdError != null 
                };

                if(stdOut != null)
                {
                    process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdOut.WriteLine(e.Data);
                            stdOut.Flush();
                        }
                    });
                }

                if(stdError != null)
                {
                    process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdError.WriteLine(e.Data);
                            stdError.Flush();
                        }
                    });
                }       

                if(modify != null)
                    modify(process);

                process.Start();
                if(stdOut != null)
                    process.BeginOutputReadLine();

                if(stdError != null)
                    process.BeginErrorReadLine();

                if(millisecondsToWait.HasValue)
                {
                    if(!process.WaitForExit(millisecondsToWait.Value))
                    {
                        tcs.TrySetResult(-1);
                        return tcs.Task;
                    }
                } else {
                     process.WaitForExit();
                }

                tcs.TrySetResult(process.ExitCode);
                return tcs.Task;
            }
        }


        public static ShellResult ExecuteAndReturn(
            string program)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturn(program, null, null, null, null);
        }

        public static ShellResult ExecuteAndReturn(
            string program, 
            string arguments)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            var args = ConvertArgsToArray(arguments);

            return ExecuteAndReturn(program, args, null, null, null);
        }

        public static ShellResult ExecuteAndReturn(
            string program, 
            IEnumerable<string> arguments)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturn(program, arguments, null, null, null);
        }

        public static ShellResult ExecuteAndReturn(
            string program, 
            IEnumerable<string> arguments,
            string workingDirectory)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturn(program, arguments, workingDirectory, null, null);
        }


        public static ShellResult ExecuteAndReturn(
            string program, 
            string arguments,
            int millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            var args = ConvertArgsToArray(arguments);

            return ExecuteAndReturn(program, args, null, null, millisecondsToWait);
        }

         public static ShellResult ExecuteAndReturn(
            string program, 
            int millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturn(program, null, null, null, millisecondsToWait);
        }

        

        public static ShellResult ExecuteAndReturn(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            Action<Process> modify,
            int? millisecondsToWait)
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            string arguments = null;
            if(args != null && args.Any()) {
                arguments = string.Join(" ", args);
            }

            if(!string.IsNullOrWhiteSpace(workingDirectory))
            {
                if(!Directory.Exists(workingDirectory))
                    throw new DirectoryNotFoundException($"Working Directory does not exist: {workingDirectory}");
            }
                
            using(var process = new Process())
            {
                var stdOut = new List<string>();
                var stdError = new List<string>();
                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
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
                        stdOut.Add(e.Data);
                    }
                });

                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdError.Add(e.Data);
                    }
                });

                var result = new ShellResult() {
                    StdError = stdError,
                    StdOut = stdOut
                };   
            
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if(millisecondsToWait.HasValue)
                {
                    if(!process.WaitForExit(millisecondsToWait.Value))
                    {
                        result.ExitCode = -1;
                        result.TimeoutExpired = true;
                        result.Timeout = millisecondsToWait.Value;

                        return result;
                    }

                } else {
                    process.WaitForExit();
                }

         
                result.ExitCode = process.ExitCode;
                return result;
            }        
        }


        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturnAsync(program, null, null, null, null, cancellationToken);
        }

        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            string arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            var args = ConvertArgsToArray(arguments);

            return ExecuteAndReturnAsync(program, args, null, null, null, cancellationToken);
        }

        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            IEnumerable<string> arguments,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturnAsync(program, arguments, null, null, null, cancellationToken);
        }

        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            IEnumerable<string> arguments,
            string workingDirectory,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            return ExecuteAndReturnAsync(program, arguments, workingDirectory, null, null, cancellationToken);
        }


        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            string arguments,
            int millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            var args = ConvertArgsToArray(arguments);

            return ExecuteAndReturnAsync(program, args, null, null, millisecondsToWait, cancellationToken);
        }

         public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            int millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);
            return ExecuteAndReturnAsync(program, null, null, null, millisecondsToWait, cancellationToken);
        }


        public static Task<ShellResult> ExecuteAndReturnAsync(
            string program, 
            IEnumerable<string> args,
            string workingDirectory,
            Action<Process> modify,
            int? millisecondsToWait,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNullOrWhiteSpace(nameof(program), program);

            TaskCompletionSource<ShellResult> tcs = new TaskCompletionSource<ShellResult>();    
            string arguments = null;
            if(args != null && args.Any()) {
                arguments = string.Join(" ", args);
            }

            if(!string.IsNullOrWhiteSpace(workingDirectory))
            {
                if(!Directory.Exists(workingDirectory))
                    throw new DirectoryNotFoundException($"Working Directory does not exist: {workingDirectory}");
            }


            using(var process = new Process())
            {
                var stdOut = new List<string>();
                var stdError = new List<string>();
                
                process.StartInfo = new ProcessStartInfo() {
                    FileName = program,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
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
                        stdOut.Add(e.Data.ToString());
                        Console.WriteLine(e.Data.ToString());
                    }
                });

                process.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        stdError.Add(e.Data.ToString());
                    }
                });

                var result = new ShellResult() {
                    StdError = stdError,
                    StdOut = stdOut
                };

                if(cancellationToken.IsCancellationRequested)
                {
                    result.ExitCode = -1;
                }
            
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if(millisecondsToWait.HasValue)
                {
                    if(!process.WaitForExit(millisecondsToWait.Value))
                    {
                        
                        result.ExitCode = -1;
                        result.TimeoutExpired = true;
                        result.Timeout = millisecondsToWait.Value;
    
                        tcs.TrySetResult(result);
                    } 
                } else {
                    process.WaitForExit();
                }
        
                result.ExitCode = process.ExitCode;

                tcs.TrySetResult(result);
            }

            return tcs.Task;
        }


        private static string[] ConvertArgsToArray(string arguments)
        {
            var args = Array.Empty<string>();
            if(!string.IsNullOrWhiteSpace(arguments))
                args = new string[] { arguments };

            return args;
        }
    }
}