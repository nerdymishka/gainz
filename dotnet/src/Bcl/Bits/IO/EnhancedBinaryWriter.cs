using System;
using System.IO;
using System.Text;

namespace NerdyMishka.IO
{
    public class EnhancedBinaryWriter : BinaryWriter 
    {

        public EnhancedBinaryWriter() :base() 
        {
           
        }

        public EnhancedBinaryWriter(Stream output) : base(output) 
        {
           
        }

        public EnhancedBinaryWriter(Stream output, Encoding encoding) :
            base(output, encoding) 
        {
           
        }

        public EnhancedBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) :
            base(output, encoding, leaveOpen) 
        {
           
        }
        
        public long Length
        {
            get
            {
                return this.BaseStream.Length;
            }
        }

        public long Position
        {
            get
            {
                return this.BaseStream.Position;
            }
        }

        public virtual long Seek(long offset)
        {
            return this.Seek(offset, SeekOrigin.Current);
        }

        public virtual long Seek(long offset, SeekOrigin origin)
        {
            return this.BaseStream.Seek(offset, origin);
        }

       

        public void WriteVariableLengthInt(long value)
        {
            int shift = 63;
            long b = value;
            
            do
            {
                if (shift == 0)
                    throw new FormatException();

                this.Write((byte)(b & 0x7f) | 0x80);
                b = (b >> 7);
                shift -= 7;
            } while ((b & ~0x7F) != 0);

           
            this.Write((byte)b);
        }

        public void WriteVariableLengthInt(int value)
        {
            this.Write7BitEncodedInt(value);
        }
    }   
}