
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class ApplicationVersion
    {
        public int Id { get; set; }

        public int ApplicationId  {get; set; }

        public Application Application { get; set; }

        public string Platform { get; set; }

        public string Version { get; set; }

        public string DownloadUri32Bit { get; set; }

        public string DownloadUri { get; set; }

        public int? LicenseId { get; set; }

        public virtual License License { get; set; }

        public string PackageUri { get; set;}
    }
}