using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{


    public class KeePassAuditFields : IKeePassAuditFields, IEquatable<IKeePassAuditFields>
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

        public bool Equals(IKeePassAuditFields other)
        {
            if(this.CreationTime != other.CreationTime)
                return false;
            
            if(this.LastModificationTime != other.LastModificationTime)
                return false;

            if(this.LastAccessTime != other.LastAccessTime)
                return false;

            if(this.ExpiryTime != other.ExpiryTime)
                return false;

            if(this.LocationChanged != other.LocationChanged)
                return false;

            if(this.Expires != other.Expires)
                return false;

            if(this.UsageCount != other.UsageCount)
                return false;

            return true;
        }
    }
}
