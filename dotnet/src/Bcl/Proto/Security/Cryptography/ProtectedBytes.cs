


using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Encrypts binary data that is stored in memory.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         https://msdn.microsoft.com/en-us/library/system.security.cryptography.protectedmemory.aspx
    ///         
    ///     </para>
    /// </remarks>
    public class ProtectedBytes : IEquatable<ProtectedBytes>, IDisposable
    {
        private readonly byte[] bytes;
        private byte[] id;

        /// <summary>
        /// The unique id for this object.
        /// </summary>
        public byte[] Id => this.id;


        /// <summary>
        /// Is the data protected?
        /// </summary>
        public bool IsProtected { get; protected set; }

        private DataProtectionAction Action { get; set; }

        /// <summary>
        /// Gets or sets the DataPortectionAction, the default implementation uses <see cref="NerdyMishka.Security.Cryptography.Salsa20"/>,
        /// however this can be replaced with <see cref="System.Security.Cryptography.ProtectedMemory.ProtectedMemory"/>
        /// </summary>
        public static DataProtectionAction DataProtectionAction { get; set; }

        public static bool UseDapi { get; set; } = false;

        public virtual int Length { get; protected set; }

        protected int HashCode { get; set; }

        protected byte[] Hash { get; set; }


        static ProtectedBytes()
        {
            var salsa20 = Salsa20.Create();
            salsa20.GenerateKey();
            var key = salsa20.Key;

            // This is defaulted to the Salsa20 Stream Cipher
            // because the ProtectedMemory Api is windows specific.
            DataProtectionAction = (bytes, state, operation) =>
            {
                var protectedData = (ProtectedBytes)state;
                
                var transform = operation == DataProtectionActionType.Encrypt ?
                    salsa20.CreateEncryptor(key, protectedData.Id) : 
                    salsa20.CreateDecryptor(key, protectedData.Id);

                

                return transform.TransformFinalBlock(bytes, 0, bytes.Length);
            };
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryBinary"/>
        /// </summary>
        public ProtectedBytes()
        {
            this.id = Util.GenerateId();
        }

        public ProtectedBytes(Span<byte> bytes, bool encrypt = true) 
            : this(bytes, true, encrypt)
        {
            
        }


        public ProtectedBytes(ReadOnlySpan<byte> bytes, bool encrypt = true)     
            : this(bytes, true, encrypt)
        {

        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryBinary"/>
        /// </summary>
        /// <param name="binary">The binary data that will be protected</param>
        /// <param name="encrypt">should the binary data be encrypted or not.</param>
        public ProtectedBytes(byte[] bytes, bool encrypt = true)
            : this(bytes, true, encrypt)
        {
          
        }


        protected ProtectedBytes(Span<byte> bytes, bool computeHash, bool encrypt = true)
        {
            var rent = ArrayPool<byte>.Shared.Rent(bytes.Length);
            this.bytes = this.Init(rent, computeHash, encrypt);
            ArrayPool<byte>.Shared.Return(rent);
        }

        protected ProtectedBytes(ReadOnlySpan<byte> bytes, bool computeHash, bool encrypt = true)
        {
            var rent = ArrayPool<byte>.Shared.Rent(bytes.Length);
            this.bytes = this.Init(rent, computeHash, encrypt);
            ArrayPool<byte>.Shared.Return(rent);
        }

        protected ProtectedBytes(byte[] bytes, bool computeHash, bool encrypt = true)
        {
            this.bytes = this.Init(bytes, computeHash, encrypt);
        }

        protected virtual byte[] Init(byte[] bytes, bool computeHash, bool encrypt = true)
        {
            this.Length = bytes.Length;
            this.IsProtected = encrypt;

            using (var sha512 = SHA512.Create())
            {
                this.Hash = sha512.ComputeHash(bytes);
            }

            // from the MSDN Docs
            // userData must be 16 bytes in length or in multiples of 16 bytes.
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.protectedmemory.protect.aspx
            bytes = Grow(bytes, 16);
            return this.Encrypt(bytes);
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            if (this.HashCode == -1)
                this.HashCode = this.CreateHashCode(7);

            return this.HashCode;
        }

        /// <summary>
        /// Creates a hashcode for this object.
        /// </summary>
        /// <param name="seed">The initial prime number.</param>
        /// <returns>The hash code.</returns>
        protected int CreateHashCode(int seed)
        {
            return MurMurHash3.ComputeHash(this.Hash, seed);
        }

        public virtual byte[] ToArray()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            return this.Decrypt();
        }

        /// <summary>
        /// Determines if the given object is equal to the current instance.
        /// </summary>
        /// <param name="other">That object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public bool Equals(ProtectedBytes other)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            // data for comparison. 
            if (other == null)
                return false;

            if (this.IsProtected != other.IsProtected)
                return false;

            if (this.Length != other.Length)
                return false;

            if (this.id.EqualTo(other.id))
                return true;

            return this.Hash.EqualTo(other.Hash);
        }

        

        private byte[] Encrypt(byte[] bytes)
        {
            if (!this.IsProtected)
                return bytes;

            var action = this.Action ?? DataProtectionAction;

            if (action == null)
                throw new NullReferenceException("DataProtectionAction");

            this.Action = action;

            return action(bytes, this, DataProtectionActionType.Encrypt);
        }

        private byte[] Decrypt()
        {
            if (!this.IsProtected)
            {
                var copy = new byte[this.Length];
                Array.Copy(this.bytes, copy, copy.Length);
                return copy;
            }

            var action = this.Action ?? DataProtectionAction;
            if (action == null)
                throw new NullReferenceException("DataProtectionAction");

            var result = action(this.bytes, this, DataProtectionActionType.Decrypt);
            if(result.Length > this.Length)
            {
                var copy = new byte[this.Length];
                Array.Copy(result, copy, copy.Length);
                return copy;
            }

            return result;
        }

        

 
        private static byte[] Grow(byte[] binary, int blockSize)
        {
            int length = binary.Length;
            int blocks = binary.Length / blockSize;
            int size = blocks * blockSize;
            if ((size) <= length)
            {
                while (size < length)
                {
                    blocks++;
                    size = blocks * blockSize;
                }
            }

            byte[] result = new byte[blocks * blockSize];
            Array.Copy(binary, result, binary.Length);
            return result;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Array.Clear(this.bytes, 0, this.bytes.Length);
                    Array.Clear(this.Hash, 0, this.Hash.Length);
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~ProtectedBytes()
        {
            Dispose(false);
        }
        #endregion


    }
}