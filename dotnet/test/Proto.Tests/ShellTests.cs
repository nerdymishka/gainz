using Xunit;
using System;
using NerdyMishka;


namespace Tests 
{
    [Integration]
    [Trait("tag", "integration")]
    public class ShellTests
    {
      
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