using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IPackageMetaInfo
    {
         string Generator { get; set; }

         string DatabaseName { get; set; }

         DateTime DatabaseNameChanged { get; set; }

         string DatabaseDescription { get; set; }

         DateTime DatabaseDescriptionChanged { get; set; }

         string DefaultUserName { get; set; }

         DateTime DefaultUserNameChanged { get; set; }

         int MaintenanceHistoryDays { get; set; }

         string Color { get; set; }

         DateTime MasterKeyChanged { get; set; }

         int MasterKeyChangeRec { get; set; }

         int MasterKeyChangeForce { get; set; }

         MemoryProtection MemoryProtection { get; set; }

         bool RecycleBinEnabled { get; set; }

         byte[] RecycleBinUUID { get; set; }

         DateTime RecycleBinChanged { get; set; }

         byte[] EntryTemplatesGroup { get; set; }

         DateTime EntryTemplatesGroupChanged { get; set; }

         int HistoryMaxItems { get; set; }

         int HistoryMaxSize { get; set; }

         byte[] LastSelectedGroup { get; set; }

         byte[] LastTopVisibleGroup { get; set; }

         IList<BinaryInfo> Binaries { get; set; }

         IList<IKeePassIcon> CustomIcons { get; set; }

         SortedDictionary<string, string> CustomData { get; set; }
    }
}
