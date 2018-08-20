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

        void Add(IKeePassEntry entry);

        void Add(IKeePassGroup group);

        void Remove(IKeePassEntry entry);

        void Remove(IKeePassGroup group);

        IKeePassGroup Group(int index);

        IKeePassGroup Group(string name);

        IKeePassEntry Entry(int index);

        IKeePassEntry Entry(string name);

        IKeePassGroup CopyTo(IKeePassGroup destinationGroup);

        void ExportTo(IKeePassGroup destination);

        void MergeTo(IKeePassGroup destination, bool overwrite = false, bool entriesOnly = false);
    }

}