using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassEntry : IKeePassEntry
    {
        private KeePassEntryFields fields;
        private IList<string> tags;
        private IList<ProtectedString> strings;
        private IList<IKeePassEntry> history;
        private IList<ProtectedBinary> binaries;
        private IKeePassAuditFields audit;

        public KeePassEntry()
        {
            this.Uuid = Guid.NewGuid().ToByteArray();
            this.audit = new KeePassAuditFields();
            this.PreventAutoCreate = false;
        }

        public KeePassEntry(bool populate): this()
        {
            if(populate)
            {
                this.Strings.Add(new ProtectedString("Notes"));
                this.Strings.Add(new ProtectedString("Password"));
                this.Strings.Add(new ProtectedString("Title"));
                this.Strings.Add(new ProtectedString("URL"));
                this.Strings.Add(new ProtectedString("UserName"));
            }
        }

        public byte[] Uuid { get; set; }

        public int IconId { get; set; }

        public byte[] CustomIconUuid { get; set; }

        public string ForegroundColor { get; set; }

        public string BackgroundColor { get; set; }

        public string OverrideUrl { get; set; }

        public bool PreventAutoCreate { get; set; }

        public IKeePassPackage Owner { get; internal protected set; }

        public IList<string> Tags
        {
            get {
                if (!this.PreventAutoCreate && this.tags == null)
                    this.tags = new List<string>();
                return this.tags;
            }
            set
            {
                if (this.tags == null && value == null)
                    return;

                if(this.tags != null && this.tags.EqualTo(value))
                    return;

                this.AuditFields.LastModificationTime = DateTime.UtcNow;

                if (value == null)
                    this.tags = new List<string>();
                else
                    this.tags = new List<string>(value);
            }
        }

        public IKeePassAuditFields AuditFields => this.audit;

        public IList<ProtectedString> Strings
        {
            get {
                if (!this.PreventAutoCreate && this.strings == null)
                    this.strings = new List<ProtectedString>();
                return this.strings;
            }
        }

        public IKeePassAutoType AutoType { get; set; }

        public IList<IKeePassEntry> History
        {
            get
            {
                if (!this.PreventAutoCreate && this.history == null)
                    this.history = new List<IKeePassEntry>();

                return this.history;
            }
        }

        public bool IsHistorical { get; set; }

        public IList<ProtectedBinary> Binaries
        {
            get {
                if (!this.PreventAutoCreate && this.binaries == null)
                    this.binaries = new List<ProtectedBinary>();
                return this.binaries;
            }
        }

        public IKeyPassEntryFields Fields
        {
            get {
                if (!this.PreventAutoCreate && this.fields == null)
                    this.fields = new KeePassEntryFields(this);
                return this.fields;
            }
        }

        public string Name
        {
            get => this.GetString("Title");
            set => this.SetString("Title", value);
        }

        public string Url
        {
            get => this.GetString("URL");
            set => this.SetString("URL", value);
        }


        public string UserName
        {
            get => this.GetString("UserName");
            set => this.SetString("UserName", value);
        }

        public string GetString(string name)
        {
            var ps = this.Strings.SingleOrDefault(o => o.Key == name);
            if (ps == null || ps.Value == null)
                return null;

            return ps.Value.UnprotectAsString();
        }

        public byte[] GetStringAsBytes(string name)
        {
            var ps = this.Strings.SingleOrDefault(o => o.Key == name);
            if (ps == null || ps.Value == null)
                return null;

            return ps.Value.UnprotectAsBytes();
        }

        public void SetString(string name, byte[] value)
        {
            var ps = this.Strings.SingleOrDefault(o => o.Key == name);
            if (ps == null)
            {
                ps = new ProtectedString(name);
                this.Strings.Add(ps);
                this.AuditFields.LastModificationTime = DateTime.UtcNow;
            }
            


            ps.Value = new ProtectedMemoryString(value);
        }

        public void SetString(string name, string value)
        {
            var ps = this.Strings.SingleOrDefault(o => o.Key == name);

             

            if (ps == null)
            {
                ps = new ProtectedString(name);
                this.Strings.Add(ps);
                this.AuditFields.LastModificationTime = DateTime.UtcNow;
            } else {
                if(ps.Value == null)
                {
                    this.AuditFields.LastModificationTime = DateTime.UtcNow;
                } else {
                    var current = ps.Value.UnprotectAsString();
                    if(current != value) {
                        this.AuditFields.LastModificationTime = DateTime.UtcNow;
                    }
                }
            }


            ps.Value = new ProtectedMemoryString(value);
        }

        public SecureString UnprotectPasswordAsSecureString()
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null || ps.Value == null)
                return null;

            return ps.Value.UnprotectAsSecureString();
        }

        public byte[] UnprotectPasswordAsBytes()
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null || ps.Value == null)
                return null;

            return ps.Value.UnprotectAsBytes();
        }

        public string UnprotectPassword()
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null || ps.Value == null)
                return null;
            
            return ps.Value.UnprotectAsString();
        }

        public void SetPassword(string password)
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null)
            {
                ps = new ProtectedString("Password");
                this.strings.Add(ps);
            }

            if (password != null)
                ps.Value = new ProtectedMemoryString(password);
        }

        public void SetPassword(SecureString password)
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null)
            {
                ps = new ProtectedString("Password");
                this.strings.Add(ps);
            }

            if (password != null)
            {
                IntPtr bstr = IntPtr.Zero;
                char[] charArray = new char[password.Length];

                try
                {
                    
                    bstr = Marshal.SecureStringToBSTR(password);
                    Marshal.Copy(bstr, charArray, 0, charArray.Length);

                    var bytes = Encoding.UTF8.GetBytes(charArray);
                    ps.Value = new ProtectedMemoryString(bytes);

                    bytes.Clear();
                    charArray.Clear();
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }
        }

        public void SetPassword(byte[] password)
        {
            var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
            if (ps == null)
            {
                ps = new ProtectedString("Password");
                this.strings.Add(ps);
            }

            if (password != null)
                ps.Value = new ProtectedMemoryString(password);
        }

        public string Notes
        {
            get => this.GetString("Notes");
            set => this.SetString("Notes", value);
        }

        public IKeePassEntry CopyTo(IKeePassEntry destination)
        {
            var sourcePackage = this.Owner;
            var destinationPackage = destination.Owner;

            var next = destination;
            var entry = this;
            var destFields = next.Fields;
            var srcFields =  entry.Fields;
            next.IconId = entry.IconId;
            next.BackgroundColor = entry.BackgroundColor;
            next.Uuid = entry.Uuid;

            next.AuditFields.CreationTime = entry.AuditFields.CreationTime;
            next.AuditFields.Expires = entry.AuditFields.Expires;
            next.AuditFields.ExpiryTime = entry.AuditFields.ExpiryTime;
            next.AuditFields.LastAccessTime = entry.AuditFields.LastAccessTime;
            next.AuditFields.LastModificationTime = entry.AuditFields.LastModificationTime;
            next.AuditFields.LocationChanged = entry.AuditFields.LocationChanged;
            next.AuditFields.UsageCount = entry.AuditFields.UsageCount;

            next.AutoType = entry.AutoType;
            next.CustomIconUuid = entry.CustomIconUuid;
            if (next.CustomIconUuid != null)
            {
                if(destinationPackage != null)
                {
                    var nextIcon = destinationPackage.MetaInfo.CustomIcons.SingleOrDefault(o => o.Uuid.EqualTo(next.CustomIconUuid));
                    if (nextIcon == null)
                    {
                        nextIcon = sourcePackage.MetaInfo.CustomIcons.SingleOrDefault(o => o.Uuid.EqualTo(next.CustomIconUuid));
                        destinationPackage.MetaInfo.CustomIcons.Add(nextIcon);
                    }
                }
            }
            next.ForegroundColor = entry.ForegroundColor;
            next.BackgroundColor = entry.BackgroundColor;
            next.History.Clear();
            next.OverrideUrl = entry.OverrideUrl;
            next.PreventAutoCreate = entry.PreventAutoCreate;


            next.Tags.Clear();

            if (entry.Tags != null)
            {
                foreach (var tag in entry.Tags)
                {
                    next.Tags.Add(tag);
                }
            }


            next.Strings.Clear();

            foreach (var str in entry.Strings)
            {
                next.Strings.Add(str);
            }


            foreach (var binary in entry.Binaries)
            {
                var nextBinary = next.Binaries.SingleOrDefault(o => o.Key == binary.Key);
                if (nextBinary != null)
                {
                    if (!nextBinary.Value.Equals(binary.Value))
                    {
                        nextBinary.Value = binary.Value;
                        if(destinationPackage != null)
                        {
                            var mainBinary = destinationPackage.Binaries.SingleOrDefault(o => o.Key == binary.Key);
                            mainBinary.Value = binary.Value;
                        }
                    }

                    continue;
                }

                if(destinationPackage != null)
                    destinationPackage.AttachBinary(next, binary.Key, binary.Value);
            }

            return next;
        }
    

        public class KeePassEntryFields : IKeyPassEntryFields
        {
            private IList<ProtectedString> strings;
            private KeePassEntry entry;


            public KeePassEntryFields(KeePassEntry entry)
            {
                this.strings = entry.Strings;
                this.entry = entry;
            }


            public string Title
            {
                get
                {
                   var ps = this.strings.SingleOrDefault(o => o.Key == "Title");
                    if (ps == null || ps.Value == null)
                        return null; 

                    return ps.Value.UnprotectAsString();
                }

                set
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "Title");
                    if (ps == null)
                    {
                        ps = new ProtectedString("Title");
                        this.strings.Add(ps);
                    }

                    if (value != null)
                        ps.Value = new ProtectedMemoryString(value);
                }
            }

            public string Password
            {
                get
                {
                    return "************";
                }

                set
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
                    if (ps == null)
                    {
                        ps = new ProtectedString("Password");
                        this.strings.Add(ps);
                    }

                    if (value != null)
                        ps.Value = new ProtectedMemoryString(value);
                }
            }

            public string Url
            {
                get
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "URL");
                    if (ps == null || ps.Value == null)
                        return null;

                    return ps.Value.UnprotectAsString();
                }

                set
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "URL");
                    if (ps == null)
                    {
                        ps = new ProtectedString("URL");
                        this.strings.Add(ps);
                    }

                    if (value != null)
                        ps.Value = new ProtectedMemoryString(value);
                }
            }

            public string UserName
            {
                get
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "UserName");
                    if (ps == null || ps.Value == null)
                        return null;

                    return ps.Value.UnprotectAsString();
                }

                set
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "UserName");
                    if (ps == null)
                    {
                        ps = new ProtectedString("UserName");
                        this.strings.Add(ps);
                    }

                    if (value != null)
                        ps.Value = new ProtectedMemoryString(value);
                }
            }

            public IList<string> Tags
            {
                get
                {
                    return this.entry.Tags;
                }

                set
                {
                    if (value == null)
                        this.entry.Tags.Clear();
                    else
                        this.entry.Tags = new List<string>(value);
                }
            }

            public string Notes
            {
                get
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "Notes");
                    if (ps == null || ps.Value == null)
                        return null;

                    return ps.Value.UnprotectAsString();
                }

                set
                {
                    var ps = this.strings.SingleOrDefault(o => o.Key == "Notes");
                    if (ps == null)
                    {
                        ps = new ProtectedString("Notes");
                        this.strings.Add(ps);
                    }

                    if (value != null)
                        ps.Value = new ProtectedMemoryString(value);
                }
            }

            public string UnprotectPassword()
            {

                var ps = this.strings.SingleOrDefault(o => o.Key == "Password");
                if (ps == null || ps.Value == null)
                    return null;

                return ps.Value.UnprotectAsString();
            }
        }
    }
}
