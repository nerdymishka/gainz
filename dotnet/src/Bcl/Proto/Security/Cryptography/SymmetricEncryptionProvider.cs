using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{   

    /// <summary>
    /// A swappable symmetric encryption engine that defaults to encrypt and then MAC using 
    /// AES with HMACSHA256. The primary use case is for storing data on disk and providing a
    /// cross platform alternative to DPAPI and for sending messages. 
    /// </summary>
    public partial class SymmetricEncryptionProvider : ISymmetricEncryptionProvider, IDisposable, IEncryptionProvider
    {
        private ISymmetricEncryptionProviderOptions options;
        private SymmetricAlgorithm algorithm;
        private KeyedHashAlgorithm signingAlgorithm;

        public SymmetricEncryptionProvider(ISymmetricEncryptionProviderOptions options = null)
        {
            this.options = options ?? new SymmetricEncryptionProviderOptions();
        }
        
        /// <summary>
        /// Decrypts encrypted data and returns the decrypted bytes.
        /// </summary>
        /// <param name="blob">The data to encrypt.</param>
        /// <param name="privateKey">
        /// A password or phrase used to generate the key for the symmetric alogrithm. If the symetric
        /// key is stored with the message, the key for the symmetric algorithm is used instead.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public byte[] Decrypt(
            byte[] blob, 
            byte[] privateKey = null,
            IEncryptionProvider symmetricKeyEncryptionProvider = null
        ) {

            var reader = new MemoryStream(blob);
            using(var header = this.ReadHeader(reader, privateKey, symmetricKeyEncryptionProvider))
            {
                reader.Dispose();

                var algo = this.algorithm ?? Create(options);
                var messageSize = blob.Length - header.HeaderSize;
                var message = new byte[messageSize];
                Array.Copy(blob, header.Bytes.Length, message, 0, messageSize);

                if(header.Hash != null)
                {
                    var signer = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options);
                    signer.Key = header.SigningKey;
                    var h1 = header.Hash;
                    var h2 = signer.ComputeHash(message);

                    if(!EncryptionUtil.SlowEquals(h1, h2))
                        return null;
                }

                using (var decryptor2 = algo.CreateDecryptor(header.SymmetricKey, header.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor2, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    writer.Write(message);
                    cs.FlushFinalBlock();
                    ms.Flush();

                    return ms.ToArray();
                }
            }
        }

        /// <summary>
        /// Encrypts the data and returns the encrypted bytes.
        /// </summary>
        /// <param name="blob">The data to encrypt.</param>
        /// <param name="privateKey">
        ///  A password or phrase used to generate the key for the symmetric alogrithm. 
        /// </param>
        /// <param name="symmetricKey">
        ///  The key for the symmetric algorithm. If used, the private key is ignored
        ///  and the symetric key is stored with the message.
        /// </param>
        /// <param name="symmetricKeyEncryptionProvider">
        ///  The encryption provider used to encrypt/decrypt the symmetric key when it is
        ///  stored with the message.
        /// </param>
        /// <returns>Encrypted bytes.</returns>
        public byte[] Encrypt(
            byte[] blob,
            byte[] privateKey = null,
            byte[] symmetricKey = null, 
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));
        
            using(var header = this.GenerateHeader(symmetricKey, privateKey, null, symmetricKeyEncryptionProvider))
            {
                var algo = this.algorithm ?? Create(options);

                byte[] encryptedBlob = null;

                using (var encryptor = algo.CreateEncryptor(header.SymmetricKey, header.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    writer.Write(blob);
                    writer.Flush();
                    cs.Flush();
                    cs.FlushFinalBlock();
                    ms.Flush();
                    encryptedBlob = ms.ToArray();
                }

                var hash = Array.Empty<byte>();

                if(!options.SkipSigning && header.SigningKey != null)
                {
                    this.signingAlgorithm = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options);
                    this.signingAlgorithm.Key = header.SigningKey;
                    hash = this.signingAlgorithm.ComputeHash(encryptedBlob);
                    Array.Copy(hash, 0, header.Bytes, header.Position, hash.Length);
                }

                header.IV.Clear();
                header.SymmetricKey?.Clear();
                header.SigningKey?.Clear();

                using (var ms = new MemoryStream())
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(header.Bytes);
                    writer.Write(encryptedBlob);
                    writer.Flush();
                    ms.Flush();
                    return ms.ToArray();
                }
            }
        }

        private static SymmetricAlgorithm Create(ISymmetricEncryptionProviderOptions options)
        {
            if(options.SymmetricAlgorithm == SymmetricAlgorithmTypes.None)
                throw new ArgumentException("SymmetricAlgo", nameof(options));

            var algo = SymmetricAlgorithm.Create(options.SymmetricAlgorithm.ToString());
            algo.KeySize = options.KeySize;
            algo.Padding = options.Padding;
            algo.Mode = options.Mode;
            algo.Padding = options.Padding;

            return algo;
        }

        private static KeyedHashAlgorithm CreateSigningAlgorithm(ISymmetricEncryptionProviderOptions options)
        {
            if(options.KeyedHashedAlgorithm == KeyedHashAlgorithmTypes.None)
                return null;

            return KeyedHashAlgorithm.Create(options.KeyedHashedAlgorithm.ToString());
        }

        public virtual void Dispose()
        {
            this.algorithm?.Dispose();
            this.signingAlgorithm?.Dispose();
            this.options = null;
        }

        public byte[] Encrypt(byte[] data)
        {
            var privateKey = this.options.Key;
            if(privateKey == null)
                throw new ArgumentNullException("options.Key");

            return Encrypt(data, privateKey); 
        }

        public byte[] Decrypt(byte[] encryptedData)
        {
            var privateKey = this.options.Key;
            if(privateKey == null)
                throw new ArgumentNullException("options.Key");

            return Decrypt(encryptedData, privateKey); 
        }
    }
}