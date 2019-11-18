using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NerdyMishka.Text;

namespace NerdyMishka.Security.Cryptography
{
  
    public class FormattedFileKeyFragment : FileKeyFragment
    {
        public FormattedFileKeyFragment(string path)
            :base(path, new FormattedKeyGenerator())
        {
            
        }

        public FormattedFileKeyFragment(string path, byte[] entropy)
            :base(path, new FormattedKeyGenerator(entropy))
        {
            
        }


        public class FormattedKeyGenerator : IFileKeyGenerator
        {
            private byte[] entropy;

            public FormattedKeyGenerator()
            {

            }


            public FormattedKeyGenerator(byte[] entropy)
            {
                this.entropy = entropy;
            }

            public byte[] GenerateContent()
            {
                byte[] checksum = null;

                using(var generator = new RandomNumberGenerator())
                using(var sha256 = SHA256.Create())
                using (var ms = new MemoryStream())
                {
                    var key = new byte[32];
                    generator.NextBytes(key);
                    if(entropy.Length > 0)
                        ms.Write(entropy, 0, entropy.Length);

                    ms.Write(key, 0, key.Length);

                    var bytes = ms.ToArray();
                    checksum = sha256.ComputeHash(bytes);
                }
                
                return Encodings.Utf8NoBom.GetBytes($"key: {Convert.ToBase64String(checksum)}");
            }

            public byte[] ReadContent(string path)
            {
               var text = File.ReadAllText(path);
                if (text != null)
                    text = text.Replace("key: ", "").Trim();

                return Convert.FromBase64String(text);
            }
        }
    }
}
