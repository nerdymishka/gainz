
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class Device
    {

        public int Id { get; set; }

        public string Serial { get; set; }

        public string DisplayName { get; set; }

        public bool Is64Bit { get; set; }

        public string Arch { get; set; }

        public int? OwnerId { get; set; }

        public UserData Owner { get; set; }

        public int? OrganizationDataId { get; set; }

        public OrganizationData Organization { get; set; }

        public int? DeviceModelId { get; set; }

        public virtual DeviceModel DeviceModel { get; set; }

        public string SyncKey { get; set; }

        public string Properties { get; set; }
    }
}