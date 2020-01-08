using System;
using NerdyMishka.ComponentModel.DataAnnotations;

namespace NerdyMishka.EfCore.Identity
{
    public class EmailAddress
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public virtual User User { get; set; }

        public Guid SyncKey { get; set; } = Guid.NewGuid();

        public string Name { get; set;}
        
        [Encrypt]
        public string Value { get; set; }

        public EmailPurpose Purpose  { get; set; }
    }
}