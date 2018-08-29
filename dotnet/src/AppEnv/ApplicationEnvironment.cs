using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using NerdyMishka.Reflection;

namespace NerdyMishka
{
    public class ApplicationEnvironment : IApplicationEnvironment
    {
        private ConcurrentDictionary<string, object> meta = new ConcurrentDictionary<string, object>();

        public ApplicationEnvironment()
        {
            this.BasePath = Environment.CurrentDirectory;
        }

        public ApplicationEnvironment(string basePath)
        {
            this.BasePath = basePath;
        }

        public ApplicationEnvironment(Type entryType, int directoriesBack = 0)
            :this(entryType.Assembly, directoriesBack)
        {

        }

        public ApplicationEnvironment(Assembly assembly, int directoriesBack = 0)
        {
            var dir = assembly.GetDirectoryName();

            for (var i = 0; i < directoriesBack; i++)
                dir = System.IO.Path.Combine(dir, "..");

            this.BasePath = System.IO.Path.GetFullPath(dir);
        }


        public string ApplicationName { get; set; }
        public string EnvironmentName { get; set; }
        public string BasePath { get; protected set; }

        public object this[string key]
        {
            get {

                if (this.meta.TryGetValue(key, out object value))
                    return value;

                return null;
                    
            }
            set
            {
                this.meta.AddOrUpdate(key, value, (k,v) => v);
            }
        }

        public string ResolvePath(string relativePath, params string[] segments)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return relativePath;

            char second = relativePath[1],
                   first = relativePath[0];
            
            if ((second == '/' || second == '\\') && (first == '.' || first == '~'))
            {
                relativePath = System.IO.Path.Combine(this.BasePath, relativePath.Substring(2));
            }

            if (segments == null || segments.Length == 0)
                return System.IO.Path.GetFullPath(relativePath);

            foreach(var segment in segments)
            {
                relativePath = System.IO.Path.Combine(relativePath, segment);
            }

            return System.IO.Path.GetFullPath(relativePath);
        }
    }
}
