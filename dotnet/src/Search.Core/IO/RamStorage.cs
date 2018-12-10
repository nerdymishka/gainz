
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace NerdyMishka.Search.IO 
{
    public class RamStorage : FileStorageProvider
    {
        private Dictionary<string, RamFile> files;
        private int blockSize = 65536;


        public RamStorage()
        {

        }

        public RamStorage(int blockSize)
        {
            this.blockSize= blockSize;
        }

        public override void Delete(string name)
        {
            if(this.files.TryGetValue(name, out RamFile file))
            {
                this.files.Remove(name);
                file.Dispose();
            }
        }

        public override void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if(isDisposing)
            {
                foreach(var file in this.files)
                    file.Value.Dispose();

                this.files.Clear();
                this.files = null;
            }
           
        }

     

        public override bool Exists(string name)
        {
            return this.files.ContainsKey(name);
        }

        public override long GetFileLength(string name)
        {
            if(!this.files.TryGetValue(name, out RamFile file))
                throw new FileNotFoundException(name);

            return file.Length;
        }

        public override long GetLastModifiedDate(string name)
        {
            if(!this.files.TryGetValue(name, out RamFile file))
                throw new FileNotFoundException(name);

            return file.LastModifiedAt;
        }

        public override string[] ListAll()
        {
            return this.files.Keys.ToArray();
        }

        public override void Move(string source, string destination, bool overwrite = true)
        {
            if(!this.files.TryGetValue(source, out RamFile file))
                throw new FileNotFoundException(source);

            if(!overwrite)
            {
                if(this.files.ContainsKey(destination))
                    throw new AccessViolationException($"{destination} already exists");
            }

            this.files.Remove(file.Name);
            file.Name = destination;
            this.files.Add(file.Name, file);
        }

        public override Stream OpenRead(string name)
        {
            if(!this.files.TryGetValue(name, out RamFile file))
                throw new FileNotFoundException(name);

            return new RamStream(file);
        }

        public override Stream OpenWrite(string name)
        {
            var file = new RamFile(this.blockSize) { Name = name };
            this.files.Add(name, file);
            return new RamStream(file, false, true);
        }

        ~RamStorage() {
            this.Dispose(false);
        }
    }
}