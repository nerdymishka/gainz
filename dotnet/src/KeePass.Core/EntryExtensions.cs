using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NerdyMishka.KeePass
{
    public static class EntryExtensions
    {

        public static void ExportBinaries(this IKeePassEntry entry, string directory, bool force = false)
        {
            if (!System.IO.Directory.Exists(directory))
                throw new System.IO.DirectoryNotFoundException(directory);

            foreach (var binary in entry.Binaries)
            {
                var fileName = System.IO.Path.Combine(directory, binary.Key);
                if (System.IO.File.Exists(fileName) && !force)
                    continue;

                var bytes = binary.Value.UnprotectAsBytes();
                System.IO.File.WriteAllBytes(fileName, bytes);
            }
        }

        public static bool ExportBinary(this IKeePassEntry entry, string directory, string fileName, bool force = false)
        {
            if (!System.IO.Directory.Exists(directory))
                return false;

            var binary = entry.Binaries.SingleOrDefault(o => o.Key == fileName);
            if (binary == null)
                return false;

            var exportFileName = System.IO.Path.Combine(directory, binary.Key);
            if (System.IO.File.Exists(exportFileName) && !force)
                return false;

            var bytes = binary.Value.UnprotectAsBytes();
            System.IO.File.WriteAllBytes(exportFileName, bytes);

            return true;
        }
    }
}
