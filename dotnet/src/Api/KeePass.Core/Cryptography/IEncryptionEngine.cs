using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public interface IEncryptionEngine
    {
        byte[] Id { get;  }

        Stream CreateCryptoStream(Stream stream, bool encrypt, byte[] key, byte[] iv);
    }
}
