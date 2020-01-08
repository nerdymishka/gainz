using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.EfCore.Identity
{
    public class Organization
    {
        public Organization()
        {
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public int? MultiFactorPolicyId { get; set; }
        
        public int? PasswordPolicyId { get; set; } 

        public virtual MultiFactorPolicy MultiFactorPolicy { get; set; }

        public virtual PasswordPolicy PasswordPolicy { get; set; }

        public virtual ICollection<Domain> Domains { get; set; }

        public virtual ICollection<User> Users { get; set;}
    }
}