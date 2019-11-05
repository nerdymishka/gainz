using System;
using System.Collections.Generic;

namespace NerdyMishka.EfCore.Identity
{
    public class MultiFactorPolicy
    {
        public int Id { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Organization> Organizations { get; set; }

        public Guid SyncKey { get; set; }

        public string Name  { get; set; }

        public string Settings { get; set; }

        public bool IsEmailRequired { get; set; }

        public bool IsSmsRequired { get; set; }

        public bool IsEnabled { get; set; }

    }
}