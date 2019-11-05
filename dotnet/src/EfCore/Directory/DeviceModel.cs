
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class DeviceModel
    {
        public int Id { get; set; }

        public int? DeviceMakeId  {get; set;}

        public virtual DeviceMake DeviceMake { get; set; }

        public string Name { get; set; }

        public string SupportUri { get; set; }

        public string Properties { get; set; }
    }
}