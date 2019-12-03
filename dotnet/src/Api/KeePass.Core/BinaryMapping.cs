using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class BinaryMapping
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public ProtectedMemoryBinary Value { get; set; }
    }
}
