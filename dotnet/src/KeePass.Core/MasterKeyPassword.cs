using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class MasterKeyPassword : MasterKeyFragment
    {

        public MasterKeyPassword(SecureString secureString)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));

            var encoding = Encoding.UTF8;

            IntPtr bstr = IntPtr.Zero;
            char[] charArray = new char[secureString.Length];

            try
            {

                bstr = Marshal.SecureStringToBSTR(secureString);
                Marshal.Copy(bstr, charArray, 0, charArray.Length);

                var bytes = Encoding.UTF8.GetBytes(charArray);
                this.SetData(bytes.ToSHA256Hash());

                bytes.Clear();
                charArray.Clear();
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }

        public MasterKeyPassword(byte[] password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            var bytes = password.ToSHA256Hash();
            this.SetData(bytes);
        }

        public MasterKeyPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (password.Length == 0)
                throw new ArgumentException("password must be greater than 0 characters");

            var bytes = Encoding.UTF8.GetBytes(password).ToSHA256Hash();
            this.SetData(bytes);
        }
    }
}
