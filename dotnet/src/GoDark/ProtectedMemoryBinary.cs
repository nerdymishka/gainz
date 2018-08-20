using System;
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
    public class ProtectedMemoryBinary : IEquatable<ProtectedMemoryBinary>, IDisposable
    {
        private readonly byte[] binary;
        private byte[] hashBytes;
        private readonly int length;
        private byte[] id;
        private int hashCode = -1;

        /// <summary>
        /// The unique id for this object.
        /// </summary>
        public byte[] Id { get { return this.id; } }


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

        public virtual int Length
        {
            get { return this.length; }
        }


        static ProtectedMemoryBinary()
        {
            var salsa20 = Salsa20.Create();
            salsa20.GenerateKey();
            var key = salsa20.Key;

            // This is defaulted to the Salsa20 Stream Cipher
            // because the ProtectedMemory Api is windows specific.
            DataProtectionAction = (data, state, operation) =>
            {
                var protectedData = (ProtectedMemoryBinary)state;
                
                var transform = operation == DataProtectionActionType.Encrypt ?
                    salsa20.CreateEncryptor(key, protectedData.Id) : 
                    salsa20.CreateDecryptor(key, protectedData.Id);

                return transform.TransformFinalBlock(data, 0, data.Length);
            };
        }
        
        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryBinary"/>
        /// </summary>
        public ProtectedMemoryBinary()
        {
            this.id = Util.GenerateId();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryBinary"/>
        /// </summary>
        /// <param name="binary">The binary data that will be protected</param>
        /// <param name="encrypt">should the binary data be encrypted or not.</param>
        public ProtectedMemoryBinary(byte[] binary, bool encrypt = true)
            : this()
        {
            this.length = binary.Length;
            this.IsProtected = encrypt;

            using (var sha512 = SHA512.Create())
            {
                this.hashBytes = sha512.ComputeHash(binary);
            }
            
            // from the MSDN Docs
            // userData must be 16 bytes in length or in multiples of 16 bytes.
            // https://msdn.microsoft.com/en-us/library/system.security.cryptography.protectedmemory.protect.aspx
            binary = Grow(binary, 16);

            if (encrypt)
                this.binary = this.Encrypt(binary);
            else
                this.binary = binary;
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            if (this.hashCode == -1)
                this.hashCode = this.CreateHashCode(7);

            return this.hashCode;  
        }

        /// <summary>
        /// Creates a hashcode for this object.
        /// </summary>
        /// <param name="seed">The initial prime number.</param>
        /// <returns>The hash code.</returns>
        protected int CreateHashCode(int seed)
        {
            return MurMurHash3.ComputeHash(this.hashBytes, seed);
        }

        /// <summary>
        /// Decrypts the inner data and returns a copy.
        /// </summary>
        /// <returns>Returns a copy of the data.</returns>
        public virtual byte[] UnprotectAsBytes()
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            // Decrypt will create a copy.
            byte[] copy = new byte[this.length];
            byte[] reference = this.binary;

            if (this.IsProtected)
                reference = this.Decrypt();

            Array.Copy(reference, copy, this.length);

            if (this.IsProtected)
                Array.Clear(reference, 0, reference.Length);

            return copy;
        }

        /// <summary>
        /// Determines if the given object is equal to the current instance.
        /// </summary>
        /// <param name="other">That object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public bool Equals(ProtectedMemoryBinary other)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException($"ProtectedMemoryBinary {this.id}");

            // TODO: determine if equality needs to decrypt binary
            // data for comparison. 
            if (other == null)
                return false;

            if (this.IsProtected != other.IsProtected)
                return false;

            if (this.length != other.length)
                return false;

            if (this.id.EqualTo(other.id))
                return true;

            // SHA512 Collisons should be low. 
            return this.hashBytes.EqualTo(other.hashBytes);
        }

        private byte[] Encrypt(byte[] binary)
        {
            if (!this.IsProtected)
                return this.binary;

            var action = this.Action ?? DataProtectionAction;

            if (action == null)
                throw new NullReferenceException("DataProtectionAction");

            this.Action = action;

            return action(binary, this, DataProtectionActionType.Encrypt);
        }

        private byte[] Decrypt()
        {
            if (!this.IsProtected)
                return this.binary;

            
            var action = this.Action;
            if (action == null)
                throw new NullReferenceException("DataProtectionAction");

            return action(binary, this, DataProtectionActionType.Decrypt);
        }

        // TODO: move to BitsAndPieces
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
                    Array.Clear(this.binary, 0, this.binary.Length);
                    Array.Clear(this.hashBytes, 0, this.hashBytes.Length);
                }

                disposedValue = true;
            }
        }


        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        ~ProtectedMemoryBinary()
        {
            Dispose(false);
        }
        #endregion


    }
}
