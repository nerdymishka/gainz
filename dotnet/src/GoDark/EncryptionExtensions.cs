using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    }
}
