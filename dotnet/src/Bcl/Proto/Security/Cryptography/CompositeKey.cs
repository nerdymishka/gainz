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
                if(hash == null) 
                {
                    return ms.ToArray();
                }

                return hash.ComputeHash(ms);
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
        public virtual byte[] AssembleKey()
        {
            // defaults to GenerateKey
            var symmetricKey = this.options.SymmetricKey;
            var transform = this.options.ComputeHash ?? ComputePbkdf2Hash;

            if(symmetricKey.Length != 32)
            {
                var temp = new byte[32];
                Array.Copy(symmetricKey, temp, symmetricKey.Length);
                symmetricKey = temp;
            }

            byte[] key = null;
            if(this.options.HashAlgorithm != HashAlgorithmTypes.None)
            {
                using(var signer = HashAlgorithm.Create(this.options.HashAlgorithm.ToString()))
                {
                    byte[] hashedKey = UnprotectAndConcatData(this, signer);
                    if (hashedKey == null || hashedKey.Length != (signer.HashSize / 8))
                        return null;
                    // key generator can be swapped out with a native implementation.
                    key = transform(hashedKey, symmetricKey, this.options.Iterations);
                    
                    var hash = signer.ComputeHash(key);
                    hashedKey.Clear();
                    key.Clear();
                    return hash;
                }
            }

            byte[] rawKey = UnprotectAndConcatData(this, null);
            // key generator can be swapped out with a native implementation.
            key = transform(rawKey, symmetricKey, this.options.Iterations);
            rawKey.Clear();
            return key;
            
        }

        //TODO: provide an argon2 method. 

        public static byte[] ComputePbkdf2Hash(byte[] compositeKeyData, byte[] symmetricKey, long iterations)
        {
            var salt = new byte[32];
            Array.Copy(symmetricKey, salt, salt.Length);
            if(iterations > int.MaxValue) 
                iterations = int.MaxValue;
    
     
            return PasswordAuthenticator.Pbkdf2(compositeKeyData, salt, (int)iterations, compositeKeyData.Length);
        } 

        public static byte[] ComputeAesKdfHash(byte[] compositeKeyData, byte[] symmetricKey, long iterations)
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

            var hash = new byte[compositeKeyData.Length];
            Array.Copy(compositeKeyData, hash, compositeKeyData.Length);

            if (transform == null || transform.InputBlockSize != 16 || transform.OutputBlockSize != 16)
            {
                // TODO: add logging to CompositeKey 
                return null;
            }

            for (long i = 0; i < iterations; ++i)
            {
                transform.TransformBlock(hash, 0, 16, hash, 0);
                transform.TransformBlock(hash, 16, 16, hash, 16);
            }

            return hash;
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