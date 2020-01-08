using System;

namespace NerdyMishka.EfCore.Identity
{
    public class Domain
    {
        public Domain()
        {
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Name  { get; set; }

        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}