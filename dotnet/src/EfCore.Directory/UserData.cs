
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class UserData
    {

        public int Id { get; set; }

        [Encrypt]
        public string Upn { get; set; }

        [Encrypt]
        public string DisplayName { get; set; }

        public int? OrganizationDataId { get; set; }

        public OrganizationData Organization { get; set; }

        public string SyncKey { get; set; }

        public string Properties { get; set; }
    }
}