using Xunit;
using NerdyMishka;
using System;
using NerdyMishka.Validation;
using System.Linq;

namespace Tests
{

    public class LittleEndianBitConverterTests
    {
#pragma warning disable xUnit2013
      
        [Fact]
        public static void Bytes_ToBoolean()
        {
            var tBool =LittleEndianBitConverter.ToBoolean(new byte[] { 1 });
            var fBool =LittleEndianBitConverter.ToBoolean(new byte[] { 0 });

            Assert.True(tBool);
            Assert.False(fBool);
        }

        [Fact]
        public static void Bytes_ToBoolean_FromSlice()
        {
            var tBool =LittleEndianBitConverter.ToBoolean(new byte[] { 0, 1 }, 1);
            var fBool =LittleEndianBitConverter.ToBoolean(new byte[] { 1, 0 }, 1);

            Assert.True(tBool);
            Assert.False(fBool);
        }

        [Fact]
        public static void Bytes_ToBoolean_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToBoolean(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToBoolean(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToBoolean(new byte[2]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToBoolean(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToBoolean(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToBoolean(new byte[2], 3);
            });
        }


        [Fact]
        public unsafe static void Bytes_ToDouble()
        {
            double d = 1.1;
            var l = (*(long*)&d);
            var bytes =LittleEndianBitConverter.ToBytes(d);
            var bytes2 =LittleEndianBitConverter.ToBytes(l);
            var bytes3 = BitConverter.GetBytes(d);  
            var bytes4 = BitConverter.GetBytes(l);
            if(!BitConverter.IsLittleEndian)
            {
                bytes3 = bytes3.Reverse().ToArray();
                bytes4 = bytes4.Reverse().ToArray();
            }

            Assert.Equal(bytes, bytes2);
            Assert.Equal(bytes3, bytes);
            Assert.Equal(bytes4, bytes2);
            Assert.Equal(bytes3, bytes2);

            var value =LittleEndianBitConverter.ToDouble(bytes);
            var l2 =LittleEndianBitConverter.ToInt64(bytes4);
            double d2 =BitConverter.ToDouble(bytes);
            long l3 = BitConverter.ToInt64(bytes);
        
            if(!BitConverter.IsLittleEndian)
            {
                bytes = bytes.Reverse().ToArray();
                d2 =BitConverter.ToDouble(bytes);
                l3 = BitConverter.ToInt64(bytes);
            } 
          
            Assert.Equal(l, l3);
    
            Assert.Equal(l, l2);
            Assert.Equal(d, d2);
            Assert.Equal(d, value);
        }

        [Fact]
        public unsafe static void Bytes_ToDouble_FromSlice()
        {
            double d = 1.2;
     
            var bytes = LittleEndianBitConverter.ToBytes(d);        
            var b2 = new byte[9];

            Array.Copy(bytes, 0,  b2, 1, 8);

            var d2 = LittleEndianBitConverter.ToDouble(b2, 1);
            Assert.Equal(d, d2);
        }

        [Fact]
        public static void Bytes_ToDouble_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToDouble(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToDouble(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToDouble(new byte[9]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToDouble(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToDouble(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToDouble(new byte[9], 10);
            });
        }



        [Fact]
        public static void Bytes_ToInt()
        {
            int i = 1;
            var bytes = new byte[4] {  
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToInt32(bytes);
            Assert.Equal(i, value);
        }

        [Fact]
        public static void Bytes_ToInt_FromSlice()
        {
            int i = 1;
            var bytes = new byte[5] {  
                (byte)0,
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToInt32(bytes, 1);
            Assert.Equal(value, i);
        }

        [Fact]
        public static void Bytes_ToInt_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt32(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt32(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt32(new byte[5]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt32(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt32(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt32(new byte[5], 6);
            });
        }



        [Fact]
        public static void Bytes_ToLong()
        {
            long l = 1;
            var bytes = new byte[8] {  
                (byte)(l & 0xFF),
                (byte)((l >> 8) & 0xFF),
                (byte)((l >> 16) & 0xFF),
                (byte)((l >> 24) & 0xFF),
                (byte)((l >> 32) & 0xFF),
                (byte)((l >> 40) & 0xFF),
                (byte)((l >> 48) & 0xFF),
                (byte)((l >> 56) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToInt64(bytes);
            Assert.Equal(value, l);
        }

        [Fact]
        public static void Bytes_ToLong_FromSlice()
        {

            long l = 1;
            var bytes = new byte[9] {  
                (byte)0,
                (byte)(l & 0xFF),
                (byte)((l >> 8) & 0xFF),
                (byte)((l >> 16) & 0xFF),
                (byte)((l >> 24) & 0xFF),
                (byte)((l >> 32) & 0xFF),
                (byte)((l >> 40) & 0xFF),
                (byte)((l >> 48) & 0xFF),
                (byte)((l >> 56) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToInt64(bytes, 1);
            Assert.Equal(value, l);
        }

        [Fact]
        public static void Bytes_ToLong_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt64(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt64(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt64(new byte[9]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt64(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt64(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt64(new byte[9], 10);
            });
        }

        [Fact]
        public static void Bytes_ToShort()
        {
            short s = 1;
            var bytes = new byte[2] {  
                (byte)(s & 0xFF),
                (byte)((s >> 8)& 0xFF),
                
            };

            var l =LittleEndianBitConverter.ToInt16(bytes);
            Assert.Equal(l, s);
        }

        [Fact]
        public static void Bytes_ToShort_FromSlice()
        {
            short s = 1;
            var bytes = new byte[3] {  
                (byte)0,
                (byte)(s & 0xFF),
                (byte)((s >> 8)& 0xFF),
            };

            var l =LittleEndianBitConverter.ToInt16(bytes, 1);
            Assert.Equal(s, l);
        }

        [Fact]
        public static void Bytes_ToShort_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt16(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt16(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt16(new byte[3]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt16(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToInt16(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToInt16(new byte[3], 4);
            });
        }



        [Fact]
        public unsafe static void Bytes_ToSingle()
        {
            float f = 1;
            int i = *(int*)&f;
            var bytes = new byte[4] {  
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),   
            };

            var value =LittleEndianBitConverter.ToSingle(bytes);
            Assert.Equal(value, f);
        }

        [Fact]
        public unsafe static void Bytes_ToSingle_FromSlice()
        {
            float f = 1;
            int i = *(int*)&f;
            var bytes = new byte[5] {  
                (byte)0,
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),   
            };

            var value =LittleEndianBitConverter.ToSingle(bytes, 1);
            Assert.Equal(value, f);
        }

        [Fact]
        public static void Bytes_ToSingle_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToSingle(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToSingle(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToSingle(new byte[5]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToSingle(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToSingle(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToSingle(new byte[5], 6);
            });
        }



 


        [Fact]
        public static void Bytes_ToUint()
        {
            uint i = 1;
            var bytes = new byte[4] {  
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),   
            };

            var value =LittleEndianBitConverter.ToUInt32(bytes);
            Assert.Equal(value, i);
        }

        [Fact]
        public static void Bytes_ToUint_FromSlice()
        {
            uint i = 1;
            var bytes = new byte[5] {  
                (byte)0,
                (byte)(i & 0xFF),
                (byte)((i >> 8)& 0xFF),
                (byte)((i >> 16) & 0xFF),
                (byte)((i >> 24) & 0xFF),   
            };

            var value =LittleEndianBitConverter.ToUInt32(bytes, 1);
            Assert.Equal(value, i);
        }

        [Fact]
        public static void Bytes_ToUint_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt32(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt32(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt32(new byte[5]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt32(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt32(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt32(new byte[5], 6);
            });
        }



        [Fact]
        public static void Bytes_ToULong()
        {
            ulong l = 1;
            var bytes = new byte[8] {  
                (byte)(l & 0xFF),
                (byte)((l >> 8) & 0xFF),
                (byte)((l >> 16) & 0xFF),
                (byte)((l >> 24) & 0xFF),
                (byte)((l >> 32) & 0xFF),
                (byte)((l >> 40) & 0xFF),
                (byte)((l >> 48) & 0xFF),
                (byte)((l >> 56) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToUInt64(bytes);
            Assert.Equal(value, l);
        }

        [Fact]
        public static void Bytes_ToULong_FromSlice()
        {

            ulong l = 1;
            var bytes = new byte[9] {  
                (byte)0,
                (byte)(l & 0xFF),
                (byte)((l >> 8) & 0xFF),
                (byte)((l >> 16) & 0xFF),
                (byte)((l >> 24) & 0xFF),
                (byte)((l >> 32) & 0xFF),
                (byte)((l >> 40) & 0xFF),
                (byte)((l >> 48) & 0xFF),
                (byte)((l >> 56) & 0xFF),
            };

            var value =LittleEndianBitConverter.ToUInt64(bytes, 1);
            Assert.Equal(value, l);
        }

        [Fact]
        public static void Bytes_ToULong_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt64(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt64(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt64(new byte[9]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt64(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt64(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt64(new byte[9], 10);
            });
        }


         [Fact]
        public static void Bytes_ToUShort()
        {
            ushort s = 1;
            var bytes = new byte[2] {  
                (byte)(s & 0xFF),
                (byte)((s >> 8)& 0xFF),
            };

            var l =LittleEndianBitConverter.ToUInt16(bytes);
            Assert.Equal(l, s);
        }

        [Fact]
        public static void Bytes_ToUShort_FromSlice()
        {
            ushort s = 1;
            var bytes = new byte[3] {  
                (byte)0,
                (byte)(s & 0xFF),
                (byte)((s >> 8)& 0xFF),
            };

            var l =LittleEndianBitConverter.ToUInt16(bytes, 1);
            Assert.Equal(l, s);
        }

        [Fact]
        public static void Bytes_ToUShort_Throws_Exceptions()
        {
            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt16(null);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt16(new byte[0]);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt16(new byte[3]);
            });  


            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt16(null, 1);
            }); 

            Assert.Throws<ArgumentNullOrEmptyException>(() => {
                 LittleEndianBitConverter.ToUInt16(new byte[0], 1);
            });  

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                 LittleEndianBitConverter.ToUInt16(new byte[3], 4);
            });
        }

 

        [Fact]
        public static void Boolean_ToBytes()
        {
            byte[] tBool =LittleEndianBitConverter.ToBytes(true);
            byte[] fBool =LittleEndianBitConverter.ToBytes(false);

            Assert.NotNull(tBool);
            Assert.NotNull(fBool);

            Assert.Equal(1, tBool.Length);
            Assert.Equal(1, fBool.Length);
            Assert.Equal((byte)1, tBool[0]);
            Assert.Equal((byte)0, fBool[0]);
        }

        [Fact]
        public static void Char_ToBytes()
        {
            char c = 'x';
            var bytes =LittleEndianBitConverter.ToBytes(c);
            var s = (short)c;
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[0],  (byte)s);
            Assert.Equal(bytes[1],  (byte)(s >> 8));

            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(s), bytes);
            else 
                Assert.Equal(BitConverter.GetBytes(s).Reverse(), bytes);
        }


        [Fact]
        public static void Double_ToBytes()
        {
            double d = 1.0;
            var bytes =LittleEndianBitConverter.ToBytes(d);
            var bytes2 =LittleEndianBitConverter.ToBytes(1L);
            long l;
            unsafe {
                l = *(long*)&d;
            }
              
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
         
            Assert.Equal(bytes[7],(byte) (l >> 56));
            Assert.Equal(bytes[6],(byte) (l >> 48));
            Assert.Equal(bytes[5],(byte) (l >> 40));
            Assert.Equal(bytes[4],(byte) (l >> 32));
            Assert.Equal(bytes[3],(byte) (l >> 24));
            Assert.Equal(bytes[2],(byte) (l >> 16));
            Assert.Equal(bytes[1],(byte) (l >> 8));
            Assert.Equal(bytes[0],(byte) l);

            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(d), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(d).Reverse(), bytes); 
        }



        [Fact]
        public static void Int_ToBytes()
        {
            int i = 1;
            var bytes =LittleEndianBitConverter.ToBytes(i);
            Assert.NotNull(bytes);
            Assert.Equal(4, bytes.Length);
            Assert.Equal(bytes[3], (byte)(i >> 24));
            Assert.Equal(bytes[2], (byte)(i >> 16));
            Assert.Equal(bytes[1], (byte)(i >> 8));
            Assert.Equal(bytes[0], (byte)i );
           
            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(i), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(i).Reverse(), bytes); 
        }

        [Fact]
        public static void Long_ToBytes()
        {
            long l = 1L;
            var bytes =LittleEndianBitConverter.ToBytes(l);
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
            Assert.Equal(bytes[7],(byte) (l >> 56));
            Assert.Equal(bytes[6],(byte) (l >> 48));
            Assert.Equal(bytes[5],(byte) (l >> 40));
            Assert.Equal(bytes[4],(byte) (l >> 32));
            Assert.Equal(bytes[3],(byte) (l >> 24));
            Assert.Equal(bytes[2],(byte) (l >> 16));
            Assert.Equal(bytes[1],(byte) (l >> 8));
            Assert.Equal(bytes[0],(byte) 1);

            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(l), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(l).Reverse(), bytes); 
            
        }       

  

        [Fact]
        public static void Short_ToBytes()
        {
            short s = 1;
            var bytes =LittleEndianBitConverter.ToBytes(s);
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[1], (byte)(s >> 8));
            Assert.Equal(bytes[0],  (byte)s);

            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(s), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(s).Reverse(), bytes); 
        }




        [Fact]
        public static void UInt_ToBytes()
        {
            uint i = 1;
            var bytes =LittleEndianBitConverter.ToBytes(i);
            Assert.NotNull(bytes);
            Assert.Equal(4, bytes.Length);
            Assert.Equal(bytes[3], (byte)(i >> 24));
            Assert.Equal(bytes[2], (byte)(i >> 16));
            Assert.Equal(bytes[1], (byte)(i >> 8));
            Assert.Equal(bytes[0], (byte)i );

           if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(i), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(i).Reverse(), bytes); 
        }




        [Fact]
        public static void ULong_ToBytes()
        {
            ulong l = 1L;
            var bytes =LittleEndianBitConverter.ToBytes(l);
            Assert.NotNull(bytes);
            Assert.Equal(8, bytes.Length);
            Assert.Equal(bytes[7],(byte) (l >> 56));
            Assert.Equal(bytes[6],(byte) (l >> 48));
            Assert.Equal(bytes[5],(byte) (l >> 40));
            Assert.Equal(bytes[4],(byte) (l >> 32));
            Assert.Equal(bytes[3],(byte) (l >> 24));
            Assert.Equal(bytes[2],(byte) (l >> 16));
            Assert.Equal(bytes[1],(byte) (l >> 8));
            Assert.Equal(bytes[0],(byte) 1);
            
            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(l), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(l).Reverse(), bytes); 
        }

        [Fact]
        public static void UShort_ToBytes()
        {
            ushort s = 1;
            var bytes =LittleEndianBitConverter.ToBytes(s);
            Assert.NotNull(bytes);
            Assert.Equal(2, bytes.Length);
            Assert.Equal(bytes[1], (byte)(s >> 8));
            Assert.Equal(bytes[0], (byte)s);

            if(BitConverter.IsLittleEndian)
                Assert.Equal(BitConverter.GetBytes(s), bytes);
            else
                Assert.Equal(BitConverter.GetBytes(s).Reverse(), bytes); 
        }


 


     
#pragma warning disable xUnit2013
       
    }
}