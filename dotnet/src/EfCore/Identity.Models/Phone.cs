using System;
using NerdyMishka.ComponentModel.DataAnnotations;

namespace NerdyMishka.EfCore.Identity
{
    public class Phone
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }

        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        public Guid SyncKey { get; set; }  = Guid.NewGuid();

        public string Name { get; set;}
        
        [Encrypt]
        public string Value { get; set; }

        public PhonePurpose Purpose  { get; set; }
    }
}