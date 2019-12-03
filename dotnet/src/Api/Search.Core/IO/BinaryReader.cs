/**
 * Copyright 2016 Bad Mishka LLC
 * Based Upon Lucene from The Apache Foundation, Copyright 2004
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.IO;
using System.Text;

namespace NerdyMishka.Search.IO
{
    public class BinaryReader : System.IO.BinaryReader, IBinaryReader, ICloneable<BinaryReader>
    {
        private bool closeStream = true;
        public BinaryReader(Stream input, bool leaveOpen = false):
            base(input, new UTF8Encoding(false, false), leaveOpen)
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

        object ICloneable.Clone()
        {
            return this.Clone();
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

        public BinaryReader Clone()
        {
            var memoryStream = new MemoryStream();
            base.BaseStream.CopyTo(memoryStream);
            memoryStream.Seek(this.Position, SeekOrigin.Begin);
            return new BinaryReader(memoryStream);
        }

    

       
    }
}
