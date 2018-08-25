using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IKeePassPackage : IDisposable
    {

         MasterKey MasterKey { get; }

         KeePassFileHeaderInformation HeaderInfo { get; }

         KeePassPackageMetaInfo MetaInfo { get;  }

         IKeePassDocument Document { get; }

         IList<BinaryMapping> Binaries { get; }

        /// <summary>
        /// Custom Data
        /// </summary>
         IList<StringMapping> Strings { get; }

        void AttachFile(IKeePassEntry entry, string path);

        void AttachBinary(IKeePassEntry entry, string key, byte[] data);
        void AttachBinary(IKeePassEntry entry, string key, ProtectedMemoryBinary protectedMemoryBinary);

        IKeePassGroup CreateGroup(
            string path,
            bool force = false);

         IKeePassGroup CreateGroup(
            string path,
            IKeePassGroup group,
            bool force = false);

        IKeePassEntry CreateEntry(
            string path,
            byte[] password = null,
            string username = null,
            string uri = null,
            string notes = null,
            IEnumerable<string> tags = null,
            bool force = false);

         IKeePassEntry CreateEntry(
            string path,
            IKeePassEntry entry,
            bool force = false);

        IKeePassEntry FindEntry(
           string path,
           bool caseInsensitive = true);

        IKeePassEntry[] FindEntriesMatchesByField(
           string name,
           string value);

        IKeePassEntry[] FindEntriesMatchesByField(
           string name,
           string value,
           bool caseSensitve,
           StringValueComparison stringValueComparison);

        IKeePassEntry[] FindEntriesByTitle(
            string title);

        IKeePassEntry[] FindEntriesByTitle(
            string title,
            bool caseSensitve,
            StringValueComparison stringValueComparison);

        IKeePassGroup FindGroup(
            string path,
            bool caseInsensitive = true);

        void Merge(IKeePassPackage package);

        IKeePassPackage SetKey(MasterKey key);

        IKeePassPackage Open(MasterKey key, Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Open(Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Save(MasterKey key, Stream stream, IKeePassPackageSerializer serializer);

        IKeePassPackage Save(Stream stream, IKeePassPackageSerializer serializer);
    }
}
