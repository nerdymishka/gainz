using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class ProtectedString
    {
        public ProtectedString()
        {

        }

        public ProtectedString(string key)
        {
            this.Key = key;
        }

        public string Key { get; set; }

        public ProtectedMemoryString Value { get; set; }
    }
}
