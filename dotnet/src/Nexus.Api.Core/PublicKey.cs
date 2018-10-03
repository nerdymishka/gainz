namespace Nexus.Api
{
    public class PublicKey
    {
        public int? Id  { get; set; }

        public string Blob { get; set; }

        public string UriPath { get; set; }

        public int? UserId { get; set; }

        public string Username { get; set; }
    }
}