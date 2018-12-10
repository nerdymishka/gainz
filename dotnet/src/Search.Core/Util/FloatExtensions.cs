using System;

namespace NerdyMishka.Search
{
   public static class FloatExtensions
    {
        /// <summary>
        /// Packs the <paramref name="value"/> into a single <see cref="byte"/>.
        /// </summary>
        /// <param name="value">The value to be transformed.</param>
        /// <returns>a single <see cref="byte"/>.</returns>
        public static byte ToByte(this float value)
        {
            if (value < 0.0f)
                value = 0.0f;

            // zero is a special case
            if (value == 0.0f)
                return 0;

            int bits = BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
            int mantissa = (bits & 0xFFFFFFF) >> 21;
            int exponent = (((bits >> 24) & 0x7f) - 63) + 15;

            if(exponent > 31)
            {
                exponent = 31;
                mantissa = 7;
            }

            if(exponent < 0)
            {
                exponent = 0;
                mantissa = 1;
            }

            return (byte)((exponent << 3) | mantissa);
        }
    }
}