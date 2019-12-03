using System;

namespace NerdyMishka.KeePass
{
    public interface IKeePassAuditFields
    {
         DateTime CreationTime { get; set; }

         DateTime LastModificationTime { get; set; }

         DateTime LastAccessTime { get; set; }

         DateTime ExpiryTime { get; set; }

         bool Expires { get; set; }

         int UsageCount { get; set; }

         DateTime LocationChanged { get; set; }
    }
}