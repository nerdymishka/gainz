using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyDerivedPasswordProvider : CompositeKeyFragment
    {

        private static readonly byte[] salt = System.Text.Encoding.UTF8.GetBytes("We all live in a ywllow marshm3llo");

        public CompositeKeyDerivedPasswordProvider(SecureString secureString, HashAlgorithm hash = null, int iterations = 10000, byte[] salt = null)
        {
            if(salt == null)
                salt = CompositeKeyDerivedPasswordProvider.salt;

            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var encoding = Encoding.UTF8;

            IntPtr bstr = IntPtr.Zero;
            char[] charArray = new char[secureString.Length];

            try
            {
                bstr = Marshal.SecureStringToBSTR(secureString);
                Marshal.Copy(bstr, charArray, 0, charArray.Length);

                var bytes = Encoding.UTF8.GetBytes(charArray);
                bytes = Pbkdf2(bytes, salt, iterations);
                bytes = hash.ComputeHash(bytes);
                this.SetData(bytes);

                bytes.Clear();
                charArray.Clear();
            }
            finally
            {
                if (createdHash)
                    hash.Dispose();

                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        public CompositeKeyDerivedPasswordProvider(byte[] password, HashAlgorithm hash = null, int iterations = 10000, byte[] salt = null)
        {
             if(salt == null)
                salt = CompositeKeyDerivedPasswordProvider.salt;

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var bytes = hash.ComputeHash(password);
            bytes = Pbkdf2(bytes, salt, iterations);
            bytes = hash.ComputeHash(bytes);
            this.SetData(bytes);

            if (createdHash)
                hash.Dispose();
        }

        public CompositeKeyDerivedPasswordProvider(string password, HashAlgorithm hash = null, int iterations = 10000, byte[] salt = null)
        {
            if(salt == null)
                salt = CompositeKeyDerivedPasswordProvider.salt;

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var bytes = Encoding.UTF8.GetBytes(password);
            bytes = Pbkdf2(bytes, salt, iterations);
            bytes = hash.ComputeHash(bytes);
            this.SetData(bytes);

            if (createdHash)
                hash.Dispose();
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
