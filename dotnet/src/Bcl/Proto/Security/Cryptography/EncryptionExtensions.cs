using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;
using System.Text;
using NerdyMishka.Text;
using NerdyMishka.Validation;

namespace NerdyMishka.Security.Cryptography
{
    public static class EncryptionUtil
    {

        public static bool SlowEquals(IList<byte> left, IList<byte> right)
        {
            uint diff = (uint)left.Count ^ (uint)left.Count;
            for (int i = 0; i < left.Count; i++)
            {
                diff |= (uint)(left[i] ^ right[i]);
            }

            return diff == 0;
        }

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
            Check.NotNull(nameof(secureString), secureString);

            IntPtr bstr = IntPtr.Zero;
            char[] charArray = new char[secureString.Length];
            encoding = encoding ?? Encodings.Utf8NoBom;
         

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
