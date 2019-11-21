
using System;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class PasswordAuthenticatorOptions
    {
        public int Iterations { get; set; } = 64000;

        public short HashType { get; set; } = 1;

        public byte[] Salt { get; set; } = null;

        public Func<byte[], byte[], int, byte[]> ComputeHash { get; set; }
    }


    public class PasswordAuthenticator : IPasswordAuthenticator
    {
        private const int SaltSize = 32;
        private PasswordAuthenticatorOptions options;

        public PasswordAuthenticator(PasswordAuthenticatorOptions options = null)
        {
            this.options = options ?? new PasswordAuthenticatorOptions() {
                ComputeHash = Pbkdf2
            };
        }   

        public byte[] ComputeHash(byte[] value)
        {
            byte[] salt = this.options.Salt;
            if(salt == null)
            {
                salt = new byte[SaltSize];
                using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
                {
                    csprng.GetBytes(salt);
                }
            }

            byte[] hash = options.ComputeHash(value, salt, this.options.Iterations);
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                var size = (short)salt.Length;
                writer.Write(options.HashType);
                writer.Write(size);
                writer.Write(this.options.Iterations);
                writer.Write(salt); // 32
                writer.Write(hash); // 32 
                writer.Flush();
                ms.Flush();

                return ms.ToArray();
            }
        }

        public bool Verify(byte[] value, byte[] hash)
        {
            using (var ms = new MemoryStream(hash))
            using (var reader = new BinaryReader(ms))
            {
                // in case one wants to switch to bcrypt or another 
                // implementation later. 
                var hashType = reader.ReadInt16();
                var size = reader.ReadInt16();
                var iterations = reader.ReadInt32();
                var salt = reader.ReadBytes(size);
            
                var actualHash = reader.ReadBytes(hash.Length - (size + 8));
                var attemptedHash = options.ComputeHash(value, salt, iterations);

                return EncryptionUtil.SlowEquals(attemptedHash, actualHash);
            }
        }

        public static byte[] Pbkdf2(byte[] password, byte[] salt, int iterations)
        {
            return Pbkdf2(password, salt, iterations, 32);
        }

        public static byte[] Pbkdf2(byte[] password, byte[] salt, int iterations = 64000, int outputBytes = 32)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}