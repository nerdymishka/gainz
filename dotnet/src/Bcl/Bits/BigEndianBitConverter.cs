using System;

namespace NerdyMishka
{
    public static class BigEndianBitConverter
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
            return new byte[] { (byte)(value >> 8), (byte) value };
        }

        public static byte[] ToBytes(int value)
        {
            return new byte[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static byte[] ToBytes(long value)
        {
            return new byte[]
            {
                (byte)((value >> 56) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        [CLSCompliant(false)]
        public static byte[] ToBytes(ushort value)
        {
            return new byte[] { (byte)(value >> 8), (byte)value };
        }

        [CLSCompliant(false)]
        public static byte[] ToBytes(uint value)
        {
            return new byte[]
            {
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        [CLSCompliant(false)]
        public static byte[] ToBytes(ulong value)
        {
            return new byte[]
            {
                (byte)((value >> 56) & 0xFF),
                (byte)((value >> 48) & 0xFF),
                (byte)((value >> 40) & 0xFF),
                (byte)((value >> 32) & 0xFF),
                (byte)((value >> 24) & 0xFF),
                (byte)((value >> 16) & 0xFF),
                (byte)((value >> 8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        public static char ToChar(byte[] value) 
        {
            return (char)ToInt16(value);
        }

        public static char ToChar(byte[] value, int startIndex) 
        {
            return (char)ToInt16(value, startIndex);
        }

        public static bool ToBoolean(byte[] value)
        {
            return value[0] == 0 ? false : true;
        }

        public static bool ToBoolean(byte[] value, int startIndex)
        {
            return value[startIndex] == 0 ? false : true;
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
            return (ushort)(value[0] << 8 | value[1]);
        }

        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            return (ushort)(value[startIndex] << 8 | value[++startIndex]);
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
            return (short)(value[0] << 8 | value[1]);
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            return (short)(value[startIndex] << 8 | value[++startIndex]);
        }


        public static int ToInt32(byte[] value)
        {
            return (
                value[0] << 24 | 
                value[1] << 16 | 
                value[2] << 8 | 
                value[3]
            );
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            return (
                value[startIndex] << 24 | 
                value[++startIndex] << 16 | 
                value[++startIndex] << 8 | 
                value[++startIndex]
             );
        }


        public static long ToInt64(byte[] value)
        {
            return (
                value[0] << 56 | 
                value[1] << 48 | 
                value[2] << 40 | 
                value[3] << 42 |
                value[4] << 24 | 
                value[5] << 16 | 
                value[6] << 8 | 
                value[7]
            );
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            return (
                value[startIndex] << 56 | 
                value[++startIndex] << 48 | 
                value[++startIndex] << 40 | 
                value[++startIndex] << 42 |
                value[++startIndex] << 24 | 
                value[++startIndex] << 16 | 
                value[++startIndex] << 8 | 
                value[++startIndex]
            );
        }
    }
}