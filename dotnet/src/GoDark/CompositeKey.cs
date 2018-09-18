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
    /// <summary>
    /// A compsite key used for encryption. This can be used for the <see cref="DataProtection" />
    /// class. The composite key requires the fragments to be added in the same order and is
    /// designed with plugins in mind. 
    /// </summary>
    /// <typeparam name="ICompositeKeyFragment"></typeparam>
    public class CompositeKey : IEnumerable<ICompositeKeyFragment>, IDisposable
    {
        private List<ICompositeKeyFragment> fragments = new List<ICompositeKeyFragment>();
        private Func<byte[], byte[], long, bool> keyGenerator;
        public Func<HashAlgorithm> CreateHash { get; private set; }
        
        public int Count => this.fragments.Count;

        public CompositeKey()
        {
            this.CreateHash = () => SHA256.Create();
            this.keyGenerator = GenerateKey;
        }

        public CompositeKey(Func<HashAlgorithm> create)
        {
            this.CreateHash = create;
            this.keyGenerator = GenerateKey;
        }

        public CompositeKey(Func<HashAlgorithm> create, Func<byte[], byte[], long, bool> keyGenerator)
        {
            this.CreateHash = create;
            this.keyGenerator = keyGenerator;
        }

        public void AddPassword(byte[] password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.CreateHash()));
        }

        public void AddPassword(string password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.CreateHash()));
        }

        public void AddPassword(SecureString password)
        {
            this.Add(new CompositeKeyPasswordProvider(password, this.CreateHash()));
        }

        public void AddDerivedPassword(SecureString password, int iterations = 64000)
        {
            this.Add(new CompositeKeyDerivedPasswordProvider(password, this.CreateHash(), iterations));
        }

        public void AddDerivedPassword(string password, int iterations = 64000)
        {
            this.Add(new CompositeKeyDerivedPasswordProvider(password, this.CreateHash(), iterations));
        }

        public void AddDerivedPassword(byte[] password, int iterations = 64000)
        {
            this.Add(new CompositeKeyDerivedPasswordProvider(password, this.CreateHash(), iterations));
        }


        public void AddKeyFile(string file)
        {
            this.Add(new CompositeKeyFileProvider(file));
        }

        public void AddRsaCertificate(X509Certificate2 certificate2)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, this.CreateHash()));
        }

        public void AddRsaCertificate(X509Certificate2 certificate2, byte[] message)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, message, this.CreateHash()));
        }

        public void AddRsaCertificate(X509Certificate2 certificate2, string file)
        {
            this.Add(new CompositeKeyRsaProvider(certificate2, file, this.CreateHash()));
        }

        public static byte[] UnprotectAndConcatData(CompositeKey key, HashAlgorithm hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
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
                hash.Dispose();
                return checksum;
            }
        }

        /// <summary>
        /// Assembles the composite key.
        /// </summary>
        /// <param name="symmetricKey">
        /// Optional. The symmetricKey used to create the composite key. The
        /// symmetricKey could be stored with the encrypted value or application.
        /// </param>
        /// <param name="iterations">The number of iterations to use to generate the key.</param>
        /// <param name="transform">Optional. The transform function used to generate the key.</param>
        /// <returns>The composite key.</returns>
        public byte[] AssembleKey(
            byte[] symmetricKey = null, 
            long iterations = 10000,
            Func<byte[], byte[], long, bool> transform = null)
        {
            const int size = 32;

            // defaults to GenerateKey
            if (transform == null)
                transform = this.keyGenerator;

            if (symmetricKey == null)
                symmetricKey = System.Text.Encoding.UTF8.GetBytes("#2342f 234d++_12sq21 sq__");

            if(symmetricKey.Length != 32)
            {
                var temp = new byte[32];
                Array.Copy(symmetricKey, temp, symmetricKey.Length);
                symmetricKey = temp;
            }

            byte[] raw = UnprotectAndConcatData(this, this.CreateHash());
            if (raw == null || raw.Length != size)
                return null;

            byte[] compositeKeyData = new byte[size];

            Array.Copy(raw, compositeKeyData, size);
            raw.Clear();
 
            // key generator can be swapped out with a native implementation.
            if (!transform(compositeKeyData, symmetricKey, iterations))
                return null;

            using(var hash = this.CreateHash())
            {
                var checksum = hash.ComputeHash(compositeKeyData);
                return checksum;
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
                
            }
        }

        ~CompositeKey() {
            this.Dispose(false);
        }
    }
}