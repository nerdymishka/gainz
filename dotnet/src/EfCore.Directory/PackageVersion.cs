
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class PackageVersion
    {
        public int Id { get; set; }

        public int PackageId  {get; set; }

        public Package Application { get; set; }

        public string Platform { get; set; }

        public string Version { get; set; }

        public string Uri { get; set; }

        public int? LicenseId { get; set; }

        public virtual License License { get; set; }

        public string Configuration { get; set; }
    }
}