using System;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Identity
{

    public class ApiKey
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Name { get; set; }
        
        [Hash]
        public byte[] Value  { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public IEnumerable<Role> Roles 
        { 
            get => this.ApiKeyRoles?.Select( o => o.Role) ?? Array.Empty<Role>(); 
        }

        public ICollection<ApiKeyRole> ApiKeyRoles { get; set; }

     
    }
}