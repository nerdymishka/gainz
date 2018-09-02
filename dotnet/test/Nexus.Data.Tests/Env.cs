using NerdyMishka;

public static class Env
    {
        private static IApplicationEnvironment appEnv;
        
        static Env()
        {
            appEnv = new ApplicationEnvironment(typeof(Env), 3);
            appEnv.EnvironmentName = "Test";
            appEnv.ApplicationName = "NerdyMishka.Nexus.Data.Tests";
        }

        public static string ResolvePath(string path) => appEnv.ResolvePath(path);
    }