namespace NerdyMishka.EfCore.Identity
{
    public class UserOrganization
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int OrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}