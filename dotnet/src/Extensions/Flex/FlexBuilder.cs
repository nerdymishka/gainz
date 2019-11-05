using System.Collections.Generic;
using System.IO;

namespace NerdyMishka.Flex 
{
    public class FlexBuilder
    {
        public FlexBuilder()
        {
            this.DateTimeFormats = new List<FlexDateTimeOptions>();
            this.NamingConvention = new CamelCaseNamingConvention();
        }

        public static string BaseUri {get; set;}

        public FileStream ReadStream {get; set;}

        public FileStream WriteStream { get; set; }

        public IFlexCryptoProvider FlexCryptoProvider { get; private set; }  

        protected IList<FlexDateTimeOptions> DateTimeFormats {get; private set; }

        protected INamingConvention NamingConvention { get; private set; }

        protected bool OmitNulls { get; private set; }

        public FlexSettings Build()
        {
            return new FlexSettings() {
                CryptoProvider = this.FlexCryptoProvider,
                DateTimeOptions = DateTimeFormats,
                NamingConvention = NamingConvention,
                OmitNulls= this.OmitNulls,
            };
        }

        public FlexBuilder SetOmitNull(bool shouldOmitNulls)
        {
            this.OmitNulls = shouldOmitNulls;
            return this;
        }

        public FlexBuilder SetCryptoProvider(IFlexCryptoProvider provider)
        {
            this.FlexCryptoProvider = provider;
            return this;
        }

        public FlexBuilder AddDateTimeFormat(
            string format, 
            string name = "Default", 
            string provider = null,
            bool isUtc = true)
        {
            this.DateTimeFormats.Add(new FlexDateTimeOptions() {
                 Name = name,
                 Format = format,
                 Provider = provider,
                 IsUtc = isUtc
            });
            return this;
        }
        
    

        public FlexBuilder OpenReadStream(string path)
        {
            path = this.ResolvePath(path);
            this.ReadStream = File.OpenRead(path);
            return this;
        }

        public FlexBuilder OpenWriteStream(string path)
        {
            path = this.ResolvePath(path);
            this.WriteStream = File.OpenWrite(path);
            return this;
        }

        public string ResolvePath(string path, params string[] segments)
        {
            if(string.IsNullOrWhiteSpace(path))
                return path;

            if(path.StartsWith("~/") || path.StartsWith("./"))
            {
                var baseUri = BaseUri ?? System.Environment.CurrentDirectory;
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