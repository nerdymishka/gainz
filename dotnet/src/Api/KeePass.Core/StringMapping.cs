using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class StringMapping
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public ProtectedMemoryString Value { get; set; }
    }
}
