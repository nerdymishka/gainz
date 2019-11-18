using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public partial class SymmetricEngine
    {
        public interface ISymmetricKeyDecryptor
        {
            byte[] Encrypt(byte[] bytes);

            byte[] Decrypt(byte[] bytes);
        }

        public class RsaSymmetricKeyDecryptor : ISymmetricKeyDecryptor
        {
            private RSA rsa;
            public RsaSymmetricKeyDecryptor(RSA rsa)
            {
                this.rsa = rsa;
            }
            public byte[] Decrypt(byte[] bytes)
            {
                return this.rsa.Decrypt(bytes, RSAEncryptionPadding.Pkcs1);
            }

            public byte[] Encrypt(byte[] bytes)
            {
                return this.rsa.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
            }
        }
        protected class Header : IDisposable
        {
            public virtual short Version { get; } = 1;

            public int MetaDataSize { get; set; }

            public short SymmetricKeySize { get; set; } 

            public short SymmetricSaltSize { get; set; } 

            public short SigningSaltSize { get; set; }

            public short IvSize { get; set; }

            public short HashSize { get; set; }

            public AlgorithmType AlgorithmType { get; set; }

            public SigningAlgorithmType SigningAlgorithmType { get; set; }

            public byte[] SymmetricKey { get; set; }

            public byte[] SigningKey { get; set; }

            public byte[] IV { get; set; }

            public long Position { get; set; } = 0;

            public byte[] Bytes { get; set; }

            public byte[] Hash { get; set; }

            public virtual int HeaderSize => 0;

            public void Dispose()
            {
                this.Hash?.Clear();
                this.Bytes?.Clear();
                this.SymmetricKey?.Clear();
                this.SigningKey?.Clear();
            }
        }

        protected class HeaderV1 : Header
        {
           
            public override int HeaderSize => 
                (1 * 4) + // ints
                (8 * 2) + // shorts
                this.MetaDataSize +
                this.SymmetricKeySize +
                this.SymmetricSaltSize + 
                this.SigningSaltSize + 
                this.IvSize + 
                this.HashSize;
        }

        protected static byte[] GenerateSalt(int length)
        {
            using (var rng = new RandomNumberGenerator())
            {
                return rng.NextBytes(length);
            }
        }


        protected Header ReadHeader(
            Stream reader,
            byte[] privateKey = null,
            ISymmetricKeyDecryptor decryptor = null
        ) 
        {
            var signingKey = this.options.SigningKey;
            using(var br = new BinaryReader(reader))
            {
                var version = br.ReadInt32();
                Header header = null;
                switch(version)
                {
                    case 1:
                    default: 
                        header = new HeaderV1();
                    break;
                }

                // header  ints
                // 1. version
                // 2. algo
                // 3. signing,
                // 2. metadataSize
                // 3. symmetricSaltSize
                // 4. signingSaltSize
                // 5. ivSize
                // 6. symmetricKeySize
                // 7. hashSize

                // header values
                // 1. metadata
                // 2. symmetricSalt
                // 3. signingSalt
                // 4. iv
                // 5. symmetricKey
                // 6. hash

                header.AlgorithmType = (AlgorithmType)br.ReadInt16();
                header.SigningAlgorithmType = (SigningAlgorithmType)br.ReadInt16();
                header.MetaDataSize = br.ReadInt32();
                header.SymmetricSaltSize = br.ReadInt16();
                header.SigningSaltSize = br.ReadInt16();
                header.IvSize = br.ReadInt16();
                header.SymmetricKeySize = br.ReadInt16();
                header.HashSize = br.ReadInt16();

                if(this.options.SymmetricAlgorithm != header.AlgorithmType.ToString())
                {
                    this.options.SymmetricAlgorithm = header.AlgorithmType.ToString();
                    this.algorithm = null;
                }

                if(this.options.KeyedHashedAlgorithm != header.SigningAlgorithmType.ToString())
                {
                    this.options.KeyedHashedAlgorithm = header.SigningAlgorithmType.ToString();
                    this.signingAlgorithm = null;
                }

               

                byte[] metadata = null;
                byte[] symmetricSalt = null;
                byte[] signingSalt = null;
                byte[] iv = null;
                byte[] symmetricKey = null;
                byte[] hash = null;

                if(header.MetaDataSize > 0)
                    metadata = br.ReadBytes(header.MetaDataSize);
                
                if(header.SymmetricSaltSize > 0)
                    symmetricSalt = br.ReadBytes(header.SymmetricSaltSize);

                if(header.SigningSaltSize > 0)
                    signingSalt = br.ReadBytes(header.SigningSaltSize);

                if(header.IvSize > 0)
                    iv = br.ReadBytes(header.IvSize);

                if(header.SymmetricKeySize > 0)
                    symmetricKey = br.ReadBytes(header.SymmetricKeySize);

                if(header.HashSize > 0)
                    hash = br.ReadBytes(header.HashSize);

                header.Position = reader.Position;

                if(decryptor != null)
                    symmetricKey = decryptor.Decrypt(symmetricKey);

                if(symmetricKey == null && privateKey == null)
                    throw new ArgumentNullException(nameof(privateKey), 
                        "privateKey or symmetricKey must have a value");

                if(!options.SkipSigning && privateKey == null && signingKey == null)
                    throw new ArgumentNullException(nameof(privateKey), 
                        "privateKey must have a value or options.SigningKey must have a value or options.SkipSigning must be true");


                

                if(symmetricKey == null)
                {
                    if(symmetricSalt == null)
                        throw new InvalidOperationException("symmetricSalt for the privateKey could not be retrieved");

                    using (var generator = new Rfc2898DeriveBytes(privateKey, symmetricSalt, options.Iterations))
                    {
                        header.SymmetricKey = generator.GetBytes(options.KeySize / 8);
                    }                    
                }

                if(!options.SkipSigning && signingKey == null)
                {
                    if(signingSalt == null)
                        throw new InvalidOperationException("symmetricSalt for the privateKey could not be retrieved");

                    var key = symmetricKey ?? privateKey;

                    using (var generator = new Rfc2898DeriveBytes(key, signingSalt, options.Iterations))
                    {
                        header.SigningKey = generator.GetBytes(options.KeySize / 8);
                    }       
                }

                header.SymmetricKey = header.SymmetricKey ?? symmetricKey;
                header.IV = iv;
                header.Hash = hash;

                return header;
            }
        }

        protected Header GenerateHeader(
            byte[] symmetricKey = null,
            byte[] privateKey = null,
            byte[] metadata = null,
            ISymmetricKeyDecryptor decryptor = null)
        {
            privateKey = privateKey ?? options.Key;
            var signingKey = options.SigningKey;

            // header  ints
            // 1. version
            // 2. metadataSize
            // 3. symmetricSaltSize
            // 4. signingSaltSize
            // 5. ivSize
            // 6. symmetricKeySize
            // 7. hashSize

            // header values
            // 1. metadata
            // 2. symmetricSalt
            // 3. signingSalt
            // 4. iv
            // 5. symmetricKey
            // 6. hash

            metadata = metadata ?? Array.Empty<byte>();
            var header = new HeaderV1();
            header.MetaDataSize = metadata.Length;

            if(symmetricKey == null && privateKey == null)
                throw new ArgumentNullException(nameof(privateKey), "privateKey or symmetricKey must have a value");

            if(!options.SkipSigning && privateKey == null && signingKey == null)
                throw new ArgumentNullException(nameof(privateKey), 
                    "privateKey must have a value or options.SigningKey must have a value or options.SkipSigning must be true");

            

            if(privateKey != null)
            {
                header.SymmetricSaltSize = (short)(options.SaltSize / 8);
                
                if(!options.SkipSigning && signingKey == null)
                {
                    header.SigningSaltSize = (short)(options.SaltSize / 8);   
                    this.signingAlgorithm = this.signingAlgorithm ?? CreateSigningAlgorithm(options);
                }
            }

            if(symmetricKey != null)
            {
                header.SymmetricKeySize = (short)(options.KeySize / 8);
            }

          

            this.algorithm = this.algorithm ?? Create(this.options);
            this.algorithm.GenerateIV();
            var iv = this.algorithm.IV;
            header.IvSize = (short)iv.Length;
            header.HashSize = (short)(this.signingAlgorithm.HashSize / 8);
            header.Bytes = new byte[header.HeaderSize];
            using(var ms = new MemoryStream(header.Bytes))
            using(var bw = new BinaryWriter(ms))
            {
                if(symmetricKey != null && decryptor != null)
                {
                    symmetricKey = decryptor.Encrypt(symmetricKey);
                    header.SymmetricKeySize  = (short)symmetricKey.Length;
                }
                Enum.TryParse(options.SymmetricAlgorithm, out AlgorithmType algorithmType);
                Enum.TryParse(options.KeyedHashedAlgorithm, out SigningAlgorithmType signingAlgorithmType);
                header.AlgorithmType = algorithmType;
                header.SigningAlgorithmType = signingAlgorithmType;
                
                bw.Write(header.Version);
                bw.Write((short)algorithmType);
                bw.Write((short)signingAlgorithmType);
                bw.Write(header.MetaDataSize);
                bw.Write(header.SigningSaltSize);
                bw.Write(header.SigningSaltSize);
                bw.Write(header.IvSize);
                bw.Write(header.SymmetricKeySize);
                bw.Write(header.HashSize);

                if (privateKey != null)
                {
                    var symmetricSalt = GenerateSalt(header.SymmetricSaltSize);

                    using (var generator = new Rfc2898DeriveBytes(privateKey, symmetricSalt, options.Iterations))
                    {
                        symmetricSalt = generator.Salt;
                        header.SymmetricKey = generator.GetBytes(options.KeySize / 8);

                        bw.Write(symmetricSalt, 0, symmetricSalt.Length);
                    }

                    symmetricSalt.Clear();

                    if (!options.SkipSigning || signingKey != null)
                    {
                        var signingSalt = GenerateSalt(header.SigningSaltSize);

                        using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                        {
                            signingSalt = generator.Salt;
                            header.SigningKey = generator.GetBytes(options.KeySize / 8);
                            bw.Write(signingSalt, 0, signingSalt.Length);
                            
                        }

                        signingSalt.Clear();
                    }
                }

                bw.Write(iv);
                if(symmetricKey != null)
                {
                    bw.Write(symmetricKey);
                }
                
                bw.Flush();
                ms.Flush();
                header.Position = ms.Position;
            }
               
            return header;
        }

    }
}