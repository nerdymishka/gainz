using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassGroup : IKeePassGroup
    {
        private IList<IKeePassEntry> entries;
        private IList<IKeePassGroup> groups;

        public KeePassGroup()
        {
            var empty = new byte[16];

            this.AuditFields = new KeePassAuditFields();
            this.Uuid = Guid.NewGuid().ToByteArray();
            this.LastTopVisibleEntry = empty;
        }

        public byte[] Uuid { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public int IconId { get; set; }

        public byte[] CustomIconUuid { get; set; }

        public IKeePassAuditFields AuditFields { get; set; }

        public bool IsExpanded { get; set; }

        public string DefaultAutoTypeSequence { get; set; }

        public bool? EnableAutoType { get; set; }

        public bool? EnableSearching { get; set; }

        public byte[] LastTopVisibleEntry { get; set; }

        public IKeePassPackage Owner { get; internal protected set; }

        public IEnumerable<IKeePassEntry> Entries
        {
            get
            {
                if (this.entries == null)
                    this.entries = new List<IKeePassEntry>();
                return this.entries;
            }
        }


        public IEnumerable<IKeePassGroup> Groups
        {
            get
            {
                if (this.groups == null)
                    this.groups = new List<IKeePassGroup>();
                return this.groups;
            }
        }

        public IKeePassGroup Group(int index)
        {
            if (this.groups == null)
                return null;

            if (index < 0 || index > this.groups.Count)
                return null;

            return this.groups[0];
        }

        public IKeePassGroup Group(string name)
        {
            if (this.groups == null)
                return null;

            var lowered = name.ToLowerInvariant();
            foreach (var group in this.groups)
                if (group.Name.ToLowerInvariant() == lowered)
                    return group;

            return null;
        }

        public IKeePassEntry Entry(string name)
        {
            if (this.entries == null)
                return null;

            var lowered = name.ToLowerInvariant();
            foreach (var entry in this.entries)
                if (entry.Name.ToLowerInvariant() == lowered)
                    return entry;

            return null;
        }

        public IKeePassEntry Entry(int index)
        {
            if (this.entries == null)
                return null;

            if (index < 0 || index > this.entries.Count)
                return null;

            return this.entries[0];
        }

   

        public void Add(IKeePassGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            var one = this.Groups.FirstOrDefault(o => o.Uuid.EqualTo(group.Uuid));
            if (one == null)
            {
                this.groups.Add(group);
                if (group is KeePassGroup)
                    ((KeePassGroup)group).Owner = this.Owner;
                
            }
        }

        public void Add(IKeePassEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            var one = this.Entries.FirstOrDefault(o => o.Uuid.EqualTo(entry.Uuid));
            if (one == null)
            {
                this.entries.Add(entry);
                if (entry is KeePassEntry)
                    ((KeePassEntry)entry).Owner = this.Owner;
            }  
        }

        public void Remove(IKeePassGroup group)
        {
            if (this.groups == null)
                return;

            if (this.groups.Remove(group))
            {
                if (group is KeePassGroup)
                    ((KeePassGroup)group).Owner = null;
            }

        }

        public void Remove(IKeePassEntry entry)
        {
            if (this.entries == null)
                return;

            if (this.entries.Remove(entry))
            {
                if (entry is KeePassEntry)
                    ((KeePassEntry)entry).Owner = null;
            }
        }

        public void ExportTo(IKeePassGroup destination)
        {
            var source = this;
            var rootGroup = new KeePassGroup() { };
            source.CopyTo(rootGroup);

            destination.Add(rootGroup);

            source.MergeTo(rootGroup, true);
        }

        public void MergeTo(IKeePassGroup destination, bool overwrite = false, bool entriesOnly = false)
        {
            var source = this;
            var destinationPackage = destination.Owner;
            var sourcePackage = this.Owner;


            var entries = source.Entries.ToList();
            foreach (var entry in entries)
            {
                var next = destination.Entries.FirstOrDefault(o => o.Uuid.EqualTo(source.Uuid));
                if (next == null)
                {
                    next = destination.Entries.FirstOrDefault(o => o.Name.ToLowerInvariant() == entry.Name.ToLowerInvariant());
                }

                if (next != null && !overwrite)
                {
                    continue;
                }

                if (next == null)
                {
                    var destinationEntry = new KeePassEntry(true);
                    destinationEntry.Owner = destinationPackage;
                    destination.Add(destinationEntry);
                    next = destinationEntry;
                }

                entry.CopyTo(next);
            }

            if (entriesOnly)
                return;

            var groups = source.Groups.ToList();
            foreach (var group in groups)
            {
                var next = destination.Groups.SingleOrDefault(o => o.Uuid.EqualTo(group.Uuid));
                if (next == null)
                {
                    next = destination.Groups.FirstOrDefault(o => o.Name.ToLowerInvariant() == group.Name.ToLowerInvariant());
                }

                if (next != null && !overwrite)
                    continue;

                if (next == null)
                {
                    next = new KeePassGroup() { Owner = destination.Owner };
                    destination.Add(next);
                }

                group.CopyTo(next);

                group.MergeTo(next, overwrite, entriesOnly);
            }
        }

        public IKeePassGroup CopyTo(IKeePassGroup destinationGroup)
        {
            var sourceGroup = this;

            
            destinationGroup.Uuid = sourceGroup.Uuid;
            destinationGroup.AuditFields.CreationTime = sourceGroup.AuditFields.CreationTime;
            destinationGroup.AuditFields.Expires = sourceGroup.AuditFields.Expires;
            destinationGroup.AuditFields.ExpiryTime = sourceGroup.AuditFields.ExpiryTime;
            destinationGroup.AuditFields.LastAccessTime = sourceGroup.AuditFields.LastAccessTime;
            destinationGroup.AuditFields.LastModificationTime = sourceGroup.AuditFields.LastModificationTime;
            destinationGroup.AuditFields.LocationChanged = sourceGroup.AuditFields.LocationChanged;
            destinationGroup.AuditFields.UsageCount = sourceGroup.AuditFields.UsageCount;

            destinationGroup.CustomIconUuid = sourceGroup.CustomIconUuid;
            destinationGroup.DefaultAutoTypeSequence = sourceGroup.DefaultAutoTypeSequence;
            destinationGroup.EnableAutoType = sourceGroup.EnableAutoType;
            destinationGroup.EnableSearching = sourceGroup.EnableSearching;
            destinationGroup.IconId = sourceGroup.IconId;
            destinationGroup.IsExpanded = sourceGroup.IsExpanded;
            destinationGroup.LastTopVisibleEntry = sourceGroup.LastTopVisibleEntry;
            destinationGroup.Name = sourceGroup.Name;
            destinationGroup.Notes = sourceGroup.Notes;

            return destinationGroup;
        }
    }
}
