using System;

namespace NerdyMishka
{
    public static class LittleEndianBitConverter
    {
        public static byte[] ToBytes(bool value)
        {
            var bytes = new byte[1];
            bytes[0] = (value ? (byte)1 : (byte)0 );
            return bytes;
        }

        public static byte[] ToBytes(char value)
        {
            return ToBytes((short)value);
        }

        public static byte[] ToBytes(double value)
        {
            unsafe
            {
                return ToBytes(*(long*)&value);
            }
        }

        public static byte[] ToBytes(short value)
        {
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }

        public static byte[] ToBytes(int value)
        {
            return new byte[]
            {
                (byte)value,
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
            };
        }

        public static byte[] ToBytes(long value)
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

        [CLSCompliant(false)]
        public static byte[] ToBytes(ushort value)
        {
            return new byte[] { (byte)value, (byte)(value >> 8) };
        }


        [CLSCompliant(false)]
        public static byte[] ToBytes(uint value)
        {
            
            return new byte[]
            {
                (byte)value,
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
            };
        }

         [CLSCompliant(false)]
        public static byte[] ToBytes(ulong value)
        {
            return new byte[]
            {
                (byte)value,
                (byte)((value >> 8) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 56) & 0xFF),
            };
        }

        public static char ToChar(byte[] value) 
        {
            return (char)ToInt16(value);
        }

        public static double ToDouble(byte[] value)
        {
            return ToInt64(value);
        }

        public static double ToDouble(byte[] value, int startIndex)
        {
            return ToInt64(value, startIndex);
        }

          [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value)
        {
            return (ushort)(value[0] | value[1] << 8);
        }

        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return (ushort)(value[startIndex] | value[++startIndex] << 8);
        }

        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value)
        {
            return (uint)ToInt32(value);
        }

        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            return (uint)ToInt32(value, startIndex);
        }

        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value)
        {
            return (ulong)ToInt64(value);
        }

        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            return (ulong)ToInt64(value, startIndex);
        }

         public static short ToInt16(byte[] value)
        {
            return (short)((value[0] << 8) | value[1]);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            return (short)((value[startIndex] << 8) | value[++startIndex]);
        }

        public static int ToInt32(byte[] value)
        {
            return (
                value[0] | 
                value[1] << 8 | 
                value[2] << 16 | 
                value[3] << 24
            );
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            return (
                value[startIndex] | 
                value[++startIndex] << 8 | 
                value[++startIndex] << 16 | 
                value[++startIndex] << 24
            );
        }


        public static long ToInt64(byte[] value)
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

        public static long ToInt64(byte[] value, int startIndex)
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