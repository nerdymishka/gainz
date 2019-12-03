using NerdyMishka.KeePass.Cryptography;
using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace NerdyMishka.KeePass
{
    public class KeePassPackage : IKeePassPackage
    {
        private object syncLock = new object();

        public MasterKey MasterKey { get; protected set; }

        public KeePassFileHeaderInformation HeaderInfo { get; protected set; }

        public KeePassPackageMetaInfo MetaInfo { get; protected set; }

        public IKeePassDocument Document { get; protected set; }

        public DocumentFormats Format { get; set; }

        public IList<BinaryMapping> Binaries { get; set; }


        public IKeePassGroup FindGroup(string path, bool caseInsensitive = true)
        {
            var parts = path.Split('/');

            var last = parts.Length - 1;
            var group = this.Document.RootGroup;


            if (group == null)
                return null;

            var i = 0;

            if (group.Name.ToLowerInvariant() == parts[0].ToLowerInvariant())
            {
                i++;
            }

            for (; i < parts.Length; i++)
            {
                var segement = parts[i];

                if (last == i)
                {
                    if (caseInsensitive)
                        return group.Groups
                            .FirstOrDefault(o => o.Name.ToLowerInvariant() == segement.ToLowerInvariant());
                    else
                        return group.Groups
                            .FirstOrDefault(o => o.Name == segement);
                }

                if (caseInsensitive)
                {
                    group = group.Groups
                        .FirstOrDefault(o => o.Name.ToLowerInvariant() == segement.ToLowerInvariant());

                    if (group == null)
                        return null;
                    else
                        continue;
                }

                group = group.Groups
                            .FirstOrDefault(o => o.Name == segement);

                if (group == null)
                    return null;
            }

            return null;
        }

        

        /// <summary>
        /// Merges a group from the source KeePass file to the group in the destination KeePass file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="overwrite"></param>
        /// <param name="entriesOnly"></param>
        public void Merge(IKeePassPackage destination)
        {
            this.Document.RootGroup.MergeTo(destination.Document.RootGroup, true);
        }

        public IKeePassEntry[] FindEntriesByTitle(string title)
        {
            return this.FindEntriesByTitle(title, false, StringValueComparison.Equal);
        }

        public IKeePassEntry[] FindEntriesByTitle(
            string title,
            bool caseSensitve,
            StringValueComparison stringValueComparison)
        {
            return this.FindEntriesMatchesByField("Title", title, caseSensitve, stringValueComparison);
        }

        public IKeePassEntry[] FindEntriesMatchesByField(
            string name,
            string value)
        {
            return this.FindEntriesMatchesByField(name, value, false, StringValueComparison.Equal);
        }

        public IKeePassEntry[] FindEntriesMatchesByField(
            string name,
            string value,
            bool caseSensitve,
            StringValueComparison stringValueComparison)
        {
            var doc = this.Document;
            var selector = CreateSelector(name, value, caseSensitve, stringValueComparison);
            var matches = new List<IKeePassEntry>();
            foreach (var group in doc.Groups)
            {
                Collect(group, new List<Func<IKeyPassEntryFields, bool>>() { selector }, matches);
            }

            return matches.ToArray();
        }

        private static bool Collect(
            IKeePassGroup group, 
            List<Func<IKeyPassEntryFields, bool>> predicates, 
            List<IKeePassEntry> matches, 
            bool matchAny = true, 
            int? top = null)
        {
            if (group == null)
                return true;

            if (group.Entries != null)
            {
                foreach (var entry in group.Entries)
                {
                    var fields = entry.Fields;
                    var add = false;
                    foreach (var predicate in predicates)
                    {

                        var match = predicate.Invoke(fields);
                        if (!matchAny && !match)
                        {
                            break;
                        }

                        if (matchAny && match)
                        {
                            add = true;
                            break;
                        }
                    }

                    if (add)
                    {
                        matches.Add(entry);
                        if (top.HasValue && top.Value == matches.Count)
                            return false;
                    }
                }
            }

            if (group.Groups != null)
            {
                foreach (var child in group.Groups)
                {
                    if (!Collect(child, predicates, matches))
                        return false;
                }
            }

            return true;
        }

        private static Func<IKeyPassEntryFields, bool> CreateSelector(
            string fieldName, 
            string searchValue, 
            bool caseSensitive = false,
            StringValueComparison stringValueComparison = StringValueComparison.Equal
            )
        {
            
       
            Func<string, bool> match = null;

            if (!caseSensitive)
                searchValue = searchValue.ToLowerInvariant();

            switch (stringValueComparison)
            {
                case StringValueComparison.Equal:
                    match = (input) => input == searchValue;
                    break;
                case StringValueComparison.StartsWith:
                    match = (input) => input.StartsWith(searchValue);
                    break;
                case StringValueComparison.EndsWith:
                    match = (input) => input.EndsWith(searchValue);
                    break;
                case StringValueComparison.Contains:
                    match = (input) => input.Contains(searchValue);
                    break;
                default:
                    throw new NotSupportedException($"Search type {stringValueComparison} is not supported");
            }

            fieldName = fieldName.ToLowerInvariant();
            switch (fieldName)
            {
                case "title":
                    return (entry) =>
                    {
                        if (entry == null)
                            return false;

                        var title = entry.Title;
                        if (title == null)
                            return false;

                        if (!caseSensitive)
                            title = title.ToLowerInvariant();

                        return match(title);
                    };
                case "username":
                    return (entry) =>
                    {
                        var username = entry.UserName;
                        if (username == null)
                            return false;

                        if (!caseSensitive)
                            username = username.ToLowerInvariant();

                        return match(username);
                    };
                case "url":
                    return (entry) =>
                    {
                        var url = entry.Url;
                        if (url == null)
                            return false;

                        if (!caseSensitive)
                            url = url.ToLowerInvariant();

                        return match(url);
                    };

                case "notes":
                    return (entry) =>
                    {


                        var notes = entry.Notes;
                        if (notes == null)
                            return false;

                        if (!caseSensitive)
                            notes = notes.ToLowerInvariant();

                        return match(notes);
                    };
                default:
                    throw new NotSupportedException($"Search field {fieldName} is not supported");
            }
        }

        public IKeePassEntry FindEntry(string path, bool caseInsensitive = true)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var parts = path.Split('/');

            var last = parts.Length - 1;
            var group = this.Document.RootGroup;


            if (group == null)
                return null;

            var i = 0;

            if(group.Name.ToLowerInvariant() == parts[0].ToLowerInvariant())
            {
                i++;
            }

            for (; i < parts.Length; i++)
            {
                var segement = parts[i];

                if(last == i)
                {
                    if (caseInsensitive)
                        return group.Entries
                            .FirstOrDefault(o => o.Name.ToLowerInvariant() == segement.ToLowerInvariant());
                    else
                        return group.Entries
                            .FirstOrDefault(o => o.Name == segement);
                }

                if(caseInsensitive)
                {
                    group = group.Groups
                        .FirstOrDefault(o => o.Name.ToLowerInvariant() == segement.ToLowerInvariant());

                    if (group == null)
                        return null;
                    else
                        continue;
                }

                group = group.Groups
                            .FirstOrDefault(o => o.Name == segement);

                if (group == null)
                    return null;
            }

            return null;
        }

        public IKeePassGroup CreateGroup(string path, IKeePassGroup group, bool force = false)
        {
            var instance = group;
            var parts = path.Split('/');
            var i = 0;

            group = this.Document.RootGroup;
            if (group == null)
            {
                group = new KeePassGroup() { Name = parts[0] };
                this.Document.Add(group);
                i++;
            }
            else if(parts[0].ToLowerInvariant() == group.Name.ToLowerInvariant())
            {
                i++;
            }

            if (i == parts.Length)
                return group;

            var last = parts.Length - 1;
            for (; i < parts.Length; i++) {
                var segment = parts[i];

                if(last == i)
                {
                    group.Add(instance);
                    instance.Name = segment;

                    return instance;
                }

                var next = group.Groups.FirstOrDefault(o => o.Name.ToLowerInvariant() == segment.ToLowerInvariant());
                if(next != null)
                {
                    group = next;
                    continue;
                }

               
                if (force)
                {
                    next = new KeePassGroup() { Name = segment };
                    group.Add(next);

                    group = next;
                    continue;
                }

                return null;
            }


            return null;
        }

        /// <summary>
        /// Creates a s
        /// </summary>
        /// <param name="path"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        public IKeePassGroup CreateGroup(string path, bool force = false) 
        {
            var parts = path.Split('/');
            var i = 0;

            var group = this.Document.RootGroup;
            if (group == null)
            {
                group = new KeePassGroup() { Name = parts[0] };
                this.Document.Add(group);
                i++;
            }
            else if(parts[0].ToLowerInvariant() == group.Name.ToLowerInvariant())
            {
                i++;
            }

            if (i == parts.Length)
                return group;

            var last = parts.Length - 1;
            for (; i < parts.Length; i++) {
                var segment = parts[i];

                if(last == i)
                {
                    var lastGroup = new KeePassGroup() {
                        Name = segment
                    };
                    

                    group.Add(group);

                    return lastGroup;
                }

                var next = group.Groups.FirstOrDefault(o => o.Name.ToLowerInvariant() == segment.ToLowerInvariant());
                if(next != null)
                {
                    group = next;
                    continue;
                }

               
                if (force)
                {
                    next = new KeePassGroup() { Name = segment };
                    group.Add(next);

                    group = next;
                    continue;
                }

                return null;
            }


            return null;
        }

        public IKeePassEntry CreateEntry(string path,
            IKeePassEntry entry,  
            bool force = false)
        {
            var group = this.Document.RootGroup;
            if (group == null)
                return null;

            var parts = path.Split('/');
            if (parts.Length < 2)
                throw new ArgumentException("path must have at least one group");

            var currentPw = entry.UnprotectPasswordAsBytes();
            if(currentPw == null || currentPw.Length == 0)
                 entry.SetPassword(PasswordGenerator.GenerateAsBytes(16));

            
            var i = 0;
            if(parts[0].ToLowerInvariant() == group.Name.ToLowerInvariant())
            {
                i++;
            }

            var last = parts.Length - 1;
            for (; i < parts.Length; i++) {
                var segment = parts[i];

                if(last == i)
                {
                    group.Add(entry);
                    entry.Name = segment;

                    return entry;
                }

                var next = group.Groups.FirstOrDefault(o => o.Name.ToLowerInvariant() == segment.ToLowerInvariant());
                if(next != null)
                {
                    group = next;
                    continue;
                }

               
                if (force)
                {
                    next = new KeePassGroup() { Name = segment };
                    group.Add(next);

                    group = next;
                    continue;
                }

                return null;
            }


            return null;
        }

        /// <summary>
        /// Creates an entry at the specified path location e.g. Root/Group1/EntryTitle
        /// </summary>
        /// <param name="path">The path for this entry e.g. Root/Group1/EntryTitle</param>
        /// <param name="password">The password for this entry</param>
        /// <param name="username">The name of the user for this entry.</param>
        /// <param name="url">The url for this entry.</param>
        /// <param name="notes">The note data for this entry.</param>
        /// <param name="tags">The tags for this entry.</param>
        /// <param name="force">If true, groups will be created if they do not exist.</param>
        /// <returns><see cref="IKeePassEntry"/></returns>
        public IKeePassEntry CreateEntry(string path,
            byte[] password = null,
            string username = null, 
            string url = null, 
            string notes = null, 
            IEnumerable<string> tags = null,  
            bool force = false)
        {
            var group = this.Document.RootGroup;
            if (group == null)
                return null;

            var parts = path.Split('/');
            if (parts.Length < 2)
                throw new ArgumentException("path must have at least one group");

            if (password == null)
                password = PasswordGenerator.GenerateAsBytes(20);

            
            var i = 0;
            if(parts[0].ToLowerInvariant() == group.Name.ToLowerInvariant())
            {
                i++;
            }

            var last = parts.Length - 1;
            for (; i < parts.Length; i++) {
                var segment = parts[i];

                if(last == i)
                {
                    var entry = new KeePassEntry(true);
                    
                    var fields = entry.Fields;
                    fields.Title = segment;
                    fields.UserName = username;
                    fields.Url = url;
                    fields.Notes = notes;
                    entry.SetPassword(password);

                    if(tags != null)
                    {
                        foreach (var tag in tags)
                            fields.Tags.Add(tag);
                    }
                    

                    group.Add(entry);

                    return entry;
                }

                var next = group.Groups.FirstOrDefault(o => o.Name.ToLowerInvariant() == segment.ToLowerInvariant());
                if(next != null)
                {
                    group = next;
                    continue;
                }

               
                if (force)
                {
                    next = new KeePassGroup() { Name = segment };
                    group.Add(next);

                    group = next;
                    continue;
                }

                return null;
            }


            return null;
        }

        /// <summary>
        /// Custom Data
        /// </summary>
        public IList<StringMapping> Strings { get; set; }

        static KeePassPackage()
        {
            if(ProtectedMemoryBinary.DataProtectionAction == null)
            {
                var provider = new PortableSalsa20ProtectedDataProvider();
                ProtectedMemoryBinary.DataProtectionAction = (data, state, operation) =>
                {
                    var protectedData = (ProtectedMemoryBinary)state;
                    if (operation == DataProtectionActionType.Encrypt)
                        return provider.ProtectData(data, protectedData.Id);
                    else
                        return provider.UnprotectData(data, protectedData.Id);
                };
            }
        }

        public KeePassPackage(MasterKey key = null, string rootGroupName = null)
        {
            this.Binaries = new List<BinaryMapping>();
            this.Strings = new List<StringMapping>();
            this.HeaderInfo = new KeePassFileHeaderInformation();
            this.MetaInfo = new KeePassPackageMetaInfo();
            this.Document = new KeePassDocument(this);
            this.MasterKey = key;

            if (!string.IsNullOrWhiteSpace(rootGroupName))
                this.Document.Add(new KeePassGroup()
                {
                    Name = rootGroupName
                });

            this.HeaderInfo.GenerateValues();
        }


        public KeePassPackage() {
            this.Binaries = new List<BinaryMapping>();
            this.Strings = new List<StringMapping>();
            this.HeaderInfo = new KeePassFileHeaderInformation();
            this.MetaInfo = new KeePassPackageMetaInfo();
            this.Document = new KeePassDocument(this); 
            this.HeaderInfo.GenerateValues();
        }

   

        public KeePassPackage(MasterKey key, Stream stream, IKeePassPackageSerializer serializer) :
            this(key, null)
        {
            this.Open(key, stream, serializer);
        }

        public IKeePassPackage SetKey(MasterKey key)
        {
            this.MasterKey = key;
            return this;
        }

        public IKeePassPackage Open(MasterKey key, Stream stream, IKeePassPackageSerializer serializer)
        {
            this.MasterKey = key;
            Open(this, key, stream, serializer);
            return this;
        }

        public IKeePassPackage Open(Stream stream, IKeePassPackageSerializer serializer)
        {
            if (this.MasterKey == null)
                throw new InvalidOperationException("MasterKey must be set before calling open without specifying a master key.");

            Open(this, this.MasterKey, stream, serializer);
            return this;
        }


        public IKeePassPackage Save(MasterKey key, Stream stream, IKeePassPackageSerializer serializer)
        {
            this.MasterKey = key;
            Save(this, key, stream, serializer);
            return this;
        }

        public IKeePassPackage Save(Stream stream, IKeePassPackageSerializer serializer)
        {
            if (this.MasterKey == null)
                throw new InvalidOperationException("MasterKey must be set before calling open without specifying a master key.");

            Save(this, this.MasterKey, stream, serializer);
            return this;
        }


        public void AttachFile(IKeePassEntry entry, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));

            var key = Path.GetFileName(path);
            var bytes = File.ReadAllBytes(path);
            this.AttachBinary(entry, key, bytes);
        }
        
        public void AttachBinary(IKeePassEntry entry, string key, byte[] data)
        {
            this.AttachBinary(entry, key, new ProtectedMemoryBinary(data, true));
        }

        public void AttachBinary(IKeePassEntry entry, string key, ProtectedMemoryBinary protectedMemoryBinary)
        {
            lock(this.syncLock)
            {
                int id = this.Binaries.Count;
                this.Binaries.Add(new BinaryMapping()
                {
                    Id = id,
                    Key = key,
                    Value = protectedMemoryBinary
                });

                entry.Binaries.Add(new ProtectedBinary()
                {
                    Key = key,
                    Ref = id,
                    Value = protectedMemoryBinary
                });

                this.MetaInfo.Binaries.Add(new BinaryInfo()
                {
                    Compressed = this.HeaderInfo.DatabaseCompression == (byte)1,
                    Id = id
                });
            }
        }

        public static IKeePassPackage Open(MasterKey key, string path, IKeePassPackageSerializer serializer)
        {
            using (var fs = File.OpenRead(path))
            {
                return new KeePassPackage(key, fs, serializer);
            } 
        }



        private static void Open(KeePassPackage package, MasterKey key, Stream stream, IKeePassPackageSerializer serializer)
        {
            package.HeaderInfo.Read(stream);
            var outerStream = KeePassFileCryptoStreamFactory.CreateCryptoStream(
                stream,
                false,
                key,
                package.HeaderInfo);

            byte[] expectedHeaderByteMark = package.HeaderInfo.HeaderByteMarks;
            byte[] actualHeaderByteMark = outerStream.ReadBytes(32);

            
            bool equal = Check.Equal(expectedHeaderByteMark, actualHeaderByteMark);
            if(!equal)
                throw new Exception("Invalid File Format");

            outerStream = new HMACBlockStream(outerStream, false);

            if(package.HeaderInfo.DatabaseCompression == 1)
               outerStream = new GZipStream(outerStream, CompressionMode.Decompress);

            serializer.Read(package, outerStream);
            outerStream.Dispose();
            
        }

        private static void Save(KeePassPackage package, MasterKey masterKey, Stream stream, IKeePassPackageSerializer serializer)
        {
            //package.HeaderInfo.GenerateValues();
            var actualByteMarks = package.HeaderInfo.HeaderByteMarks;
            package.HeaderInfo.Write(stream);
            var outerStream = KeePassFileCryptoStreamFactory.CreateCryptoStream(
                stream,
                true,
                masterKey,
                package.HeaderInfo);

           
            outerStream.Write(actualByteMarks, 0, actualByteMarks.Length);

            outerStream = new HMACBlockStream(outerStream, true);

            if (package.HeaderInfo.DatabaseCompression == 1)
                outerStream = new GZipStream(outerStream, CompressionMode.Compress);

      
            serializer.Write(package, outerStream);
            outerStream.Dispose();
        }

        public void Dispose()
        {
            if(this.MasterKey != null)
                this.MasterKey = null;

            if(this.MetaInfo != null)
                this.MetaInfo = null;

            if(this.Document != null)
                this.Document = null;

            if (this.Strings != null)
            {
                this.Strings.Clear();
                this.Strings = null;
            }

            if(this.Binaries != null)
            {
                this.Binaries.Clear();
                this.Binaries = null;
            }
        }
    }
}
