

using System;
using System.Security;
using System.Text;
using NerdyMishka.Text;

namespace NerdyMishka.Security.Cryptography
{
    /// <summary>
    /// Stores a string encrypted in memory. If the string is unprotected, it
    /// can be found in memory. The protected string will not keep a reference of
    /// string unless it is interned in hopes that the string will be collected.
    /// </summary>
    public class ProtectedString : ProtectedBytes, IEquatable<ProtectedString>, IComparable<ProtectedString>
    {
        private string text;
        private int hashCode;
        private bool disposed = false;
        private Encoding encoding;


        public ProtectedString(Span<byte> bytes, Encoding encoding = null, bool encrypt = true) 
            : base(bytes, true, encrypt)
        {
            
        }

        public ProtectedString(ReadOnlySpan<byte> bytes, Encoding encoding = null, bool encrypt = true) 
            : base(bytes, true, encrypt)
        {
            
        }


        /// <summary>
        /// Initializes a new instance of <see cref="ProtectedMemoryString"/>
        /// </summary>
        /// <param name="binary">The character binary data that will be stored.</param>
        /// <param name="encrypt">Should the data be encrypted. If string data is already in memory, this should be false.</param>
        public ProtectedString(byte[] bytes, Encoding encoding = null, bool encrypt = true) 
            : base(bytes, true, encrypt)
        {
            this.encoding = encoding;
        }


        public ProtectedString(ReadOnlySpan<char> chars, Encoding encoding = null, bool encrypt = true) 
            :base((encoding ?? Encodings.Utf8NoBom).GetBytes(chars.ToArray()), true, encrypt)
        {
            this.encoding = encoding;
        }


        public ProtectedString(char[] chars, Encoding encoding = null, bool encrypt = true) 
            :base((encoding ?? Encodings.Utf8NoBom).GetBytes(chars), true, encrypt)
        {
            this.encoding = encoding;
        }


        protected ProtectedString(SecureString secureString, Encoding encoding = null)
            :base(secureString.ToBytes(encoding), true, false)
        {
           
        }


        protected ProtectedString(string value, Encoding encoding = null)
            :base((encoding ?? Encodings.Utf8NoBom).GetBytes(value), true, false)
        {
             this.encoding = encoding;
        }

       protected override ReadOnlyMemory<byte> Init(byte[] bytes, bool computeHash, bool encrypt = true)
       {    
            // get the length before Grow is invoked.
            this.Length = (this.encoding ?? Encodings.Utf8NoBom).GetCharCount(bytes);

            // case the base
            var result =  base.Init(bytes, computeHash, encrypt);
            
            // get a unique hash code protected string
            this.HashCode = MurMurHash3.ComputeHash(this.Hash, 20);
            return result;
       }


       public ReadOnlySpan<char> CopyAsReadOnlySpan()
       {
            var bytes = this.ToArray();
            ReadOnlySpan<char> span = (encoding ?? Encodings.Utf8NoBom)
                    .GetChars(bytes);

            bytes.Clear();
            return span;
       }

       public char[] ToCharArray()
       {
           var bytes = this.ToArray();
           var chars = (encoding ?? Encodings.Utf8NoBom)
                    .GetChars(bytes);
            
            bytes.Clear();
            return chars;
       }

       public static bool operator ==(ProtectedString left, ProtectedString right)
       {
           return left.Equals(right);
       }

       public static bool operator !=(ProtectedString left, ProtectedString right)
       {
           return !left.Equals(right);
       }


       public void Append(char c)
       {
            if(this.isReadOnly)
                throw new InvalidOperationException("Cannot append to read only protected string.");

           this.Append(new[] { c });
       }

       public void Append(char[] chars)
       {
            if(this.isReadOnly)
                throw new InvalidOperationException("Cannot append to read only protected string.");

           var bytes = (this.encoding ?? Encodings.Utf8NoBom).GetBytes(chars);
           this.Append(bytes);
       }

        /// <summary>
        /// Unprotects the binary data and returns it as a string, once in memory,
        /// there is no point to encrypting the string data again.
        /// </summary>
        /// <returns></returns>
        public string CopyAsString()
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            // already in memory. only use this for interned strings.
            // strings that are not interned have a chance of being 
            // garbage collected. 
            if (this.text != null)
            {
                return this.text;
            }
            
            var bytes = this.ToArray();
            if (bytes.Length == 0)
                return string.Empty;

            var result = (encoding ?? Encodings.Utf8NoBom)
                .GetString(bytes, 0, this.Length);

            bytes.Clear();

            var interned = String.IsInterned(result);
            if(interned != null)
            {
                this.text = interned;
            }

            return result;
        }

       
        

        /// <summary>
        /// Decrypts the inner data and returns a copy.
        /// </summary>
        /// <returns>Returns a copy of the data.</returns>
        public SecureString ToSecureString()
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            var secureString = new SecureString();
            if(this.text != null)
            {
                foreach(var c in this.text)
                    secureString.AppendChar(c);

                return secureString;
            }

            var bytes = this.ToArray();
          
            if(bytes.Length > 0)
            {
                var chars = (encoding ?? Encodings.Utf8NoBom).GetChars(bytes);
                foreach (var c in chars)
                    secureString.AppendChar(c);

                chars.Clear();
            }

            bytes.Clear();

            return secureString;
        }

        public override string ToString()
        {
            return "***********";
        }

        /// <summary>
        /// Gets a hash code for this object.
        /// </summary>
        /// <returns>The hash code. </returns>
        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override bool Equals(object obj)
        {
            if(obj is ProtectedString)
                return this.Equals((ProtectedString)obj);

            return false;
        }

        /// <summary>
        /// Determines if the given object is equal to the current instance.
        /// </summary>
        /// <param name="other">That object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public bool Equals(ProtectedString other)
        {
            if (this.disposed)
                throw new ObjectDisposedException($"ProtectedMemoryString {this.Id}");

            if (other == null)
                return false;

            return base.Equals((ProtectedBytes)other);
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
                this.encoding = null;
                this.text = null;
            }
            base.Dispose(disposing);
        }

        public int CompareTo(ProtectedString other)
        {
            if(other == null)
                return 1;
            
            var right = other.CopyAsReadOnlySpan();
            var left = this.CopyAsReadOnlySpan();

            if(right.Length != other.Length)
            {
                if(right.Length > other.Length)
                    return 1;
                return -1;
            }

            for(var i = 0; i < right.Length; i++)
            {
                char a  = left[i];
                char b = right[i];
                if(a == b)
                {
                    continue;
                }

                if(a > b)
                    return 1;

                return -1;
            }

            return 0;
        }
    }
}
