

using System;
using System.IO;

namespace NerdyMishka.Extensions.Flex 
{
    public class FlexSerializationBuilder
    {
        public IFlexSerializationSettings Settings { get; private set;}

        private string baseUri = null;

        public FlexSerializationBuilder()
        {
            this.Settings = new FlexSerializationSettings();
        }


        public FlexSerializationBuilder SetSettings(IFlexSerializationSettings settings)
        {
            this.Settings = settings;
            return this;
        }

        public FlexSerializationBuilder UpdateSettings(Action<IMutableFlexSerializationSettings> update)
        {
            if(this.Settings is IMutableFlexSerializationSettings && update != null)
            {
                update((IMutableFlexSerializationSettings)this.Settings);
            }
            
            return this;
        }

        public FlexSerializationBuilder SetBaseUri(string baseUri)
        {
            this.baseUri = baseUri;
            return this;
        }

        

        public string ResolvePath(string path, params string[] segments)
        {
            if(string.IsNullOrWhiteSpace(path))
                return path;

            if(path.StartsWith("~/") || path.StartsWith("./"))
            {
                var baseUri = this.baseUri ?? System.Environment.CurrentDirectory;
                path = Path.Combine(baseUri, path.Substring(2));
            }

            if(segments != null && segments.Length > 0)
            {
                foreach(var segment in segments)
                    path = Path.Combine(path, segment);
            }

            return path;
        }
    }
}