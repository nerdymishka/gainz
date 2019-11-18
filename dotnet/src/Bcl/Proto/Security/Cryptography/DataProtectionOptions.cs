

using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
      public class DataProtectionOptions : IDataProtectionOptions
    {
        public int KeySize { get; set; } = 256;

        public int BlockSize { get; set; } = 128;

        public CipherMode Mode { get; set; } = CipherMode.CBC;

        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;

        public string SymmetricAlgorithm { get; set; } = "AES";

        public string KeyedHashedAlgorithm { get; set; } = "HMACSHA256";


        public int SaltSize { get; set; } = 64;

        public int Iterations { get; set; } = 10000;

        public int MinimumPrivateKeyLength { get; set; } = 12;

        public bool SkipSigning { get; set; } = false;

#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Key { get; set; }

        public byte[] SigningKey { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }

}