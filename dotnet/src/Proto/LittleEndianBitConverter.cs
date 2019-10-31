using System;
using NerdyMishka.Validation;

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

        public static bool ToBoolean(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, 1);

            return value[0] == 1 ? true : false;
        }


        public static bool ToBoolean(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, 1);

            return value[startIndex] == 1 ? true : false;
        }

        public static char ToChar(byte[] value) 
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, 2);

            return (char)ToInt16(value);
        }

        
        [System.Security.SecuritySafeCritical]

        public unsafe static double ToDouble(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(double));

            var l = ToInt64(value);
            return *(double*)&l;
        }
        

        [System.Security.SecuritySafeCritical]
        public unsafe static double ToDouble(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(double));

            var l = ToInt64(value, startIndex);
            return *(double*)&l;
        }

        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(short));

            return (ushort)ToInt16(value);
        }

        [CLSCompliant(false)]
        public static ushort ToUInt16(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(short));


            return (ushort)ToInt16(value, startIndex);
        }

        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(uint));

            return (uint)ToInt32(value);
        }

        [CLSCompliant(false)]
        public static uint ToUInt32(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(uint));


            return (uint)ToInt32(value, startIndex);
        }

        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(ulong));
            
            return (ulong)ToInt64(value);
        }

        [CLSCompliant(false)]
        public static ulong ToUInt64(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(ulong));

            return (ulong)ToInt64(value, startIndex);
        }

        public static short ToInt16(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(short));

            return (short)(value[0] | 
                (value[1] << 8));
        }

        public static short ToInt16(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(short));

            return (short)(value[startIndex] | 
                    (value[++startIndex] << 8));
        }

        public static int ToInt32(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(int));

            return (
                value[0] | 
                value[1] << 8 | 
                value[2] << 16 | 
                value[3] << 24
            );
        }

        public static int ToInt32(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(int));
            
            return (
                value[startIndex] | 
                value[++startIndex] << 8 | 
                value[++startIndex] << 16 | 
                value[++startIndex] << 24
            );
        }


        public static long ToInt64(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, sizeof(long));

            int lo = 
                    (value[0]) | 
                    (value[1] << 8) | 
                    (value[2] << 16) | 
                    (value[3] << 24);
            
            int hi = 
                    (value[4]) | 
                    (value[5] << 8) | 
                    (value[6] << 16) | 
                    (value[7] << 24);

            return ((uint)lo | ((long)hi << 32));
        }

        public static long ToInt64(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, sizeof(long));

            int lo = 
                (value[startIndex]) | 
                (value[++startIndex] << 8) | 
                (value[++startIndex] << 16) | 
                (value[++startIndex] << 24);
            int hi = 
                (value[++startIndex]) | 
                (value[++startIndex] << 8) | 
                (value[++startIndex] << 16) | 
                (value[++startIndex] << 24);

            return ((uint)lo | ((long)hi << 32));
        }


        [System.Security.SecuritySafeCritical]
        public unsafe static float ToSingle(byte[] value)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Count(nameof(value), value, 4);

            int val = ToInt32(value);
            return *(float*)&val;
        }


        [System.Security.SecuritySafeCritical]
        public unsafe static float ToSingle(byte[] value, int startIndex)
        {
            Check.NotNullOrEmpty(nameof(value), value);
            Check.Slice(nameof(value), value, startIndex, 4);

            int val = ToInt32(value, startIndex);
            return *(float*)&val;
        }
    }
}