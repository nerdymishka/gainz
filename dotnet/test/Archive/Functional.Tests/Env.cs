using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NerdyMishka
{    
    public static class Env
    {
        public static readonly bool IsWindows = System.Environment.GetEnvironmentVariable("OS") == "Windows_NT";

        public static readonly bool IsLocalDb = true;

        private static IApplicationEnvironment appEnv;
        
        static Env()
        {
            appEnv = new ApplicationEnvironment(typeof(Env), 3);
            appEnv.EnvironmentName = "Test";
            appEnv.ApplicationName = "NerdyMishka.Functional.Tests";
        
        }

        public static string ResolvePath(string path) => appEnv.ResolvePath(path);
    }
}