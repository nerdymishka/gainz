using System;
using System.IO;
using System.Text;

namespace NerdyMishka.Search.IO 
{
    public class BinaryWriter : System.IO.BinaryWriter, 
        IBinaryWriter,
        ICloneable
    {
      

     

        public BinaryWriter(Stream stream)
            :base(stream, new UTF8Encoding(true, true))
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

        public override void Write(byte value)
        {
            base.Write(value);
        }

        public override void Write(byte[] buffer, int index, int count)
        {
            base.Write(buffer, index, count);
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

        public BinaryWriter Clone()
        {

            var memoryStream = new MemoryStream();
            base.BaseStream.CopyTo(memoryStream);
            memoryStream.Seek(this.Position, SeekOrigin.Begin);
            return new BinaryWriter(memoryStream);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}