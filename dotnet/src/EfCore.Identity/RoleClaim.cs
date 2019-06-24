using System.Security.Claims;

namespace NerdyMishka.EfCore.Identity
{
    public class RoleClaim
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string Type { get; set; }

        public string Value { get; set; }

        public virtual Claim ToClaim()
        {
            return new Claim(this.Type, this.Value);
        }

        public virtual void InitializeFromClaim(Claim claim)
        {
            this.Type = claim.Type;
            this.Value = claim.Value;
        }
    }

}