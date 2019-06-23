using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NerdyMishka.EfCore.Identity
{
    public class Permission
    {
        public Permission()
        {
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Code { get; set; }

        public string Name  { get; set; }

   

        public string Description { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimTypes.Permission, this.Code);
        }

        public IEnumerable<Role> Roles 
        {
            get => this.RolePermissions?.Select(o => o.Role) ?? Enumerable.Empty<Role>();
        }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}