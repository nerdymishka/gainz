using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    internal static class Util
    {
        private static readonly List<byte[]> s_ids = new List<byte[]>();
        private static object s_syncLock = new object();
        private static RandomNumberGenerator s_rng = RandomNumberGenerator.Create();


        /// <summary>
        /// Generates a unique id.
        /// </summary>
        /// <returns>A unique id</returns>
        public static byte[] GenerateId()
        {
            lock(s_syncLock)
            {
                var iv = new byte[8];
                s_rng.GetBytes(iv);

                while(s_ids.Any(o => o.SequenceEqual(iv)))
                {
                    s_rng.GetBytes(iv);
                }

                s_ids.Add(iv);

                return iv;
            }
            
        }

        /// <summary>
        /// Converts the bytes to a 32 bit unsigned integer.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>a unsigned integer value.</returns>
        public static uint ToUInt32(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts the bytes to a 32 bit unsigned integer.
        /// </summary>
        /// <param name="bytes">The index that a read should be started from.</param>
        /// <returns>a unsigned integer value.</returns>
        public static uint ToUInt32(this byte[] bytes, int startIndex)
        {
            return BitConverter.ToUInt32(bytes, startIndex);
        }
    }
}
