using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NerdyMishka.Text;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// A compsite key used for encryption. This can be used for the <see cref="DataProtection" />
    /// class. The composite key requires the fragments to be added in the same order and is
    /// designed with plugins in mind. 
    /// </summary>
    /// <typeparam name="ICompositeKeyFragment"></typeparam>
    public class CompositeKey : ICompositeKey
    {
        private List<ICompositeKeyFragment> fragments = new List<ICompositeKeyFragment>();

        private CompositeKeyOptions options;
        
        public int Count => this.fragments.Count;

      

        public CompositeKey(CompositeKeyOptions options = null)
        {
            this.options = options ?? new CompositeKeyOptions() {
                SymmetricKey = Encodings.Utf8NoBom.GetBytes("#2342f 234d++_12sq21 sq__")
            };
        }


       

        public static byte[] UnprotectAndConcatData(CompositeKey key, HashAlgorithm hash)
        {
            if (hash == null)
            {
                throw new ArgumentNullException(nameof(hash));
            }

         
            using (var ms = new MemoryStream())
            {
                foreach (var fragment in key)
                {
                    var bytes = fragment.CopyData();
                    #if NETSTANDARD2_1
                        ms.Write(bytes);
                    #else 
                        var set = ArrayPool<byte>.Shared.Rent(bytes.Length);
                        bytes.CopyTo(set);
                        ms.Write(set, 0, bytes.Length);
                        ArrayPool<byte>.Shared.Return(set);
                    #endif 
                }
                
                ms.Flush();
                ms.Position = 0;
                var checksum = hash.ComputeHash(ms);
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
        public byte[] AssembleKey()
        {
            const int size = 32;

            // defaults to GenerateKey
            var symmetricKey = this.options.SymmetricKey;
            var transform = this.options.Transform ?? GenerateKey;
            using(var hash = HashAlgorithm.Create(this.options.HashAlgorithm.ToString()))
            {

                if(symmetricKey.Length != 32)
                {
                    var temp = new byte[32];
                    Array.Copy(symmetricKey, temp, symmetricKey.Length);
                    symmetricKey = temp;
                }

                byte[] raw = UnprotectAndConcatData(this, hash);
                if (raw == null || raw.Length != size)
                    return null;

                byte[] compositeKeyData = new byte[size];

                Array.Copy(raw, compositeKeyData, size);
                raw.Clear();
    
                // key generator can be swapped out with a native implementation.
                if (!transform(compositeKeyData, symmetricKey, this.options.Iterations))
                    return null;
           
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
                foreach(var fragment in this.fragments)
                {
                    if(fragment is IDisposable)
                    {
                        ((IDisposable)fragment).Dispose();
                    }
                }

                this.fragments.Clear();
                this.fragments = null;
                this.options = null;
            }
        }

        ~CompositeKey() {
            this.Dispose(false);
        }
    }
}