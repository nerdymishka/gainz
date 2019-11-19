using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public partial class SymmetricEngine : IDisposable
    {
        private ISymmetricEngineOptions options;
        private SymmetricAlgorithm algorithm;
        private KeyedHashAlgorithm signingAlgorithm;

        public SymmetricEngine(ISymmetricEngineOptions options = null)
        {
            this.options = options ?? new SymmetricEngineOptions();
        }

        public enum AlgorithmType : short 
        {
            AES = 0,
            AESGCM = 1,
            AESCCM = 2,
        }

        public enum SigningAlgorithmType : short 
        {
            None = 0,
            HMACSHA256 = 1
        }


        public byte[] DecryptBlob(
            byte[] blob, 
            RSA rsa) {
            return DecryptBlob(blob, null, new RsaSymmetricKeyDecryptor(rsa));
        }

        
        public byte[] DecryptBlob(
            byte[] blob, 
            CompositeKey key
        ) {
            var privateKey = key.AssembleKey();
            var result = DecryptBlob(blob, privateKey);
            privateKey.Clear();
            return result;
        }
        
        public byte[] DecryptBlob(
            byte[] blob, 
            byte[] privateKey
        ) {
            return DecryptBlob(blob, privateKey);
        }


        protected byte[] DecryptBlob(
            byte[] blob, 
            byte[] privateKey = null,
            ISymmetricKeyDecryptor decryptor2 = null
        ) {

            var reader = new MemoryStream(blob);
            using(var header = this.ReadHeader(reader, privateKey, decryptor2))
            {
                reader.Dispose();

                var algo = this.algorithm ?? Create(options);
                var messageSize = blob.Length - header.HeaderSize;
                var message = new byte[messageSize];
                Array.Copy(blob, 0, message, 0, messageSize);

                if(header.Hash != null)
                {
                    var signer = this.signingAlgorithm ?? CreateSigningAlgorithm(this.options);
                    signer.Key = header.SigningKey;
                    var h1 = header.Hash;
                    var h2 = signer.ComputeHash(message);

                    if(!EncryptionUtil.SlowEquals(h1, h2))
                        return null;
                }

                using (var decryptor = algo.CreateDecryptor(header.SymmetricKey, header.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    writer.Write(message);
                    cs.FlushFinalBlock();
                    ms.Flush();

                    return ms.ToArray();
                }
            }
        }


        public byte[] EncryptBlob(
            byte[] blob,
            RSA rsa)
        {
            var symmetricKey = PasswordGenerator.GenerateAsBytes(options.KeySize / 8);
            return EncryptBlob(blob, null, symmetricKey, new RsaSymmetricKeyDecryptor(rsa));
        }

        public byte[] EncryptBlob(
            byte[] blob,
            CompositeKey compositeKey)
        {
            var privateKey = compositeKey.AssembleKey();
            return EncryptBlob(blob, privateKey);
        }

        public ReadOnlySpan<byte>  EncryptBlob( 
            ReadOnlySpan<byte> blob,
            CompositeKey compositeKey)
        {
            var privateKey = compositeKey.AssembleKey();
            return EncryptBlob(blob, privateKey);
        }


        public ReadOnlySpan<byte> EncryptBlob(
            ReadOnlySpan<byte> blob,
            byte[] privateKey = null)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

        
            using(var header = this.GenerateHeader(privateKey, null, null))
            {
                var algo = this.algorithm ?? Create(options);

                byte[] encryptedBlob = null;

                using (var encryptor = algo.CreateEncryptor(header.SymmetricKey, header.IV))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    
                    #if NETSTANDARD2_0
                        writer.Write(blob.ToArray());
                    #else 
                        writer.Write(blob);
                    #endif 

                  
                    writer.Flush();
                    cs.Flush();
                    cs.FlushFinalBlock();
                    ms.Flush();

                    encryptedBlob = ms.ToArray();
                }

                var hash = Array.Empty<byte>();

                if(!options.SkipSigning && options.SigningKey != null)
                {
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


    

        protected byte[] EncryptBlob(
            byte[] blob,
            byte[] privateKey = null,
            byte[] symmetricKey = null, 
            ISymmetricKeyDecryptor decryptor = null)
        {
            if (blob == null)
                throw new ArgumentNullException(nameof(blob));
        
            using(var header = this.GenerateHeader(symmetricKey, privateKey, null, decryptor))
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

                if(!options.SkipSigning && options.SigningKey != null)
                {
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

        private static SymmetricAlgorithm Create(ISymmetricEngineOptions options)
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

        private static KeyedHashAlgorithm CreateSigningAlgorithm(ISymmetricEngineOptions options)
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
    }
}