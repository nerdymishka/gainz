using NerdyMishka.KeePass.Cryptography;
using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Xml
{
    public class SerializerContext
    {

        public SerializerContext()
        {
            this.Binaries = new List<BinaryMapping>();
        }

        public IRandomByteGeneratorEngine RandomByteGenerator { get; set; }

        public MemoryProtection MemoryProtection { get; set; }

        public byte DatabaseCompression { get; set; }

        public List<BinaryMapping> Binaries { get; set; }
        public IDictionary<Type, Type> Mappings { get; internal set; }
    }
}
