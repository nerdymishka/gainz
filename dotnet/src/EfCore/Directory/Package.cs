
using System.Collections.Generic;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class Package
    {
        public int Id { get; set; }

        public string Name { get; set;}

        public int? LicenseId { get; set; }

        public virtual License License { get; set; }

        
    }
}