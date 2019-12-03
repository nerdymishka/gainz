using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public interface IRandomByteGeneratorEngine
    {
        int Id { get; }

        void Initialize(byte[] key);

        byte[] NextBytes(int count);
    }
}
