using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NerdyMishka.KeePass.Xml
{
    public static class KeePassXmlExtensions
    {
        public static IKeePassPackage OpenKdbx(this IKeePassPackage package, MasterKey key, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            using (var fs = File.OpenRead(filePath))
            {
                return package.Open(key, fs, new KeePassPackageXmlSerializer());
            }
        }

        public static IKeePassPackage OpenKdbx(this IKeePassPackage package, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));


            using (var fs = File.OpenRead(filePath))
            {
                return package.Open(fs, new KeePassPackageXmlSerializer());
            }
        }

        public static IKeePassPackage SaveKdbx(this IKeePassPackage package, MasterKey key, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            using (var fs = File.OpenWrite(filePath))
            {
                return package.Save(key, fs, new KeePassPackageXmlSerializer());
            }
        }

        public static IKeePassPackage SaveKdbx(this IKeePassPackage package, string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentNullException(nameof(filePath));


            using (var fs = File.OpenWrite(filePath))
            {
                return package.Save(fs, new KeePassPackageXmlSerializer());
            }
        }
    }
}
