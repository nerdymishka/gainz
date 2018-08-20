using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{


    public class KeePassAuditFields : IKeePassAuditFields
    {
        public KeePassAuditFields()
        {
            // TODO: figure out if KeePass uses UTC time.
            var now = DateTime.UtcNow;
            this.CreationTime = now;
            this.LastModificationTime = now;
            this.LastAccessTime = now;
            this.ExpiryTime = now;
            this.LocationChanged = now;
            this.Expires = false;
            this.UsageCount = 0;
        }

        public DateTime CreationTime { get; set; }

        public DateTime LastModificationTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public DateTime ExpiryTime { get; set; }

        public bool Expires { get; set; }

        public int UsageCount { get; set; }

        public DateTime LocationChanged { get; set; }
    }
}
