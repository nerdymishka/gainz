using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{

    public interface IPasswordAuthenticator
    {
        byte[] ComputeHash(byte[] value);
        string ComputeHash(string value);

        bool Verify(string value, string hash);
        bool Verify(byte[] value, byte[] hash);
    }

    public class PasswordAuthenticator : IPasswordAuthenticator
    {
        private const int SaltSize = 32;

        public byte[] ComputeHash(byte[] value)
        {
            return ComputeHash(value, 64000);
        }

        public byte[] ComputeHash(byte[] value, int iterations)
        {
            byte[] salt = new byte[SaltSize];

            using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
            {
                csprng.GetBytes(salt);
            }


            byte[] hash = Pbkdf2(value, salt, iterations);
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(salt);
                writer.Write(hash);
                writer.Flush();
                ms.Flush();

                return ms.ToArray();
            }
        }

        public string ComputeHash(string value)
        {
            return ComputeHash(value, 64000);
        }

        public string ComputeHash(string value, int iterations)
        {
            byte[] salt = new byte[SaltSize];

            using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
            {
                csprng.GetBytes(salt);
            }


            byte[] hash = Pbkdf2(value, salt, iterations);
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(salt);
                writer.Write(hash);
                writer.Flush();
                ms.Flush();

                return Convert.ToBase64String(ms.ToArray());
            }
        }


        public bool Verify(byte[] value, byte[] hash)
        {
           
            byte[] salt = new byte[SaltSize];
            byte[] actualHash = new byte[hash.Length - SaltSize];

            Array.Copy(hash, 0, salt, 0, SaltSize);
            Array.Copy(hash, SaltSize - 1, actualHash, 0, actualHash.Length);

            var attemptedHash = Pbkdf2(value, salt);

            if (attemptedHash.Length != actualHash.Length)
                return false;

            return SlowEquals(attemptedHash, actualHash);
        }

        public bool Verify(string value, string hash)
        {
            var bytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[SaltSize];
            byte[] actualHash = new byte[bytes.Length - SaltSize];

            Array.Copy(bytes, 0, salt, 0, SaltSize);
            Array.Copy(bytes, SaltSize, actualHash, 0, actualHash.Length);

            var attemptedHash = Pbkdf2(value, salt);

            if (attemptedHash.Length != actualHash.Length)
                return false;

            return SlowEquals(attemptedHash, actualHash);
        }

        // for timing attacks.
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }



        private static byte[] Pbkdf2(string password, byte[] salt, int iterations = 64000, int outputBytes = 32)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }

        private static byte[] Pbkdf2(byte[] password, byte[] salt, int iterations = 64000, int outputBytes = 32)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
            }
        }
    }
}
