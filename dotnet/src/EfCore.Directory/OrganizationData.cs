
using System.Collections.Generic;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Directory
{
    public class OrganizationData
    {
        public int Id { get; set; }

        public string Name { get; set;}

        [Encrypt]
        public string DisplayName { get; set; }

        public string Uri { get; set; }

        public virtual ICollection<UserData> Users { get; set; }

        public string SyncKey { get; set; }

        public string Properties { get; set; }
    }
}