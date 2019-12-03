using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass.Cryptography
{
    public static class KeePassFileCryptoStreamFactory
    {
        private static List<IEncryptionEngine> s_ecryptionEngines;

        static KeePassFileCryptoStreamFactory()
        {
            s_ecryptionEngines = new List<IEncryptionEngine>();
            s_ecryptionEngines.Add(new KeePassCompliantAesEngine());
        }

        public static void AddEngine(IEncryptionEngine engine)
        {
            s_ecryptionEngines.Add(engine);
        }

        public static IEncryptionEngine Find(byte[] id)
        {
            return s_ecryptionEngines.SingleOrDefault(o =>  Check.Equal(o.Id, id));
        }

        public static Stream CreateCryptoStream(Stream stream, bool encrypt, MasterKey key, KeePassFileHeaderInformation headerInfo)
        {
            byte[] encryptionKey = null; 
            try {
               

                using (var ms = new MemoryStream())
                {
                    byte[] masterKeyData = GenerateKey(key, headerInfo.MasterKeyHashKey, headerInfo.MasterKeyHashRounds);
                    if (masterKeyData == null)
                        throw new Exception("Invalid Master Key or Invalid Master Key Generation");

                    ms.Write(headerInfo.DatabaseCipherKeySeed);
                    ms.Write(masterKeyData);
                    masterKeyData.Clear();
                    encryptionKey = ms.ToSHA256Hash();
                }

                if (encryptionKey == null)
                    throw new Exception("Invalid Master Key or Key Generation");

                var encryptionEngine = Find(headerInfo.DatabaseCipherId);
                if (encryptionEngine == null)
                    throw new Exception($"Encryption Engine could not be found for Id {headerInfo.DatabaseCipherId}");


                return encryptionEngine.CreateCryptoStream(stream, encrypt, encryptionKey, headerInfo.DatabaseCipherIV);
            }
            finally
            {
                encryptionKey.Clear();
            }
        }

        private static byte[] GenerateKey(MasterKey key, byte[] masterKeyHashKey, long masterKeyHashRounds)
        {
            const int size = 32;

            byte[] raw = UprotectedAndConcatData(key);
            if (raw == null || raw.Length != size)
                return null;

            byte[] masterKeyData = new byte[size];

            Array.Copy(raw, masterKeyData, size);
            raw.Clear();
            try
            {
                // TODO: make generate key plugable.
                if (!GenerateKey(masterKeyData, masterKeyHashKey, masterKeyHashRounds))
                    return null;

                return masterKeyData.ToSHA256Hash();
            }
            finally
            {
                masterKeyData.Clear();
            }
        }

        private static bool GenerateKey(byte[] masterKeyData, byte[] masterKeyHashKey, long masterKeyHashRounds)
        {
            byte[] iv = new byte[16];
            iv.Clear();

            Aes aes = Aes.Create();
            if (aes.BlockSize != 128)
            {
                aes.BlockSize = 128;
            }

            aes.IV = iv;
            aes.Mode = CipherMode.ECB;
            aes.KeySize = 256;
            aes.Key = masterKeyHashKey;
            ICryptoTransform transform = aes.CreateEncryptor();

            if (transform == null || transform.InputBlockSize != 16 || transform.OutputBlockSize != 16)
            {
                // TODO: add logging to CompositeKey 
                return false;
            }

            for (long i = 0; i < masterKeyHashRounds; ++i)
            {
                transform.TransformBlock(masterKeyData, 0, 16, masterKeyData, 0);
                transform.TransformBlock(masterKeyData, 16, 16, masterKeyData, 16);
            }

            return true;
        }

        private static byte[] UprotectedAndConcatData(MasterKey key)
        {
            // TODO: research if using a memory stream is the right thing to do.
            using (var ms = new MemoryStream())
            {
                foreach(var fragment in key)
                {
                    ms.Write(fragment.UnprotectAndCopyData());
                }

                return ms.ToSHA256Hash();
            }
        }
    }
}
