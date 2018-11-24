using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public interface IDataProtectionOptions
    {
        int KeySize { get; set; }
        int BlockSize { get; set; }

        CipherMode Mode { get; set; }

        PaddingMode Padding { get; set; }

        int SaltSize { get; set; }

        int Iterations { get; set; }

        int MinimumPrivateKeyLength { get; set; }

        bool SkipSigning { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        byte[] Key { get; set; }

        byte[] SigningKey { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }

    public class DataProtectionOptions : IDataProtectionOptions
    {
        public int KeySize { get; set; } = 256;

        public int BlockSize { get; set; } = 128;

        public CipherMode Mode { get; set; } = CipherMode.CBC;

        public PaddingMode Padding { get; set; } = PaddingMode.PKCS7;

        public int SaltSize { get; set; } = 64;

        public int Iterations { get; set; } = 10000;

        public int MinimumPrivateKeyLength { get; set; } = 12;

        public bool SkipSigning { get; set; } = false;

#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Key { get; set; }

        public byte[] SigningKey { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
    }

    public static class DataProtection
    {

        public static IDataProtectionOptions Options { get; set; }


        public static Func<IDataProtectionOptions, SymmetricAlgorithm> CreateSymmetricAlgorithm { get; set; }

        public static Func<KeyedHashAlgorithm> CreateSigningAlorithm { get; set; }

        static DataProtection()
        {
            Options = new DataProtectionOptions();

            CreateSymmetricAlgorithm = (options) =>
            {
                var aes = Aes.Create();
                aes.KeySize = options.KeySize;
                aes.Padding = options.Padding;
                aes.Mode = options.Mode;
                aes.Padding = options.Padding;

                return aes;
            };

            // HMACSHA256.Create() throws operation not supported
            CreateSigningAlorithm = () => new HMACSHA256();
        }

        private static byte[] GenerateSalt(int length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return bytes;
            }
        }

        public static bool DecryptStream(Stream reader, Stream writer, RSA rsa, IDataProtectionOptions options = null) 
        {   
            options = options ?? Options;

            var sb = new System.Text.StringBuilder();
            char end = ';';
            char c = Char.MinValue;
            while((c = (char)reader.ReadByte()) != end) {
                sb.Append(c);
            }

            var docMeta = sb.ToString();
            
            sb.Clear();
            var parts = docMeta.Split(',');
            int headerLength = 0,
                hashLength = 0,
                metaLength = 0,
                ivLength = 0,
                docMetaLength = Encoding.UTF8.GetByteCount(docMeta);

            foreach(var part in parts)
            {
                var kv = part.Split('=');
                if(kv.Length != 2)
                {
                    throw new ArgumentException("invalid header");
                }

                switch(kv[0])
                {
                    case "header":
                        headerLength = int.Parse(kv[1]);
                    break;
                    case "meta":
                        metaLength = int.Parse(kv[1]);
                        break;
                    case "hash":
                        hashLength = int.Parse(kv[1]);
                        break;

                    case "iv":
                        ivLength = int.Parse(kv[1]);
                        break;
                    default:
                    break;
                }
            }

            var meta = new byte[metaLength]; // SHA256 padded.
            var hash = new byte[hashLength];
            var iv = new byte[ivLength];
            var header = new byte[headerLength];
            byte[] decryptKey = null;

            
            reader.Read(header, 0, header.Length);
            reader.Read(iv, 0, iv.Length);


            int symmetricKeyLength = options.KeySize / 8,
                signingSaltLength = options.SaltSize / 8,
                headerIndex = metaLength;

            var signingKey = options.SigningKey;

            Array.Copy(header, 0, meta, 0, metaLength);
 
#if NET45
            decryptKey = ((RSACryptoServiceProvider)rsa).Decrypt(meta, true);
#else 
            decryptKey = rsa.Decrypt(meta, RSAEncryptionPadding.Pkcs1);
#endif 

            var symmetricKey = new byte[symmetricKeyLength];

            Array.Copy(header, headerIndex, symmetricKey, 0, symmetricKeyLength);
            headerIndex += symmetricKeyLength;

               
            byte[] signingSalt = new byte[signingSaltLength];
            if (!options.SkipSigning)
            {
                Array.Copy(header, headerIndex, signingSalt, 0, signingSaltLength);
                using (var generator = new Rfc2898DeriveBytes(decryptKey, signingSalt, options.Iterations))
                {
                    signingKey = generator.GetBytes(options.KeySize / 8);
                }

                headerIndex += signingSaltLength;
            }

            var position = reader.Position;
            var remaining = reader.Length - (hashLength + ivLength + headerLength + docMetaLength);

            var buffer = new byte[2048];
            long bytesRemaining = reader.Length - (position + hashLength);
            //if(remaining != bytesRemaining)
            //    throw new Exception($"position {bytesRemaining} != math {remaining}");

            int capacity = 0;

            
            reader.Position = (reader.Length - hashLength);

            reader.Read(hash, 0, hashLength);

            reader.Position = position;

            var hmac = CreateSigningAlorithm();
            hmac.Key = signingKey;
    

            using(var ms = new MemoryStream())
            using(var hmacStream = new CryptoStream(ms, hmac, CryptoStreamMode.Write))
            using(var bw = new BinaryWriter(hmacStream, Encoding.UTF8, true))
            {
                while(bytesRemaining > 0)
                {
                    capacity = (int)Math.Min(bytesRemaining, 2048);
                    var count = reader.Read(buffer, 0, capacity);
                    bytesRemaining -= count;

                    bw.Write(buffer, 0, capacity);
                }

                bw.Flush();
                hmacStream.Flush();
                hmacStream.FlushFinalBlock();
            }

             if(signingKey != null && signingKey.Length > 0)
                Array.Clear(signingKey, 0, signingKey.Length);

            var h2 = hmac.Hash;

            if(h2.Length != hash.Length)
                throw new Exception($"hash length doesn't match computed: {h2.Length} stored: {hash.Length}");

            int compare = 0;
            for (int i = 0; i < hash.Length; i++)
            {
                compare = compare | (hash[i] ^ h2[i]);

                if(compare != 0)
                    throw new Exception($"{hash[i]} != {h2[i]} at position {i}");
            }

            if (compare != 0)
                return false;

            using(var alg = CreateSymmetricAlgorithm(options))
            {
                buffer = new byte[2048];
                capacity = 0;
                reader.Position = position;
                bytesRemaining = reader.Length - (position + hashLength);

                using(var cryptoStream = new CryptoStream(writer, alg.CreateDecryptor(decryptKey, iv), CryptoStreamMode.Write))
                using(var bw = new BinaryWriter(cryptoStream, Encoding.UTF8, true))
                {
                    while(bytesRemaining > 0)
                    {
                        capacity = (int)Math.Min(bytesRemaining, 2048);
                        var count = reader.Read(buffer, 0, capacity);
                        bytesRemaining -= count;

                        bw.Write(buffer, 0, capacity);
                    }

                    bw.Flush();
                    cryptoStream.FlushFinalBlock();
                }

                Array.Clear(decryptKey, 0, decryptKey.Length);

                if(symmetricKey != null && symmetricKey.Length > 0)
                    Array.Clear(symmetricKey, 0, symmetricKey.Length);

            }

            return true;
        }

        public static bool EncryptStream(Stream reader, Stream writer, RSA rsa, CompositeKey compositeKey = null, IDataProtectionOptions options = null) {

            byte[] metaInfo = null;

            if(compositeKey == null)
            {
                compositeKey = new CompositeKey();
                compositeKey.AddPassword(PasswordGenerator.GenerateAsBytes(30));
            }

            options = options ?? Options;

            var symmetricKey = options.Key;
            if(symmetricKey == null )
                symmetricKey = PasswordGenerator.GenerateAsBytes(options.KeySize / 8);

            var privateKey = compositeKey.AssembleKey(symmetricKey, options.Iterations);

#if NET45
            metaInfo = ((RSACryptoServiceProvider)rsa).Encrypt(privateKey, true);
#else 
            metaInfo = rsa.Encrypt(privateKey, RSAEncryptionPadding.Pkcs1);
#endif 
          
            if(compositeKey.Count < 1)
                throw new ArgumentException("compositeKey requires at least one key fragment");

            var signingKey = options.SigningKey;


            int headerIndex = 0,
                symmetricKeyLength = options.KeySize / 8,
                signingSaltLength = options.SaltSize / 8;

            int headerSize = metaInfo.Length + symmetricKeyLength + signingSaltLength;
            byte[] header = new byte[headerSize];

            Array.Copy(metaInfo, 0, header, 0, metaInfo.Length);
            headerIndex += metaInfo.Length;

            using (var alg = CreateSymmetricAlgorithm(Options))
            {
                alg.GenerateIV();
                var iv = alg.IV;

                Array.Copy(symmetricKey, 0, header, headerIndex, symmetricKeyLength);
                headerIndex += symmetricKeyLength;

                if(!options.SkipSigning)
                {
                    var signingSalt = GenerateSalt(signingSaltLength);
                    using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                    {
                        //signingSalt = generator.Salt;

                        signingKey = generator.GetBytes(options.KeySize / 8);
                        Array.Copy(signingSalt, 0, header, headerIndex, signingSaltLength);
                    }
                }

                long bytesRemaining = reader.Length;
                int bufferSize = 2048,
                    capacity = 0;
                var hmac = CreateSigningAlorithm();
                hmac.Key = signingKey;
                using(var bw = new BinaryWriter(writer, Encoding.UTF8, true))
                {
                    var meta =  $"meta={metaInfo.Length},header={header.Length},hash={hmac.HashSize / 8},iv={iv.Length};".ToCharArray();
                    
                    // write file header   
                    bw.Write(meta);
                    bw.Write(header);
                    bw.Write(iv);
                    bw.Flush();
                }

                using (var encryptor = alg.CreateEncryptor(privateKey, iv))
                using (var hmacStream = new CryptoStream(writer, hmac, CryptoStreamMode.Write))
                using (var cs = new CryptoStream(hmacStream, encryptor, CryptoStreamMode.Write))
                using (var bw = new BinaryWriter(cs, Encoding.UTF8, true))
                {
                    hmac.Key = signingKey;
                    
                   
                    var buffer = new byte[2048];
                 
                    while(bytesRemaining > 0L)
                    {
                        capacity = (int)Math.Min(bufferSize, bytesRemaining);
                        
                        var count= reader.Read(buffer, 0, capacity);

                        bytesRemaining -= count;

                        bw.Write(buffer, 0, capacity);
                    }
                  
                    bw.Flush();
                    cs.Flush();

                    cs.FlushFinalBlock();
                    
                    writer.Write(hmac.Hash, 0, hmac.Hash.Length);
                    hmac.Dispose();
                }
            }

            return true;
        }

        public static byte[] DecryptBlob(
            byte[] encryptedBlob,
            byte[] privateKey = null,
            IDataProtectionOptions options = null,
            byte[] metaInfo = null
        )
        {
            options = options ?? new DataProtectionOptions();

            if (encryptedBlob == null)
                throw new ArgumentNullException(nameof(encryptedBlob));

            if (privateKey != null && privateKey.Length < options.MinimumPrivateKeyLength)
                throw new ArgumentOutOfRangeException(nameof(privateKey),
                    "privateKey must be meet the length requirements of " + options.MinimumPrivateKeyLength);


#if NETSTANDARD2_0 || NET461
            if (metaInfo == null)
                metaInfo = Array.Empty<byte>();
#else 
            if(metaInfo == null)
                metaInfo = new byte[0];
#endif
            int symmetricSaltLength = 0,
                signingSaltLength = 0;

            var symmetricKey = options.Key;
            var signingKey = options.SigningKey;


            if (privateKey != null)
            {
                symmetricSaltLength = signingSaltLength = options.SaltSize / 8;
                byte[] symmetricSalt = new byte[symmetricSaltLength];
                byte[] signingSalt = new byte[signingSaltLength];

                Array.Copy(encryptedBlob, metaInfo.Length, symmetricSalt, 0, symmetricSaltLength);
                if (!options.SkipSigning)
                {
                    Array.Copy(encryptedBlob, metaInfo.Length + symmetricSaltLength, signingSalt, 0, symmetricSaltLength);
                }


                using (var generator = new Rfc2898DeriveBytes(privateKey, symmetricSalt, options.Iterations))
                {
                    symmetricKey = generator.GetBytes(options.KeySize / 8);
                }

                if (!options.SkipSigning)
                {
                    using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                    {
                        signingKey = generator.GetBytes(options.KeySize / 8);
                    }
                }

            }
            else
            {
                if (symmetricKey == null)
                    throw new ArgumentException(nameof(symmetricKey));

                if (signingKey == null)
                    throw new ArgumentException(nameof(signingKey));
            }

            int ivSize = Options.BlockSize / 8,
                hashLength = 0;

            if (!options.SkipSigning)
            {
                using (var hmac = CreateSigningAlorithm())
                {
                    hmac.Key = signingKey;
                    var hash = new byte[hmac.HashSize / 8];
                    var messageSize = encryptedBlob.Length - hash.Length;
                    var message = new byte[messageSize];
                    hashLength = hash.Length;

                    Array.Copy(encryptedBlob, 0, message, 0, messageSize);

                    var computedHash = hmac.ComputeHash(message);


                    if (encryptedBlob.Length < (hash.Length + metaInfo.Length + ivSize))
                    {
                        return null;
                    }

                    Array.Copy(encryptedBlob, encryptedBlob.Length - hash.Length, hash, 0, hash.Length);

                    int compare = 0;
                    for (int i = 0; i < hash.Length; i++)
                    {
                        compare = compare | (hash[i] ^ computedHash[i]);
                    }

                    if (compare != 0)
                        return null;
                }
            }

            using (var alg = CreateSymmetricAlgorithm(Options))
            {
                var iv = new byte[ivSize];
                var headerLength = (metaInfo.Length + symmetricSaltLength + signingSaltLength);
                Array.Copy(encryptedBlob, headerLength, iv, 0, ivSize);

                headerLength += ivSize;

                using (var decryptor = alg.CreateDecryptor(symmetricKey, iv))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    writer.Write(encryptedBlob, headerLength, encryptedBlob.Length - headerLength - hashLength);
                    cs.FlushFinalBlock();
                    ms.Flush();


                    Array.Clear(iv, 0, iv.Length);
                    if (options.Key == null)
                        Array.Clear(symmetricKey, 0, symmetricKey.Length);

                    if (options.SigningKey == null)
                        Array.Clear(signingKey, 0, signingKey.Length);

                    symmetricKey = null;
                    signingKey = null;

                    return ms.ToArray();
                }
            }
        }

        public static string EncryptString(
            string value,
            byte[] privateKey = null,
            IDataProtectionOptions options = null,
            byte[] metaInfo = null,
            Encoding encoding = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (encoding == null)
                encoding = Encoding.UTF8;

            var blob = EncryptBlob(encoding.GetBytes(value), privateKey, options, metaInfo);
            return Convert.ToBase64String(blob);
        }

        public static string DecryptString(
            string encryptedString,
            byte[] privateKey = null,
            IDataProtectionOptions options = null,
            byte[] metaInfo = null,
            Encoding encoding = null
        )
        {
            if (string.IsNullOrWhiteSpace(encryptedString))
                return null;

            if (encoding == null)
                encoding = Encoding.UTF8;

            var encryptedBlob = Convert.FromBase64String(encryptedString);
            var blob = DecryptBlob(encryptedBlob, privateKey, options, metaInfo);
            if (blob == null)
                return null;
            return encoding.GetString(blob);
        }

        public static byte[] DecryptBlob(
            byte[] encryptedBlob, 
            CompositeKey compositeKey, 
            IDataProtectionOptions options = null, 
            byte[] metaInfo = null
        ) {
            options = options ?? new DataProtectionOptions();

            if (encryptedBlob == null)
                throw new ArgumentNullException(nameof(encryptedBlob));

            if (compositeKey == null)
                throw new ArgumentNullException(nameof(compositeKey));

            if(compositeKey.Count < 1)
                throw new ArgumentException("compositeKey requires at least one key fragment");

#if NETSTANDARD2_0 || NET461
            if (metaInfo == null)
                metaInfo = Array.Empty<byte>();
#else 
            if(metaInfo == null)
                metaInfo = new byte[0];
#endif
            int symmetricKeyLength = options.KeySize / 8,
                signingSaltLength = options.SaltSize / 8,
                headerIndex = metaInfo.Length;

            var signingKey = options.SigningKey;

            var symmetricKey = new byte[symmetricKeyLength];
            Array.Copy(encryptedBlob, headerIndex, symmetricKey, 0, symmetricKeyLength);
            headerIndex += symmetricKeyLength;

            var privateKey = compositeKey.AssembleKey(symmetricKey, options.Iterations);
               
            byte[] signingSalt = new byte[signingSaltLength];
            if (!options.SkipSigning)
            {
                Array.Copy(encryptedBlob, headerIndex, signingSalt, 0, signingSaltLength);
                using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                {
                    signingKey = generator.GetBytes(options.KeySize / 8);
                }

                headerIndex += signingSaltLength;
            }


            int ivSize = Options.BlockSize / 8,
                hashLength = 0;

            if (!options.SkipSigning)
            {
                using (var hmac = CreateSigningAlorithm())
                {
                    hmac.Key = signingKey;
                    
                    var hash = new byte[hmac.HashSize / 8];
                     hashLength = hash.Length;

                    var messageSize = encryptedBlob.Length - hashLength;
                    var message = new byte[messageSize];
                   
                    // header + iv  + encrypted message
                    Array.Copy(encryptedBlob, 0, message, 0, messageSize);

                    var computedHash = hmac.ComputeHash(message);


                    if (encryptedBlob.Length < (hashLength + headerIndex + ivSize))
                    {
                        return new byte[0];
                    }

                    Array.Copy(encryptedBlob, message.Length, hash, 0, hash.Length);

                    int compare = 0;
                    for (int i = 0; i < hash.Length; i++)
                    {
                        compare = compare | (hash[i] ^ computedHash[i]);
                        if(compare != 0){
                            return new byte[2] { (byte)i, 0x0 };
                        }
                    }

                    if (compare != 0)
                        return new byte[1];
                }
            }

            using (var alg = CreateSymmetricAlgorithm(Options))
            {
                var iv = new byte[ivSize];
               
                Array.Copy(encryptedBlob, headerIndex, iv, 0, ivSize);
                headerIndex += ivSize;

                using (var decryptor = alg.CreateDecryptor(privateKey, iv))
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                using (var writer = new BinaryWriter(cs))
                {
                    writer.Write(encryptedBlob, headerIndex, encryptedBlob.Length - headerIndex - hashLength);
                    cs.FlushFinalBlock();
                    ms.Flush();


                    Array.Clear(iv, 0, iv.Length);
                    if (options.Key == null)
                        Array.Clear(symmetricKey, 0, symmetricKey.Length);

                    Array.Clear(privateKey, 0, privateKey.Length);

                    if (options.SigningKey == null)
                        Array.Clear(signingKey, 0, signingKey.Length);

                    symmetricKey = null;
                    signingKey = null;

                    return ms.ToArray();
                }
            }
        }

        public static byte[] EncryptBlob(
            byte[] blob, 
            CompositeKey compositeKey, 
            IDataProtectionOptions options = null, 
            byte[] metaInfo = null) {

            options = options ?? new DataProtectionOptions();

            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

            if (compositeKey == null)
                throw new ArgumentNullException(nameof(compositeKey));

            if(compositeKey.Count < 1)
                throw new ArgumentException("compositeKey requires at least one key fragment");

            var signingKey = options.SigningKey;

#if NETSTANDARD2_0 || NET461
            if (metaInfo == null)
                metaInfo = Array.Empty<byte>();
#else 
            if(metaInfo == null)
                metaInfo = new byte[0];
#endif
            int headerIndex = 0,
                symmetricKeyLength = options.KeySize / 8,
                signingSaltLength = options.SaltSize / 8;

            int headerSize =metaInfo.Length + symmetricKeyLength + signingSaltLength;
            byte[] header = new byte[headerSize];

            Array.Copy(metaInfo, 0, header, 0, metaInfo.Length);
            headerIndex += metaInfo.Length;

            var symmetricKey = options.Key;
            if(symmetricKey == null )
                symmetricKey = PasswordGenerator.GenerateAsBytes(options.KeySize / 8);

            var privateKey = compositeKey.AssembleKey(symmetricKey, options.Iterations);

            using (var alg = CreateSymmetricAlgorithm(Options))
            {
                alg.GenerateIV();
                var iv = alg.IV;

                Array.Copy(symmetricKey, 0, header, headerIndex, symmetricKeyLength);
                headerIndex += symmetricKeyLength;

                if(!options.SkipSigning)
                {
                    var signingSalt = GenerateSalt(signingSaltLength);
                    using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                    {
                        //signingSalt = generator.Salt;

                        signingKey = generator.GetBytes(options.KeySize / 8);
                        Array.Copy(signingSalt, 0, header, headerIndex, signingSaltLength);
                    }
                }

                byte[] encryptedBlob = null;

                using (var encryptor = alg.CreateEncryptor(privateKey, iv))
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

                using (var hmac = CreateSigningAlorithm())
                {
                    hmac.Key = signingKey;
                    using (var ms = new MemoryStream())
                    using (var writer = new BinaryWriter(ms))
                    {
                        writer.Write(header);
                        writer.Write(iv);
                        writer.Write(encryptedBlob);


                        if (!options.SkipSigning)
                        {
                            writer.Flush();
                            ms.Flush();
                            var data = ms.ToArray();
                            var hash = hmac.ComputeHash(data);
                            writer.Write(hash);
                        }


                        writer.Flush();
                        ms.Flush();

                        Array.Clear(iv, 0, iv.Length);
                        if (options.Key == null)
                            Array.Clear(symmetricKey, 0, symmetricKey.Length);

                        Array.Clear(privateKey, 0, privateKey.Length);

                        if (options.SigningKey == null)
                            Array.Clear(signingKey, 0, signingKey.Length);

                        symmetricKey = null;
                        signingKey = null;
                        return ms.ToArray();
                    }
                }
            }
        }


        public static byte[] EncryptBlob(
            byte[] blob,
            byte[] privateKey = null,
            IDataProtectionOptions options = null,
            byte[] metaInfo = null)
        {
            options = options ?? new DataProtectionOptions();

            if (blob == null)
                throw new ArgumentNullException(nameof(blob));

            if (privateKey != null && privateKey.Length < options.MinimumPrivateKeyLength)
                throw new ArgumentOutOfRangeException(nameof(privateKey),
                    "privateKey must be meet the length requirements of " + options.MinimumPrivateKeyLength);

            var symmetricKey = options.Key;
            var signingKey = options.SigningKey;

#if NETSTANDARD2_0 || NET461
            if (metaInfo == null)
                metaInfo = Array.Empty<byte>();
#else 
            if(metaInfo == null)
                metaInfo = new byte[0];
#endif
            int headerIndex = 0;
            int headerSize = ((options.SaltSize / 8) * 2) + metaInfo.Length;
            byte[] header = new byte[headerSize];

            Array.Copy(metaInfo, header, metaInfo.Length);
            headerIndex = metaInfo.Length;

            using (var alg = CreateSymmetricAlgorithm(Options))
            {
                alg.GenerateIV();
                var iv = alg.IV;

                if (privateKey != null)
                {
                    var symmetricSalt = GenerateSalt(options.SaltSize / 8);

                    using (var generator = new Rfc2898DeriveBytes(privateKey, symmetricSalt, options.Iterations))
                    {
                        symmetricSalt = generator.Salt;
                        symmetricKey = generator.GetBytes(Options.KeySize / 8);

                        Array.Copy(symmetricSalt, 0, header, headerIndex, symmetricSalt.Length);
                        headerIndex += symmetricSalt.Length;
                    }

                    Array.Clear(symmetricSalt, 0, symmetricSalt.Length);


                    if (!options.SkipSigning)
                    {
                        var signingSalt = GenerateSalt(options.SaltSize / 8);

                        using (var generator = new Rfc2898DeriveBytes(privateKey, signingSalt, options.Iterations))
                        {
                            signingSalt = generator.Salt;

                            signingKey = generator.GetBytes(Options.KeySize / 8);
                            Array.Copy(signingSalt, 0, header, headerIndex, signingSalt.Length);
                        }

                        Array.Clear(signingSalt, 0, signingSalt.Length);
                    }
                }
                else
                {
                    if (symmetricKey == null)
                        throw new ArgumentException("SymmetricKey is null");

                    if (signingKey == null)
                        throw new ArgumentException("SigningKey is null");
                }

                byte[] encryptedBlob = null;

                using (var encryptor = alg.CreateEncryptor(symmetricKey, iv))
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

                using (var hmac = CreateSigningAlorithm())
                {
                    hmac.Key = signingKey;
                    using (var ms = new MemoryStream())
                    using (var writer = new BinaryWriter(ms))
                    {
                        writer.Write(header);
                        writer.Write(iv);
                        writer.Write(encryptedBlob);


                        if (!options.SkipSigning)
                        {
                            writer.Flush();
                            ms.Flush();
                            var data = ms.ToArray();
                            var hash = hmac.ComputeHash(data);
                            writer.Write(hash);
                        }


                        writer.Flush();
                        ms.Flush();

                        Array.Clear(iv, 0, iv.Length);
                        if (options.Key == null)
                            Array.Clear(symmetricKey, 0, symmetricKey.Length);

                        if (options.SigningKey == null)
                            Array.Clear(signingKey, 0, signingKey.Length);

                        symmetricKey = null;
                        signingKey = null;
                        return ms.ToArray();
                    }
                }
            }
        }
    }
}
