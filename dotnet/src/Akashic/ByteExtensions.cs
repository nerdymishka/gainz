

namespace NerdyMishka.Data
{
    public static class ByteExtensions
    {

        public static byte[] ToBytes(this long value)
        {
            return new byte[]
            {
                (byte)(value & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 56) & 0xFF),
            };
        }
        public static long ToInt64(this byte[] value)
        {
            return (
                value[0] | 
                value[1] << 8 | 
                value[2] << 16 | 
                value[3] << 24 |
                value[4] << 32 | 
                value[5] << 40 | 
                value[6] << 48 | 
                value[7] << 56
            );
        }

        public static long ToInt64(this byte[] value, int startIndex)
        {
            return (
                value[startIndex] | 
                value[++startIndex] << 8 | 
                value[++startIndex] << 16 | 
                value[++startIndex] << 24 |
                value[++startIndex] << 32 | 
                value[++startIndex] << 40 | 
                value[++startIndex] << 48 | 
                value[++startIndex] << 56
            );
        }
    }
}