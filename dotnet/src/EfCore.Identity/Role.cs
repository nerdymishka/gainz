using System;

namespace NerdyMishka.EfCore.Identity
{
    public class Role
    {
        public Role()
        {
            this.CreatedAt = DateTime.UtcNow;
            this.SyncKey = Guid.NewGuid();
        }

        public int Id { get; set; }

        public Guid SyncKey { get; set; }

        
        public string Code { get; set; }

        public string Name  { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt  { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}