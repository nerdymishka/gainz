using System;
using System.Collections.Generic;

namespace NerdyMishka.EfCore.Identity
{
    public class PasswordPolicy
    {
        public PasswordPolicy()
        {
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public virtual ICollection<User> Users { get; set; }

        public virtual ICollection<Organization> Organizations { get; set; }

        public Guid SyncKey { get; set; }

        public string Name  { get; set; }

        public PasswordComposition Composition { get; set; }

        public short Minimum { get; set; }

        public short? Maximum { get; set; }

        public short? LifetimeInDays { get; set; }

    }
}