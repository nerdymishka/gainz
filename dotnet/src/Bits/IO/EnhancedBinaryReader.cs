using System;
using System.IO;
using System.Text;


namespace NerdyMishka.IO
{
    public class EnhancedBinaryReader : BinaryReader 
    {
        public EnhancedBinaryReader(Stream input) : base(input) {
                
        }

        public EnhancedBinaryReader(Stream input, Encoding encoding) : 
            base(input, encoding) 
        {
                
        }

        public EnhancedBinaryReader(Stream input, Encoding encoding, bool leaveOpen) : 
            base(input, encoding, leaveOpen) 
        {
                
        }

        public long Length
        {
            get
            {
                return base.BaseStream.Length;
            }
        }

        public long Position
        {
            get
            {
                return base.BaseStream.Position;
            }
        }

        public int ReadVariableLengthInt32()
        {
            return this.Read7BitEncodedInt();
        }

        public long ReadVariableLengthInt64()
        {
            long count = 0;
            int shift = 0;
            byte b;
            do
            { 
                // 9 bytes shift += 7
                if (shift == 63)  
                    throw new FormatException("The variable length int for 64bit has exceeded the number of allowed bytes (9)");

                b = this.ReadByte();
                count |= (b & 0x7FL) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        public virtual long Seek(long offset)
        {
            return this.Seek(offset, SeekOrigin.Current); 
        }

        public virtual long Seek(long offset, SeekOrigin origin)
        { 
            return this.BaseStream.Seek(offset, origin);
        }

        public byte[] ToArray()
        {
            if (this.BaseStream is MemoryStream)
                return ((MemoryStream)this.BaseStream).ToArray();

            using (var ms = new MemoryStream())
            {
                this.BaseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}