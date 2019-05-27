using System;

namespace NerdyMishka.EfCore.Identity
{
    public class Domain
    {
        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        public string Name  { get; set; }

        public int? OrganizationId { get; set; }

        public Organization Organization { get; set; }
    }
}