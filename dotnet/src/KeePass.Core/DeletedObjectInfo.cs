using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class DeletedObjectInfo
    {
        public byte[] Uuid { get; set; }

        public DateTime DeletionTime { get; set; }
    }
}
