namespace NerdyMishka.EfCore.Identity
{
    public class OrganizationUser
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}