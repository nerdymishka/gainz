using System;
using System.Collections.Generic;

namespace NerdyMishka.EfCore.Identity
{
    public class Organization
    {
        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual PasswordPolicy PasswordPolicy { get; set; }

        public int? PasswordPolicyId { get; set; } 

        public virtual ICollection<Domain> Domains { get; set; }
    }
}