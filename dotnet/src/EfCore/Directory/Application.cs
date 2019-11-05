
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class Application
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? VendorId  { get; set; }

        public Vendor Vendor { get; set; }

        public int? LicenseId { get; set; }

        public License License { get; set; }

        public string Properties { get; set; }
    }
}