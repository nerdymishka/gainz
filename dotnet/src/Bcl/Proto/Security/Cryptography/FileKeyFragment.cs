using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NerdyMishka.Validation;

namespace NerdyMishka.Security.Cryptography
{
    public interface IFileKeyGenerator
    {
        byte[] GenerateContent();
        byte[] ReadContent(string path);
    }

    public class Sha256FileKeyGenerator : IFileKeyGenerator
    {
        public byte[] GenerateContent()
        {
            using(var rng = new RandomNumberGenerator())
            {
                return rng.NextBytes(256);
            }
        }

        public byte[] ReadContent(string path)
        {
            using(var fs = System.IO.File.OpenRead(path))
            using(var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(fs);
            }
        }
    }

    public class FileKeyFragment : CompositeKeyFragment
    {

        protected FileKeyFragment()
        {

        }

        public FileKeyFragment(string path, IFileKeyGenerator generator = null)
        {
            Check.NotNullOrWhiteSpace(nameof(path), path);
            generator = generator ?? new Sha256FileKeyGenerator();
            this.Path = path;

            if(!File.Exists(this.Path))
            {
                var bytes = generator.GenerateContent();
                File.WriteAllBytes(path, bytes);
                bytes.Clear();
            }

            var data = generator.ReadContent(path);
            this.SetData(this.ApplyTransform(data));

            data.Clear();
        }

        public string Path { get; private set; }

        protected virtual byte[] ApplyTransform(byte[] data)
        {
            return data;
        }
     
    }
}
