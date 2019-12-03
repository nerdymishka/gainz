
using NerdyMishka;

namespace NerdyMishka.Flex.Yaml.Tests
{
    public static class Env
    {
        private static IApplicationEnvironment appEnv;
        
        static Env()
        {
            appEnv = new ApplicationEnvironment(typeof(Env), 3);
            appEnv.EnvironmentName = "Test";
            appEnv.ApplicationName = "NerdyMishka.Flex.Tests";
        }

        public static string ResolvePath(string path) => appEnv.ResolvePath(path);
    }
}