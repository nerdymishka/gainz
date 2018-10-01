namespace Nexus.Api
{
    public class UserRegistration
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool? IsAdmin { get; set; }

        public string IconUri { get; set; }

        public bool? GeneratePassword { get; set; }

        public bool? GenerateApiKey { get; set; }

        public string Base64ApiKey { get; set; }
    }
}