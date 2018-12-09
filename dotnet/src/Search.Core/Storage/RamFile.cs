using System;
using System.Collections.Concurrent;
using System.Linq;

namespace NerdyMishka.Search.Storeage
{
    public class RamFile : IDisposable
    {
        private ConcurrentDictionary<int, byte[]> store;
        private static readonly object s_lock = new object();
        public RamFile()
        {
            this.store = new ConcurrentDictionary<int, byte[]>();
            this.BlockSize =  65536; // 64 Kb
        }

        public RamFile(int blockSize)
        {
            this.store = new ConcurrentDictionary<int, byte[]>();
            this.BlockSize = blockSize;
        }

        public long BlockSize { get; private set; }

        public long Length { get; internal protected set; }

        public long LastModifiedAt { get; internal protected set; }

        public string Name { get; set; }

        public int BlockCount => this.store.Count;

        public byte[] this[int index] 
        {
            get { 
                if(index >= this.store.Count)
                    return null;

                return this.store[index];
            }
        }

        internal protected byte[] AllocateNewBuffer()
        {
            lock(s_lock)
            {
                var buffer = new byte[this.BlockSize];
                this.store.AddOrUpdate(this.BlockCount, buffer, (i, v) => buffer);
                return buffer;
            }
        }
        

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(!this.store.IsEmpty)
                    this.store.Clear();    

                this.store = null; 
            }
        }

        ~RamFile() {
            this.Dispose(false);
        }
    }
}