
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{

    public interface ISymmetricEncryptionProviderOptions
    {
        int KeySize { get; set; }
        int BlockSize { get; set; }

        CipherMode Mode { get; set; }

        PaddingMode Padding { get; set; }

        SymmetricAlgorithmTypes SymmetricAlgorithm { get; set; }

        KeyedHashAlgorithmTypes KeyedHashedAlgorithm { get; set; }

        int SaltSize { get; set; }

        int Iterations { get; set; }

        int MinimumPrivateKeyLength { get; set; }

        bool SkipSigning { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        byte[] Key { get; set; }

        byte[] SigningKey { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }
}