
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Storeage
{
    public class RamStream : System.IO.Stream
    {
        private static readonly object s_lock = new object();

        private bool canRead;
        private bool canWrite;
        private long length;

        private RamFile file;
        

        public RamStream() {
            this.file = new RamFile();
        }

        public RamStream(int blockSize) {
            this.file = new RamFile(blockSize);
        }

        public RamStream(
            RamFile file, 
            bool canRead = true, 
            bool canWrite = true) {
            this.file = file;
            this.canRead = true;
            this.canWrite = true;
        }

        public RamStream(RamFile file) {
            this.file = file;
            this.canWrite = true;
        }

        public override long Length => this.file.Length;

        public override long Position { get; set; } = 0;

        public override bool CanRead => this.canRead;

        public override bool CanWrite => this.canWrite;


        protected long BlockPosition => this.Position / this.file.BlockSize;

        protected long BlockOffset => this.Position % this.file.BlockSize;

        protected byte[] Block
        {
            get
            {
                while (this.file.BlockCount <= this.BlockPosition)
                    this.file.AllocateNewBuffer();

                return this.file[(int)this.BlockPosition];
            }
        }

        public override bool CanSeek => throw new NotImplementedException();

        public override void Flush() { }


        public override int Read(byte[] buffer, int offset, int count)
        {
            if(!this.canRead)
                throw new AccessViolationException("Stream is not readable");

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", count, "Number of bytes must be positive");

            if (buffer == null)
                throw new ArgumentNullException("buffer", "Buffer cannot be null.");
            
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset",offset,"Destination offset cannot be negative.");
            

            long remaining = Math.Min(count,  (this.Length - Position));
            int read = 0;
            long copysize = 0;
            long blockSize = this.file.BlockSize;
            do
	        {
                copysize = Math.Min(remaining, (blockSize - this.BlockOffset));
                Buffer.BlockCopy(this.Block, (int)this.BlockOffset, buffer, offset, (int)copysize);
                remaining -= copysize;
                offset += (int)copysize;

                read += (int)copysize;
                Position += copysize;

	        } while (remaining > 0);

            return read;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return Task.Run(() => this.Read(buffer, offset, count), cancellationToken);
        }

        public override int ReadByte()
        {
            if(!this.canRead)
                throw new AccessViolationException("Stream is not readable");
            
            if (this.Position >= this.file.Length)
                return -1;

            byte b = this.Block[this.BlockOffset];
            Position++;

            return b;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length - offset;
                    break;
            }
            return this.Position;
        }

        public override void SetLength(long value)
        {
            this.file.Length = value;
        }

      


        public override void Write(byte[] buffer, int offset, int count)
        {
            if(!this.canWrite)
                throw new AccessViolationException("Stream is not writeable.");

            long initialPosition = Position;
            int copysize;
            try
            {
                do
                {
                    copysize = Math.Min(count, (int)(this.file.BlockCount - this.BlockOffset));

                    this.EnsureCapacity(Position + copysize);

                    Buffer.BlockCopy(buffer, (int)offset, this.Block, (int)this.BlockOffset, copysize);
                    count -= copysize;
                    offset += copysize;

                    this.Position += copysize;

                } while (count > 0);
            }
            catch
            {
                this.Position = initialPosition;
                throw;
            }
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, 
            CancellationToken cancellationToken = default(CancellationToken)) {

             return Task.Run(() => this.Write(buffer, offset, count), cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            if(!this.canWrite)
                throw new AccessViolationException("Stream is not writeable.");

            this.EnsureCapacity(this.Position + 1);
            this.Block[this.BlockOffset] = value;
            this.Position++;
        }

    

        protected void EnsureCapacity(long length) 
        {
            if(this.file.Length < length)
                this.file.Length = length;
        }

        
    }
}