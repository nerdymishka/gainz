using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace NerdyMishka.KeePass
{
    public static class Env
    {
        private static IApplicationEnvironment appEnv;
        
        static Env()
        {
            appEnv = new ApplicationEnvironment(typeof(Env), 3);
            appEnv.EnvironmentName = "Test";
            appEnv.ApplicationName = "NerdyMishka.Thory.Tests";
        }

        public static string ResolvePath(string path) => appEnv.ResolvePath(path);
    }
}
