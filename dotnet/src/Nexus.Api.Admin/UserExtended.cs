namespace Nexus.Api.Admin
{
    public class UserExtended : User
    {
        public bool IsAdmin { get; set; }

        public bool IsBanned { get; set; }
    }
}