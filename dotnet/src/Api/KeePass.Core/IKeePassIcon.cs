using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IKeePassIcon
    {
        byte[] Uuid { get; set; }

        string Name { get; set; }

        byte[] Data { get; set; }
    }
}
