
using System;
using System.IO;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class PasswordAuthenticatorOptions
    {
        public int Iterations { get; set; } = 64000;

        public byte[] Salt { get; set; } = null;
    }


    public class PasswordAuthenticator : IPasswordAuthenticator
    {
        private const int SaltSize = 32;
        private PasswordAuthenticatorOptions options;

        

        public PasswordAuthenticator(PasswordAuthenticatorOptions options = null)
        {
            this.options = options ?? new PasswordAuthenticatorOptions();
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

            byte[] hash = Pbkdf2(value, salt, this.options.Iterations);
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write((short)salt.Length);
                writer.Write(salt);
                writer.Write(hash);
                writer.Flush();
                ms.Flush();

                return ms.ToArray();
            }
        }

        public bool Verify(byte[] value, byte[] hash)
        {
            using (var ms = new MemoryStream(value))
            using (var reader = new BinaryReader(ms))
            {
                var size = reader.ReadInt16();
                var salt = reader.ReadBytes(size);
                var actualHash = reader.ReadBytes(value.Length - size);

                var attemptedHash = Pbkdf2(value, salt, this.options.Iterations);

                if (attemptedHash.Length != actualHash.Length)
                    return false;

                return EncryptionUtil.SlowEquals(attemptedHash, actualHash);
            }
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