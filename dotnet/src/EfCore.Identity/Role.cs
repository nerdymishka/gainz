using System;
using System.Collections.Generic;
using System.Linq;

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

        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public IEnumerable<User> Users 
        {
            get => this.UserRoles?.Select(o => o.User) ?? Enumerable.Empty<User>();
        }
    }
}