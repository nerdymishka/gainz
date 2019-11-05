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

        char[] ComputeHash(char[] value);

        bool Verify(string value, string hash);

        bool Verify(char[] value, char[] hash);

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

        public char[] ComputeHash(char[] value)
        {
            return ComputeHash(value, 64000);
        }

        public char[] ComputeHash(char[] value, int iterations)
        {
            byte[] salt = new byte[SaltSize];

            using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
            {
                csprng.GetBytes(salt);
            }

            byte[] password = System.Text.Encoding.UTF8.GetBytes(value);
            byte[] hash = Pbkdf2(password, salt, iterations);
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(salt);
                writer.Write(hash);
                writer.Flush();
                ms.Flush();

                var set = ms.ToArray();
                var result = new char[set.Length];
               
                Convert.ToBase64CharArray(set, 0, set.Length, result, 0);
                Array.Clear(set, 0, set.Length);

                return result;
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

        public static byte[] ComputeHash(byte[] value, byte[] salt, int iterations = 640000)
        {
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

        public bool Verify(byte[] value, byte[] hash)
        {
            return Verify(value, hash, 640000);
        }

        public bool Verify(byte[] value, byte[] hash, byte[] salt, int iterations)
        {
            byte[] actualHash = new byte[hash.Length - salt.Length];

            Array.Copy(hash, salt.Length, actualHash, 0, actualHash.Length);

            var attemptedHash = Pbkdf2(value, salt, iterations);

            if(attemptedHash.Length != actualHash.Length)
                return false;

           
            return SlowEquals(attemptedHash, actualHash);
        }


        public bool Verify(byte[] value, byte[] hash, int iterations)
        {
           
            byte[] salt = new byte[SaltSize];
            byte[] actualHash = new byte[hash.Length - SaltSize];

            Array.Copy(hash, 0, salt, 0, SaltSize);
            Array.Copy(hash, SaltSize, actualHash, 0, actualHash.Length);

            var attemptedHash = Pbkdf2(value, salt, iterations);

            if (attemptedHash.Length != actualHash.Length)
                return false;

            return SlowEquals(attemptedHash, actualHash);
        }

        public bool Verify(char[] value, char[] hash)
        {
            return Verify(value, hash, 640000);
        }
        
        public bool Verify(char[] value, char[] hash, int iterations)
        {
            var bytes = Convert.FromBase64CharArray(hash, 0, hash.Length);
            byte[] salt = new byte[SaltSize];
            byte[] actualHash = new byte[bytes.Length - SaltSize];

            Array.Copy(bytes, 0, salt, 0, SaltSize);
            Array.Copy(bytes, SaltSize, actualHash, 0, actualHash.Length);

            var password = System.Text.Encoding.UTF8.GetBytes(value);
            var attemptedHash = Pbkdf2(password, salt, iterations);

            if (attemptedHash.Length != actualHash.Length)
                return false;

            return SlowEquals(attemptedHash, actualHash);
        }

        public bool Verify(string value, string hash)
        {
            return Verify(value, hash, 640000);
        }

        public bool Verify(string value, string hash, int iterations = 640000)
        {
            var bytes = Convert.FromBase64String(hash);
            byte[] salt = new byte[SaltSize];
            byte[] actualHash = new byte[bytes.Length - SaltSize];

            Array.Copy(bytes, 0, salt, 0, SaltSize);
            Array.Copy(bytes, SaltSize, actualHash, 0, actualHash.Length);

            var attemptedHash = Pbkdf2(value, salt, iterations);

            if (attemptedHash.Length != actualHash.Length)
                return false;

            return SlowEquals(attemptedHash, actualHash);
        }

        // for timing attacks.
        public static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint)(a[i] ^ b[i]);
            }
            return diff == 0;
        }



        public static byte[] Pbkdf2(string password, byte[] salt, int iterations = 64000, int outputBytes = 32)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(outputBytes);
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
