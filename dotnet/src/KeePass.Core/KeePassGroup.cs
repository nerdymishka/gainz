using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassGroup : IKeePassGroup, IEnumerable<IKeePassEntry>
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

        public KeePassGroup(string name, string notes = null, int iconId = 0, byte[] customIconId = null) : this()
        {
            this.Name = name;
            this.Notes = notes;
            this.IconId = iconId;
            this.CustomIconUuid = customIconId;
        }

        public byte[] Uuid { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public int IconId { get; set; } = (int)KeePassUiIcons.FolderWithDocument;

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

        /// <summary>
        /// Gets a group by index. If the index is higher than 
        /// the number of groups or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the group.</param>
         /// <returns>The group; otherwise, null.</returns>
        public IKeePassGroup Group(int index)
        {
            if (this.groups == null)
                return null;

            if (index < 0 || index > this.groups.Count)
                return null;

            return this.groups[0];
        }

        /// <summary>
        /// Gets a group by name. The name equality test is case
        /// insenstive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the group.</param>
        /// <returns>The group; otherwise, null.</returns>
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
        
        /// <summary>
        /// Gets an entry by name. The name equality test is case
        /// insenstive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the entry.</param>
        /// <returns>The entry; otherwise, null.</returns>
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

        /// <summary>
        /// Gets an entry by index. If the index is higher than the
        /// number of entries or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the entry.</param>
        /// <returns>The entry; otherwise, null.</returns>
        public IKeePassEntry Entry(int index)
        {
            if (this.entries == null)
                return null;

            if (index < 0 || index > this.entries.Count)
                return null;

            return this.entries[0];
        }

   
        /// <summary>
        /// Adds the sub group to the group. Sets the sub group owner to 
        /// the instance of this group.
        /// </summary>
        /// <param name="group">The sub group to add.</param>
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

        /// <summary>
        /// Adds the entry to the group. Sets the entry owner to 
        /// the instance of this group.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
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

         /// <summary>
        /// Removes the group from the group. Sets the owner to null.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        public void Remove(IKeePassGroup group)
        {
             if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (this.groups == null)
                return;

            if (this.groups.Remove(group))
            {
                if (group is KeePassGroup)
                    ((KeePassGroup)group).Owner = null;
            }
        }

        /// <summary>
        /// Removes entry from the group. Sets the owner to null.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        public void Remove(IKeePassEntry entry)
        {
             if (entry == null)
                throw new ArgumentNullException(nameof(entry));

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

        /// <summary>
        /// Merges all the sub groups and entries to the destination group. Merge only
        /// modifies the <see cref="IKeePassGroup.Groups" /> and <see cref="IKeePassGroup.Entries" />
        /// properties. Entries for each sub group will be merged.
        /// </summary>
        /// <param name="destination">The group that should recieve the entries and groups.</param>
        /// <param name="overwrite">
        /// If the entry or group isn't new, the values will be overwritten 
        /// if the Uuid property matches.
        /// </param>
        /// <param name="ignoreGroups">
        /// If true, the subgroups and the subgroup entities will not be merged. 
        /// </param>   
        public void MergeTo(IKeePassGroup destination, bool overwrite = false, bool ignoreGroups = false)
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

            if (ignoreGroups)
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

                group.MergeTo(next, overwrite, ignoreGroups);
            }
        }

        /// <summary>
        /// Copies all the values of this instance to the destination group.
        /// </summary>
        /// <param name="destinationGroup">The group the values should be copied to</param>
        /// <returns>The destination group</returns>
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

        public IEnumerator<IKeePassEntry> GetEnumerator()
        {
            foreach(var entry in this.Entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
