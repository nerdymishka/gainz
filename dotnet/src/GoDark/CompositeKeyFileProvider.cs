using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
  
    public class CompositeKeyFileProvider : CompositeKeyFragment
    {
        public CompositeKeyFileProvider(string path)
        {
            this.Path = path;

            if (!System.IO.File.Exists(path))
                throw new System.IO.FileNotFoundException(path);

            var data = GetData(path);
            this.SetData(data);

            data.Clear();
        }

        public string Path { get; private set; }

        private static byte[] GetData(string path)
        {
            return GetDataFromFile(path);
        }

        internal static byte[] GetDataFromFile(string path)
        {
            var text = File.ReadAllText(path);
            if (text != null)
                text = text.Replace("key: ", "").Trim();

            return Convert.FromBase64String(text);
        }

        public static void Generate(string path, byte[] entropy, HashAlgorithm hash = null)
        {
            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var generator = RandomNumberGenerator.Create();
            var key = new byte[32];
            generator.GetBytes(key);
            byte[] checksum = null;


            using (var ms = new MemoryStream())
            {
                ms.Write(entropy, 0, entropy.Length);
                ms.Write(key, 0, key.Length);

                var bytes = ms.ToArray();
                checksum = hash.ComputeHash(bytes);
            }

            CreateFile(path, checksum);

            if (createdHash)
                hash.Dispose();
        }

        private static void CreateFile(string path, byte[] data)
        {
            var enc = new UTF8Encoding(false, false);
            var content = $"key: {Convert.ToBase64String(data)}";
            File.WriteAllText(path, content, enc);
        }


        private byte[] Copy(byte[] source, int offset, int length = 4)
        {
            byte[] copy = new byte[length];
            Array.Copy(source, offset, copy, 0, length);
            return copy;
        }
    }
}
