using System;
using System.Collections.Generic;

namespace NerdyMishka.KeePass
{
    public interface IKeePassGroup : IKeePassNode
    {
         bool IsExpanded { get; set; }

         string DefaultAutoTypeSequence { get; set; }

         bool? EnableAutoType { get; set; }
        
         bool? EnableSearching { get; set; }
        
         byte[] LastTopVisibleEntry { get; set; }
          
         IEnumerable<IKeePassEntry> Entries { get; }

         IEnumerable<IKeePassGroup> Groups { get; }

         byte[] CustomIconUuid { get; set; }

        /// <summary>
        /// Adds the entry to the group. Sets the entry owner to 
        /// the instance of this group.
        /// </summary>
        /// <param name="entry">The entry to add.</param>
        void Add(IKeePassEntry entry);

        /// <summary>
        /// Adds the sub group to the group. Sets the sub group owner to 
        /// the instance of this group.
        /// </summary>
        /// <param name="group">The sub group to add.</param>
        void Add(IKeePassGroup group);

        /// <summary>
        /// Removes entry from the group. Sets the owner to null.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        void Remove(IKeePassEntry entry);

        /// <summary>
        /// Removes the group from the group. Sets the owner to null.
        /// </summary>
        /// <param name="group">The group to remove.</param>
        void Remove(IKeePassGroup group);
        
        /// <summary>
        /// Gets a group by index. If the index is higher than 
        /// the number of groups or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the group.</param>
         /// <returns>The group; otherwise, null.</returns>
        IKeePassGroup Group(int index);

        /// <summary>
        /// Gets a group by name. The name equality test is case
        /// insenstive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the group.</param>
        /// <returns>The group; otherwise, null.</returns>
        IKeePassGroup Group(string name);

        /// <summary>
        /// Gets an entry by index. If the index is higher than the
        /// number of entries or lower than zero, null is returned.
        /// </summary>
        /// <param name="index">The index of the entry.</param>
        /// <returns>The entry; otherwise, null.</returns>
        IKeePassEntry Entry(int index);

        /// <summary>
        /// Gets an entry by name. The name equality test is case
        /// insenstive. Returns null if the entry is not found.
        /// </summary>
        /// <param name="name">The name (title) of the entry.</param>
        /// <returns>The entry; otherwise, null.</returns>
        IKeePassEntry Entry(string name);

        /// <summary>
        /// Copies all the values of this instance to the destination group.
        /// </summary>
        /// <param name="destinationGroup">The group the values should be copied to</param>
        /// <returns>The destination group</returns>
        IKeePassGroup CopyTo(IKeePassGroup destinationGroup);

        void ExportTo(IKeePassGroup destination);

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
        void MergeTo(IKeePassGroup destination, bool overwrite = false, bool ignoreGroups = false);
    }

}