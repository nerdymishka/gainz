
using System;
using System.IO;
using NerdyMishka;

namespace Kryptos
{

    public class Env 
    {
        private static IApplicationEnvironment AppEnv { get; set; }

        static Env()
        {
            AppEnv = new ApplicationEnvironment(typeof(Env).Assembly, 0);
        }

        public static string ResolveAppPath(string path, params string[] segments)
        {
            if(string.IsNullOrWhiteSpace(path))
                return path;

            return AppEnv.ResolvePath(path, segments);
        }

        public static string GetFullPath(string fileName)
        {
            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            
            return null;
        }
    }
}