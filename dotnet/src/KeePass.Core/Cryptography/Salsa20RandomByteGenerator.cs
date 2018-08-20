using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public class Salsa20RandomByteGenerator : IRandomByteGeneratorEngine
    {
        private ICryptoTransform transform;

        public int Id { get { return 2; } }

        private byte[] buffer = new byte[16];

        static Salsa20RandomByteGenerator()
        {
            
        }

        public void Initialize(byte[] key)
        {
            using (var cipher = Salsa20.Create())
            {
                cipher.SkipXor = true;
                cipher.Rounds = Salsa20Rounds.Ten;
                var iv = new byte[8] { 0xE8, 0x30, 0x09, 0x4B, 0x97, 0x20, 0x5D, 0x2A };
                this.transform = cipher.CreateDecryptor(key.ToSHA256Hash(), iv);

                key.Clear();
                iv.Clear();
            }
        }

        public byte[] NextBytes(int count)
        {
            if (count < 1)
                return new byte[0];

            byte[] bytes = new byte[count];

            this.transform.TransformBlock(bytes, 0, count, bytes, 0);

            return bytes;
        }
    }
}
