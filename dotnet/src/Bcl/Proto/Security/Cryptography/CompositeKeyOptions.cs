using System;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyOptions
    {
        public byte[] SymmetricKey { get; set; }

        public Func<byte[], byte[], long, bool> Transform { get; set; }
        
        public HashAlgorithmTypes HashAlgorithm { get; set; } = HashAlgorithmTypes.SHA256;

        public int Iterations { get; set; } = 10000;
    }
}