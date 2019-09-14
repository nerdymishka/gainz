
using System.Collections.Generic;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class PackageManager
    {
        public int Id { get; set; }

        public string Name { get; set;}

        public string Uri { get; set; }

        public virtual ICollection<Package> Packages { get; set; }

        public string Properties { get; set; }
    }
}