using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass
{
  
    public class KeePassPackageMetaInfo 
    {
        public KeePassPackageMetaInfo()
        {
            var empty = new byte[16];
            var now = DateTime.Now;
            this.RecycleBinUUID = empty;
            this.EntryTemplatesGroup = empty;
            this.LastSelectedGroup = empty;
            this.LastTopVisibleGroup = empty;
            this.MaintenanceHistoryDays = 365;
            this.MasterKeyChangeForce = -1;
            this.MasterKeyChangeRec = -1;
            this.HistoryMaxItems = 10;
            this.HistoryMaxSize = 6291456;
            this.DatabaseNameChanged = now;
            this.DatabaseDescriptionChanged = now;
            this.DefaultUserNameChanged = now;
            this.EntryTemplatesGroupChanged = now;
            this.MemoryProtection = new MemoryProtection();
            this.Binaries = new List<BinaryInfo>();
            this.CustomIcons = new List<IKeePassIcon>();
        }

        public string Generator { get; set; }

        public string DatabaseName { get; set; }

        public DateTime DatabaseNameChanged { get; set; }

        public string DatabaseDescription { get; set; }

        public DateTime DatabaseDescriptionChanged { get; set; }

        public string DefaultUserName { get; set; }

        public DateTime DefaultUserNameChanged { get; set; }

        public int MaintenanceHistoryDays { get; set; }

        public string Color { get; set; }

        public DateTime MasterKeyChanged { get; set; }

        public int MasterKeyChangeRec { get; set; }

        public int MasterKeyChangeForce { get; set; }

        public MemoryProtection MemoryProtection { get; set; }

        public bool RecycleBinEnabled { get; set; }

        public byte[] RecycleBinUUID { get; set; }

        public DateTime RecycleBinChanged { get; set; }

        public byte[] EntryTemplatesGroup { get; set; }

        public DateTime EntryTemplatesGroupChanged { get; set; }

        public int HistoryMaxItems { get; set; }

        public int HistoryMaxSize { get; set; }

        public byte[] LastSelectedGroup { get; set; }

        public byte[] LastTopVisibleGroup { get; set; }

        public List<BinaryInfo> Binaries { get; set; }

        public List<IKeePassIcon> CustomIcons { get; set; }

        public SortedDictionary<string, string> CustomData { get; set; }
    }
}
