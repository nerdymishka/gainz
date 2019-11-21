using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using NerdyMishka.Text;

namespace NerdyMishka.Security.Cryptography
{
    public partial class SymmetricEncryptionProvider
    {
        

        public class RsaSymmetricKeyDecryptor : IEncryptionProvider
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
        internal protected class Header : IDisposable
        {
            public virtual short Version { get; } = 1;

            public int MetaDataSize { get; set; }

            public short SymmetricKeySize { get; set; } 

            public short SymmetricSaltSize { get; set; } 

            public short SigningSaltSize { get; set; }

            public short IvSize { get; set; }

            public short HashSize { get; set; }

            public SymmetricAlgorithmTypes SymmetricAlgorithmType { get; set; }

            public KeyedHashAlgorithmTypes KeyedHashAlgorithmType { get; set; }

            public byte[] SymmetricKey { get; set; }

            public byte[] SigningKey { get; set; }

            public byte[] IV { get; set; }

            public int Iterations { get; set; }

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

        internal protected class HeaderV1 : Header
        {
           
            public override int HeaderSize => 
                (2 * 4) + // ints
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


        internal protected Header ReadHeader(
            Stream reader,
            byte[] privateKey = null,
            IEncryptionProvider symmetricKeyEncryptionProvider = null
        ) 
        {
            var signingKey = this.options.SigningKey;
            using(var ms = new MemoryStream())
            using(var bw = new BinaryWriter(ms, Encodings.Utf8NoBom, true))
            using(var br = new BinaryReader(reader, Encodings.Utf8NoBom, true))
            {
                var version = br.ReadInt16();
                bw.Write(version);
                Header header = null;
                switch(version)
                {
                    case 1:
                    default: 
                        header = new HeaderV1();
                    break;
                }

                // header shorts/ints
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

                header.SymmetricAlgorithmType = (SymmetricAlgorithmTypes)br.ReadInt16();
                header.KeyedHashAlgorithmType = (KeyedHashAlgorithmTypes)br.ReadInt16();
                header.MetaDataSize = br.ReadInt32();
                header.Iterations = br.ReadInt32();
                header.SymmetricSaltSize = br.ReadInt16();
                header.SigningSaltSize = br.ReadInt16();
                header.IvSize = br.ReadInt16();
                header.SymmetricKeySize = br.ReadInt16();
                header.HashSize = br.ReadInt16();

                bw.Write((short)header.SymmetricAlgorithmType);
                bw.Write((short)header.KeyedHashAlgorithmType);
                bw.Write(header.MetaDataSize);
                bw.Write(header.Iterations);
                bw.Write(header.SymmetricSaltSize);
                bw.Write(header.SigningSaltSize);
                bw.Write(header.IvSize);
                bw.Write(header.SymmetricKeySize);
                bw.Write(header.HashSize);

                if(this.options.SymmetricAlgorithm != header.SymmetricAlgorithmType)
                {
                    this.options.SymmetricAlgorithm = header.SymmetricAlgorithmType;
                    this.algorithm = null;
                }

                if(this.options.KeyedHashedAlgorithm != header.KeyedHashAlgorithmType)
                {
                    this.options.KeyedHashedAlgorithm = header.KeyedHashAlgorithmType;
                    this.signingAlgorithm = null;
                }

               

                byte[] metadata = null;
                byte[] symmetricSalt = null;
                byte[] signingSalt = null;
                byte[] iv = null;
                byte[] symmetricKey = null;
                byte[] hash = null;

                if(header.MetaDataSize > 0)
                {
                    metadata = br.ReadBytes(header.MetaDataSize);
                    bw.Write(metadata);
                }
                    
                
                if(header.SymmetricSaltSize > 0)
                {
                    symmetricSalt = br.ReadBytes(header.SymmetricSaltSize);
                    bw.Write(symmetricSalt);
                }
                   

            
                if(header.SigningSaltSize > 0)
                {
                    signingSalt = br.ReadBytes(header.SigningSaltSize);
                    bw.Write(signingSalt);
                }
                   

                if(header.IvSize > 0)
                {
                    iv = br.ReadBytes(header.IvSize);
                    bw.Write(iv);
                }
                    

                if(header.SymmetricKeySize > 0)
                {
                    symmetricKey = br.ReadBytes(header.SymmetricKeySize);
                    bw.Write(symmetricKey);
                }
                    

                if(header.HashSize > 0)
                {
                    hash = br.ReadBytes(header.HashSize);
                    bw.Write(hash);
                }

                bw.Flush();
                ms.Flush();
                header.Bytes = ms.ToArray();
                   

                header.Position = reader.Position;

                if(symmetricKeyEncryptionProvider != null)
                    symmetricKey = symmetricKeyEncryptionProvider.Decrypt(symmetricKey);

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

                    using (var generator = new Rfc2898DeriveBytes(privateKey, symmetricSalt, header.Iterations))
                    {
                        header.SymmetricKey = generator.GetBytes(options.KeySize / 8);
                    }                    
                }

                if(!options.SkipSigning && signingKey == null)
                {
                    if(signingSalt == null)
                        throw new InvalidOperationException("symmetricSalt for the privateKey could not be retrieved");

                    var key = symmetricKey ?? privateKey;

                    using (var generator = new Rfc2898DeriveBytes(key, signingSalt, header.Iterations))
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

        internal protected Header GenerateHeader(
            byte[] symmetricKey = null,
            byte[] privateKey = null,
            byte[] metadata = null,
            IEncryptionProvider symmetricKeyEncryptionProvider = null)
        {
            privateKey = privateKey ?? options.Key;
            var signingKey = options.SigningKey;

            // header values
            // 1. version
            // 2. metadataSize
            // 3. iterations
            // 4. symmetricSaltSize
            // 5. signingSaltSize
            // 6. ivSize
            // 7. symmetricKeySize
            // 8. hashSize

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
            header.IV = iv;
            header.HashSize = (short)(this.signingAlgorithm.HashSize / 8);
            header.Bytes = new byte[header.HeaderSize];
            header.Iterations = options.Iterations;
            using(var ms = new MemoryStream(header.Bytes))
            using(var bw = new BinaryWriter(ms, Encodings.Utf8NoBom, false))
            {
                if(symmetricKey != null && symmetricKeyEncryptionProvider != null)
                {
                    symmetricKey = symmetricKeyEncryptionProvider.Encrypt(symmetricKey);
                    header.SymmetricKeySize  = (short)symmetricKey.Length;
                }
               
                header.SymmetricAlgorithmType = options.SymmetricAlgorithm;
                header.KeyedHashAlgorithmType = options.KeyedHashedAlgorithm;




                bw.Write(header.Version);
                bw.Write((short)header.SymmetricAlgorithmType);
                bw.Write((short)header.KeyedHashAlgorithmType);
                bw.Write(header.MetaDataSize);
                bw.Write(header.Iterations);
                bw.Write(header.SymmetricSaltSize);
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