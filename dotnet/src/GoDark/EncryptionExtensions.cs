using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public static class EncryptionUtil
    {

        public static byte[] CreateOutputBuffer(byte[] inputBuffer, int blockSize)
        {
            var l = inputBuffer.Length;
            var actualBlockSize = (blockSize / 8);
            var pad = l % actualBlockSize;
            if(pad != 0)
            {
                return new byte[l + (actualBlockSize - pad)];
            }

            return new byte[l];
        }

        public static byte[] ToBytes(this SecureString secureString, Encoding encoding = null)
        {
            IntPtr bstr = IntPtr.Zero;
            char[] charArray = new char[secureString.Length];

            if(encoding == null)
                encoding = Encoding.UTF8;

            try
            {
                bstr = Marshal.SecureStringToBSTR(secureString);
                Marshal.Copy(bstr, charArray, 0, charArray.Length);

                var bytes = encoding.GetBytes(charArray);
                charArray.Clear();
                return bytes;
            }
            finally
            {
                Marshal.ZeroFreeBSTR(bstr);
            }
        }
    }
}
