using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public class KeePassCompliantAesEngine : IEncryptionEngine
    {
        public byte[] Id
        {
            get
            {
                // this must match in order to open a KeePass file.
                return new byte[]{
                        0x31, 0xC1, 0xF2, 0xE6, 0xBF, 0x71, 0x43, 0x50,
                        0xBE, 0x58, 0x05, 0x21, 0x6A, 0xFC, 0x5A, 0xFF };
            }

        }

        public Stream CreateCryptoStream(Stream stream, bool encrypt, byte[] key, byte[] iv)
        {
            ICryptoTransform transform = null;
            byte[] localKey = new byte[32];
            byte[] localIV = new byte[16];
            Array.Copy(key, localKey, 32);
            Array.Copy(iv, localIV, 16);

            using (Aes aes = Aes.Create())
            {
                // setup 
                aes.BlockSize = 128;
                aes.KeySize = 256;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                transform = encrypt ? 
                    aes.CreateEncryptor(localKey, localIV) 
                    : aes.CreateDecryptor(localKey, localIV);

                // The transform performs operations that mutates the key and iv
                // The key and iv can be disguarded after the AesCngCryptoTransform
                // is created. 
                localKey.Clear();
                localIV.Clear();
            }

            if (transform == null)
                throw new Exception("AES transform failed");

            return new CryptoStream(stream, transform, encrypt ? CryptoStreamMode.Write : CryptoStreamMode.Read);
        }
    }
}
