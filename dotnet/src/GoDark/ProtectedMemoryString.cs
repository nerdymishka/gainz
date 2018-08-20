using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Stores a string encrypted in memory, if the string is loaded as bytes.
    /// Once the string is unprotected, it can be found in the memory of the 
    /// application. 
    /// </summary>
    public class ProtectedMemoryString : ProtectedMemoryBinary, IEquatable<ProtectedMemoryString>
    {
        private string text;
        private readonly int length;
        private int hashCode;
        private static readonly System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryString"/>
        /// </summary>
        /// <param name="binary">The character binary data that will be stored.</param>
        /// <param name="encrypt">Should the data be encrypted. If string data is already in memory, this should be false.</param>
        public ProtectedMemoryString(byte[] binary, bool encrypt = true) :base(binary, encrypt)
        {
            if (binary == null)
                throw new ArgumentNullException("value");

            this.length = utf8.GetCharCount(binary);
            using (var sha512 = SHA512.Create())
            {
                var hashBytes = sha512.ComputeHash(binary);
                this.hashCode = MurMurHash3.ComputeHash(hashBytes, 20);
            }
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryString"/>
        /// </summary>
        /// <param name="value">The decrypted data that is stored</param>
        public ProtectedMemoryString(string value): base()
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.text = value;
            this.length = value.Length;
            this.hashCode = MurMurHash3.ComputeHash(utf8.GetBytes(value), 20);
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        public override int Length => this.length;

        /// <summary>
        /// Unprotects the binary data and returns it as a string, once in memory,
        /// there is no point to encrypting the string data again.
        /// </summary>
        /// <returns></returns>
        public string UnprotectAsString()
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            if (this.text != null)
                return this.text;

            var binary = this.UnprotectAsBytes();
            if (binary.Length == 0)
                return string.Empty;

           
            var result = utf8.GetString(binary, 0, this.Length);
            this.hashCode = MurMurHash3.ComputeHash(binary, 20);
            Array.Clear(binary, 0, binary.Length);

            this.text = result;
            return result;
        }

        /// <summary>
        /// Decrypts the inner data and returns a copy.
        /// </summary>
        /// <returns>Returns a copy of the data.</returns>
        public override byte[] UnprotectAsBytes()
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            if (this.text != null)
                return utf8.GetBytes(this.text);

            return base.UnprotectAsBytes();
        }

        /// <summary>
        /// Decrypts the inner data and returns a copy.
        /// </summary>
        /// <returns>Returns a copy of the data.</returns>
        public SecureString UnprotectAsSecureString()
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            byte[] bytes = null;
            if (this.text != null)
                bytes = utf8.GetBytes(this.text);
            else
                bytes = base.UnprotectAsBytes();

            var secureString = new SecureString();
            if(bytes.Length > 0)
            {

                var chars = utf8.GetChars(bytes);
                foreach (var c in chars)
                    secureString.AppendChar(c);

                chars.Clear();
            }

            bytes.Clear();

            return secureString;
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code. </returns>
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        /// <summary>
        /// Determines if the given object is equal to the current instance.
        /// </summary>
        /// <param name="other">That object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public bool Equals(ProtectedMemoryString other)
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            if (other == null)
                return false;

            if (this.Length != other.Length)
                return false;

            if (this.Id.EqualTo(other.Id))
                return true;

            if (this.text != other.text)
                return false;

            return base.Equals((ProtectedMemoryBinary)other);
        }

        /// <summary>
        /// Disposes the text data.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
                return;

            this.disposed = true;
            if(disposing)
            {
                this.hashCode = 0;
                this.text = null;
            }
            base.Dispose(disposing);
        }
    }
}
