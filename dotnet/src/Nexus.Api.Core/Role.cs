namespace Nexus.Api
{
    public class Role
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string UriPath { get; set; }
        public string Description  { get; set; }

        public long? ResourceId {get; set; }
    }
}