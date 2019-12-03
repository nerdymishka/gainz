using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public class KeePassIcon : IKeePassIcon
    {
        public byte[] Uuid { get; set; }

        public string Name { get; set; }

        public byte[] Data { get; set; }
    }
}
