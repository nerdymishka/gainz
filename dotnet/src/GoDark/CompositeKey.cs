using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKey : IEnumerable<ICompositeKeyFragment>, IDisposable
    {
        private List<ICompositeKeyFragment> fragments = new List<ICompositeKeyFragment>();
        private Func<byte[], byte[], long, bool> keyGenerator;
        public HashAlgorithm HashAlgorithm { get; private set; }

        public CompositeKey()
        {
            this.HashAlgorithm = SHA256.Create();
            this.keyGenerator = GenerateKey;
        }

        public CompositeKey(HashAlgorithm hashAlgorithm)
        {
            this.HashAlgorithm = hashAlgorithm;
            this.keyGenerator = GenerateKey;
        }

        public CompositeKey(HashAlgorithm hashAlgorithm, Func<byte[], byte[], long, bool> keyGenerator)
        {
            this.HashAlgorithm = hashAlgorithm;
            this.keyGenerator = keyGenerator;
        }

        public void AddPassword(byte[] password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.HashAlgorithm));
        }

        public void AddPassword(string password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.HashAlgorithm));
        }

        public void AddPassword(SecureString password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.HashAlgorithm));
        }


        public void AddKeyFile(string file)
        {
            this.Add(new CompositeKeyFileProvider(file));
        }

        public void AddRsaCertificate(X509Certificate2 certificate2)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, this.HashAlgorithm));
        }

        public void AddRsaCertificate(X509Certificate2 certificate2, byte[] message)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, message, this.HashAlgorithm));
        }


        public void AddRsaCertificate(X509Certificate2 certificate2, string file)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, file, this.HashAlgorithm));
        }


        private static byte[] UprotectedAndConcatData(CompositeKey key, HashAlgorithm hash = null)
        {
            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            // TODO: research if using a memory stream is the right thing to do.
            using (var ms = new MemoryStream())
            {
                foreach (var fragment in key)
                {
                    var bytes = fragment.UnprotectAndCopyData();
                    ms.Write(bytes, 0, bytes.Length);
                }

                var checksum = hash.ComputeHash(ms.ToArray());
                if (createdHash)
                    hash.Dispose();

                return checksum;
            }
        }

        public byte[] AssembleKey(
            byte[] symmetricKey = null, 
            long iterations = 10000,
            Func<byte[], byte[], long, bool> transform = null)
        {
            const int size = 32;

            if (transform == null)
                transform = CompositeKey.GenerateKey;

            if (symmetricKey == null)
                symmetricKey = System.Text.Encoding.UTF8.GetBytes("#2342f 234d++_12sq21 sq__");

        

            byte[] raw = UprotectedAndConcatData(this, this.HashAlgorithm);
            if (raw == null || raw.Length != size)
                return null;

            byte[] compositeKeyData = new byte[size];

            Array.Copy(raw, compositeKeyData, size);
            raw.Clear();
            try
            {
                // key generator can be swapped out with a native implementation.
                if (!this.keyGenerator(compositeKeyData, symmetricKey, iterations))
                    return null;

                var checksum = this.HashAlgorithm.ComputeHash(compositeKeyData);
                return checksum;
            }
            finally
            {
                this.Clear();
            }
        }

        private static bool GenerateKey(byte[] compositeKeyData, byte[] symmetricKey, long iterations)
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
            aes.Key = symmetricKey;
            ICryptoTransform transform = aes.CreateEncryptor();

            if (transform == null || transform.InputBlockSize != 16 || transform.OutputBlockSize != 16)
            {
                // TODO: add logging to CompositeKey 
                return false;
            }

            for (long i = 0; i < iterations; ++i)
            {
                transform.TransformBlock(compositeKeyData, 0, 16, compositeKeyData, 0);
                transform.TransformBlock(compositeKeyData, 16, 16, compositeKeyData, 16);
            }

            return true;
        }

        public void Add(ICompositeKeyFragment fragment)
        {
            this.fragments.Add(fragment);
        }

        public void Clear()
        {
            this.fragments.Clear();
        }

        public IEnumerator<ICompositeKeyFragment> GetEnumerator()
        {
            return this.fragments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        protected void Dispose(bool isDisposing)
        {
            if(isDisposing)
            {
                if(this.HashAlgorithm != null)
                {
                    this.HashAlgorithm.Dispose();
                    this.HashAlgorithm = null;
                }
            }
        }

        ~CompositeKey() {
            this.Dispose(false);
        }
    }
}
