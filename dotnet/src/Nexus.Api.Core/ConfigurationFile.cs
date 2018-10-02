namespace Nexus.Api
{
    public class ConfigurationFile
    {
        public int? Id { get; set; }  

        public string UriPath { get; set; }

        public string Base64Content { get; set; }

        public string Description { get; set; }

        public string MimeType { get; set; } = "text/plain";

        public string Encoding { get; set; } = "UTF-8";

        public bool? IsEncrypted { get; set; }

        public bool? IsKeyExternal { get; set; }

        public bool? IsTemplate { get; set; }

        public int? ConfigurationSetId { get; set; }

        public string ConfigurationSetName { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }
    }
}