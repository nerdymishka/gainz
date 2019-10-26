using Xunit;
using NerdyMishka;
using System;
using NerdyMishka.Validation;

namespace Tests
{

    public class BigEndianBitConverterTests
    {
#pragma warning disable xUnit2013
        
        [Fact]
        public static void Bytes_ToBoolean()
        {
            var tBool = BigEndianBitConverter.ToBoolean(new byte[] { 1 });
            var fBool = BigEndianBitConverter.ToBoolean(new byte[] { 0 });

            Assert.True(tBool);
            Assert.False(fBool);
        }

        [Fact]
        public static void Bytes_ToBoolean_FromSlice()
        {
            var tBool = BigEndianBitConverter.ToBoolean(new byte[] { 0, 1 }, 1);
            var fBool = BigEndianBitConverter.ToBoolean(new byte[] { 1, 0 }, 1);

            Assert.True(tBool);
            Assert.False(fBool);
        }

        [Fact]
        public static void Bytes_ToBoolean_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                  BigEndianBitConverter.ToBoolean(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                  BigEndianBitConverter.ToBoolean(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                  BigEndianBitConverter.ToBoolean(new byte[2]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                  BigEndianBitConverter.ToBoolean(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                  BigEndianBitConverter.ToBoolean(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                  BigEndianBitConverter.ToBoolean(new byte[2], 3);
            });
        }



        [Fact]
        public static void Boolean_ToBytes()
        {
            byte[] tBool = BigEndianBitConverter.ToBytes(true);
            byte[] fBool = BigEndianBitConverter.ToBytes(false);

            Assert.NotNull(tBool);
            Assert.NotNull(fBool);

            Assert.Equal(1, tBool.Length);
            Assert.Equal(1, fBool.Length);
            Assert.Equal((byte)1, tBool[0]);
            Assert.Equal((byte)0, fBool[0]);
        }

        [Fact]
        public static void Short_ToBytes()
        {
            short s = 1;
            var bytes = BigEndianBitConverter.ToBytes(s);
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[0], (byte)(s >> 8));
            Assert.Equal(bytes[1],  (byte)s);

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(bytes, BitConverter.GetBytes(s));
        }

        [Fact]
        public static void Char_ToBytes()
        {
            char c = 'x';
            var bytes = BigEndianBitConverter.ToBytes(c);
            var s = (short)c;
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[0], (byte)(s >> 8));
            Assert.Equal(bytes[1],  (byte)s);

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(bytes, BitConverter.GetBytes(s));
        }

        [Fact]
        public static void UShort_ToBytes()
        {
            ushort s = 1;
            var bytes = BigEndianBitConverter.ToBytes(s);
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[0], (byte)(s >> 8));
            Assert.Equal(bytes[1], (byte)s);

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(bytes, BitConverter.GetBytes(s));
        }

        [Fact]
        public static void Int_ToBytes()
        {
            int i = 1;
            var bytes = BigEndianBitConverter.ToBytes(i);
            Assert.NotNull(bytes);
            Assert.Equal(4, bytes.Length);
            Assert.Equal(bytes[0], (byte)(i >> 24));
            Assert.Equal(bytes[1], (byte)(i >> 16));
            Assert.Equal(bytes[2], (byte)(i >> 8));
            Assert.Equal(bytes[3], (byte)i );
           
           if(!BitConverter.IsLittleEndian)
                Assert.Equal(bytes, BitConverter.GetBytes(i));
        }

        [Fact]
        public static void UInt_ToBytes()
        {
            uint i = 1;
            var bytes = BigEndianBitConverter.ToBytes(i);
            Assert.NotNull(bytes);
            Assert.Equal(4, bytes.Length);
            Assert.Equal(bytes[0], (byte)(i >> 24));
            Assert.Equal(bytes[1], (byte)(i >> 16));
            Assert.Equal(bytes[2], (byte)(i >> 8));
            Assert.Equal(bytes[3], (byte)i );

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(bytes, BitConverter.GetBytes(i));
        }

        [Fact]
        public static void Long_ToBytes()
        {
            long l = 1L;
            var bytes = BigEndianBitConverter.ToBytes(l);
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
            Assert.Equal(bytes[0],(byte) (l >> 56));
            Assert.Equal(bytes[1],(byte) (l >> 48));
            Assert.Equal(bytes[2],(byte) (l >> 40));
            Assert.Equal(bytes[3],(byte) (l >> 32));
            Assert.Equal(bytes[4],(byte) (l >> 24));
            Assert.Equal(bytes[5],(byte) (l >> 16));
            Assert.Equal(bytes[6],(byte) (l >> 8));
            Assert.Equal(bytes[7],(byte) 1);

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(l), bytes);
        }


        [Fact]
        public static void ULong_ToBytes()
        {
            ulong l = 1L;
            var bytes = BigEndianBitConverter.ToBytes(l);
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
            Assert.Equal(bytes[0],(byte) (l >> 56));
            Assert.Equal(bytes[1],(byte) (l >> 48));
            Assert.Equal(bytes[2],(byte) (l >> 40));
            Assert.Equal(bytes[3],(byte) (l >> 32));
            Assert.Equal(bytes[4],(byte) (l >> 24));
            Assert.Equal(bytes[5],(byte) (l >> 16));
            Assert.Equal(bytes[6],(byte) (l >> 8));
            Assert.Equal(bytes[7],(byte) 1);
            
            if(!BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(l), bytes);
        }

        [Fact]
        public static void Double_ToBytes()
        {
            double d = 1.0;
            var bytes = BigEndianBitConverter.ToBytes(d);
            var bytes2 = BigEndianBitConverter.ToBytes(1L);
            long l;
            unsafe {
                l = *(long*)&d;
            }
              
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
         
            Assert.Equal(bytes[0],(byte) (l >> 56));
            Assert.Equal(bytes[1],(byte) (l >> 48));
            Assert.Equal(bytes[2],(byte) (l >> 40));
            Assert.Equal(bytes[3],(byte) (l >> 32));
            Assert.Equal(bytes[4],(byte) (l >> 24));
            Assert.Equal(bytes[5],(byte) (l >> 16));
            Assert.Equal(bytes[6],(byte) (l >> 8));
            Assert.Equal(bytes[7],(byte) l);

            if(!BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(d), bytes);
        }
#pragma warning disable xUnit2013
       
    }
}