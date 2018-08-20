using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyPasswordProvider : CompositeKeyFragment
    {

        public CompositeKeyPasswordProvider(SecureString secureString, HashAlgorithm hash = null)
        {
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

                this.SetData(hash.ComputeHash(bytes));

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

        public CompositeKeyPasswordProvider(byte[] password, HashAlgorithm hash = null)
        {
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
            this.SetData(bytes);

            if (createdHash)
                hash.Dispose();
        }

        public CompositeKeyPasswordProvider(string password, HashAlgorithm hash = null)
        {
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
            bytes = hash.ComputeHash(bytes);
            this.SetData(bytes);

            if (createdHash)
                hash.Dispose();
        }

    }
}
