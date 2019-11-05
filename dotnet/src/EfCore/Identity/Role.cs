using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NerdyMishka.EfCore.Identity
{
    public class Role
    {
        public Role()
        {
        
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Code { get; set; }

        public string Name  { get; set; }

        public string ClaimType { get; set; }

        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public Claim ToClaim()
        {
            return new Claim(this.ClaimType ?? ClaimTypes.Role, this.Code);
        }

        public IEnumerable<User> Users 
        {
            get => this.UserRoles?.Select(o => o.User) ?? Enumerable.Empty<User>();
        }
    }
}