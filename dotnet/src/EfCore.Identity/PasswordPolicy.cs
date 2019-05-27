using System;

namespace NerdyMishka.EfCore.Identity
{
    public class PasswordPolicy
    {
        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Name  { get; set; }

        public PasswordComposition Composition { get; set; }

        public short Minimum { get; set; }

        public short? Maximum { get; set; }

        public short? LifetimeInDays { get; set; }

    }
}