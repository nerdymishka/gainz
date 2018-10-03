using System;
using System.Text;

namespace Nexus.Api
{
    public class ConfigurationFile
    {

        public ConfigurationFile()
        {

        }

        public ConfigurationFile(
            string uriPath, 
            string blob, 
            string mimeType = "text/plain",
            string encoding = "utf-8") {

            if(uriPath == null)
                throw new ArgumentNullException(nameof(uriPath)); 
            this.UriPath= uriPath;
            this.SetContent(blob, mimeType, encoding);
         }

        public ConfigurationFile(
            string uriPath, 
            byte[] blob, 
            string mimeType = "text/plain",
            string encoding = "utf-8") {
            
             if(uriPath == null)
                throw new ArgumentNullException(nameof(uriPath)); 
            this.UriPath= uriPath;
            this.SetContent(blob, mimeType, encoding);
         }

        public int? Id { get; set; }  

        public string UriPath { get; set; }

        public string Base64Content { get; set; }

        public string Description { get; set; }

        public string MimeType { get; set; } = "text/plain";

        public string Encoding { get; set; } = "UTF-8";

        public bool? IsEncrypted { get; set; } = true;

        public bool? IsKeyExternal { get; set; }

        public bool? IsTemplate { get; set; }

        public int? ConfigurationSetId { get; set; }

        public string ConfigurationSetName { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }

        public void SetContentFromFile(
            string path, 
            string mimeType = null, 
            string encoding = "utf-8")
        {
            
            if(mimeType == null)
            {   
                // TODO: create a common method for this
                var ext = System.IO.Path.GetExtension(path);
                switch(ext)
                {
                    case ".json":
                        mimeType = "application/json";
                        break;
                    case ".cspkg":
                    case ".config":
                    case ".nuspec":
                    case ".xml":
                        mimeType = "application/xml";
                        break;
                    case ".yml":
                        mimeType = "application/x-yaml";
                        break;
                    case ".lzh":
                    case ".ovpn":
                        mimeType = "application/octet-stream";
                        break;
                    case ".gz":
                        mimeType = "application/x-gzip";
                        break;
                    case ".z":
                        mimeType = "application/x-compress";
                        break;
                    case ".tgz":
                        mimeType = "application/x-compressed";
                        break;
                    case ".tar":
                        mimeType = "application/x-tar";
                        break;
                    case ".bz2":
                        mimeType = "application/x-bzip2";
                        break;
                    case ".zip":
                        mimeType = "application/zip";
                        break;
                    case ".xlsx":
                        mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        break;
                    case ".csv":
                        mimeType = "application/x-csv";
                        break;
                    case ".p12":
                    case ".pfx":
                        mimeType = "application/x-pkcs12";
                        break;
                    case ".cer":
                    case ".der":
                    case ".crt":
                        mimeType = "application/x-x509-ca-cert";
                        break;
                    default:
                        mimeType = "text/plain";
                        break;
                }
            }
            this.MimeType = mimeType;
            this.Encoding = encoding;

            this.Base64Content = 
                Convert.ToBase64String(
                    System.IO.File.ReadAllBytes(path));
        }

        public void SetContent(string text, 
            string mime = "text/plain", 
            string encoding = "utf-8") {
            
            var enc = this.GetEncoding(encoding);

            this.SetContent(enc.GetBytes(text), mime, encoding);
        }

        public void SetContent(byte[] bytes, 
            string mime = "text/plain",
            string encoding = "utf-8") {

            this.Base64Content = Convert.ToBase64String(bytes);
        }

        private Encoding GetEncoding(string encoding = "utf-8")
        {
            string lowered = null;
            if(!string.IsNullOrWhiteSpace(encoding))
                lowered = encoding.ToLowerInvariant();
            
            switch(encoding)
            {
                case "ascii":
                    return System.Text.Encoding.ASCII;
                case "unicode":
                    return System.Text.Encoding.Unicode;
                case "utf-32":
                case "utf32":
                     return System.Text.Encoding.UTF32;
                case "utf-7":
                case "utf7":
                     return System.Text.Encoding.UTF7;
                case "utf-8":
                case "utf8":
                     return System.Text.Encoding.UTF8;
                default:
                    return System.Text.Encoding.UTF8;
            }
        }
    }
}