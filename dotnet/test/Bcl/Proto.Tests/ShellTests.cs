using Xunit;
using System;
using NerdyMishka;
using NerdyMishka.Validation;

namespace Tests 
{
    [Integration]
    [Trait("tag", "integration")]
    public class ShellTests
    {

        [Fact]
        public void Execute_ThrowsArgumentNullOrWhiteSpaceException()
        {
            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ");
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ", Array.Empty<string>());
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ", Array.Empty<string>(), null, null);
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ", Array.Empty<string>(), string.Empty);
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ", Array.Empty<string>(), string.Empty, null, null);
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.Execute("  ", (p) => {}, 500);
            });



            // ExecuteAndReturn
            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn("  ");
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn("  ", Array.Empty<string>());
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn("  ", Array.Empty<string>(), string.Empty);
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn("  ", Array.Empty<string>(), string.Empty, (p) => { }, null);
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn("  ", "  ");
            });

            Assert.Throws<ArgumentNullOrWhiteSpaceException>(() => {
                Shell.ExecuteAndReturn(" ", 30);
            });
            
        }

        [Fact]
        public void Execute_Simple()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.Execute("ipconfig.exe");
                    Assert.Equal(0, r1);
                    break;
                case PlatformID.Unix:
                    var r2 = Shell.Execute("ifconfig");
                    Assert.Equal(0, r2);
                    break;
            }
        }

        [Fact]
        public void Execute_WithArgs()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.Execute("cmd", new [] { "/c", "dir" });
                    Assert.Equal(0, r1);
                    break;
                case PlatformID.Unix:
                    var r2 = Shell.Execute("bash", new [] { "-c", "ls" });
                    Assert.Equal(0, r2);
                    break;
            }
        }

        [Fact]
        public void Execute_WithDirectory()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.Execute("cmd", new [] { "/c", "dir" }, Environment.CurrentDirectory);
                    Assert.Equal(0, r1);
                    break;
                case PlatformID.Unix:
                    var r2 = Shell.Execute("bash", new [] { "-c", "ls" }, Environment.CurrentDirectory);
                    Assert.Equal(0, r2);
                    break;
            }
        }

        [Fact]
        public void Execute_WithRedirection()
        {
            using(var stdOut = new System.IO.StringWriter())
            using(var stdErr = new System.IO.StringWriter())
            {
                switch(Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        {
                            var r1 = Shell.Execute("cmd", new [] { "/c", "dir" }, stdOut, stdErr);
                            var c1 = stdOut.ToString();
                            var e1 = stdErr.ToString();
                            Assert.NotEmpty(c1);
                            Assert.Empty(e1);
                            Assert.Equal(0, r1);
                        }
                        break;
                    case PlatformID.Unix:
                        {
                            var r1 = Shell.Execute("bash", new [] { "-c", "ls" }, stdOut, stdErr);
                            var c1 = stdOut.ToString();
                            var e1 = stdErr.ToString();
                            Assert.NotEmpty(c1);
                            Assert.Empty(e1);
                            Assert.Equal(0, r1);
                        }
                    
                        break;
                }
            }
        }
      
        [Fact]
        public void ExecuteAndReturn_Simple()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.ExecuteAndReturn("ipconfig.exe");
                    Assert.NotNull(r1);
                    Assert.NotNull(r1.StdOut);
                    Assert.Contains("Windows IP Configuration", r1.StdOut);
                    Assert.NotNull(r1.StdError);
                    Assert.Equal(0, r1.ExitCode);
                    break;
                case PlatformID.Unix:
                    var r2 = Shell.ExecuteAndReturn("ifconfig");
                    Assert.NotNull(r2);
                    Assert.NotNull(r2.StdOut);
                    Assert.NotNull(r2.StdError);
                    Assert.Equal(0, r2.ExitCode);
                    break;
            }
        }

        [Fact]
        public void ExecuteAndReturn_WithArgs()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.ExecuteAndReturn("net", "user /?");
                    Assert.NotNull(r1);
                    Assert.NotNull(r1.StdOut);
                    Console.WriteLine(r1.ToString());
                    Assert.NotNull(r1.StdError);
                    Assert.Contains("The syntax of this command is:", r1.StdError);
               
                    Assert.Equal(1, r1.ExitCode);
                    break;
                case PlatformID.Unix:
                    throw new NotImplementedException();
                    //break;
            }
        }



        [Fact]
        public void ExecuteAndReturn_WithWorkingDirectory()
        {
            switch(Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    var r1 = Shell.ExecuteAndReturn("cmd", new [] { "/c", "dir" }, Environment.CurrentDirectory);
                    Assert.NotNull(r1);
                    Assert.NotNull(r1.StdOut);
                    Console.WriteLine(r1.ToString());
                    Assert.NotNull(r1.StdError);
                    Assert.Equal(0, r1.ExitCode);
                    break;
                case PlatformID.Unix:
                    var r2 = Shell.ExecuteAndReturn("bash", new [] { "-c", "ls" }, Environment.CurrentDirectory);
                    Assert.NotNull(r2);
                    Assert.NotNull(r2.StdOut);
                    Console.WriteLine(r2.ToString());
                    Assert.NotNull(r2.StdError);
                    Assert.Equal(0, r2.ExitCode);
                    break;
            }
        }
    }
}