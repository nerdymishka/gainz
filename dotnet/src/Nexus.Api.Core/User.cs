namespace Nexus.Api
{
    public class User
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; } 

        public long? ResourceId { get; set; }

        public string IconUri { get; set; }
    }
}