using System;

namespace NerdyMishka
{
    public static class BitShift
    {
        /// <summary>
        /// Shifts the bits in a circular fashion
        /// </summary>
        /// <param name="value">The value to shift.</param>
        /// <param name="r"></param>
        /// <returns></returns>
        [CLSCompliant(false)]
        public static ulong RotateLeft64(ulong value, short r)
        {
            return (value << r) | (value >> (64 - r));
        }

        public static long RotateLeft64(long x, short r)
        {
            return (x << r) | (x >> (64 - r));
        }

        [CLSCompliant(false)]
        public static ulong RotateLeft64(ulong x, byte r)
        {
            return (x << r) | (x >> (64 - r));
        }

        
        public static long RotateLeft64(long x, byte r)
        {
            return (x << r) | (x >> (64 - r));
        }

        [CLSCompliant(false)]
        public static uint RotateLeft32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        public static int RotateLeft32(int x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }
    }
}