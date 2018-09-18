using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Windows
{
    [CLSCompliant(false)]
    public class VaultCredential
    {
        public int AttributeCount { get; set; }

        public IntPtr Attributes { get; set; } 

        public string Comment { get; set; }

        public CredentialFlags Flags { get; set; }

        public DateTime LastWritten { get; set; }

        public Persistence Persistence { get; set; }

        public string Alias { get; set; }

        public string Key { get; set; }

        public CredentialsType Type { get; set; }

        /// <summary>
        /// Gets or sets the data. The data protected in memory by Salsa20
        /// </summary>
        /// <value></value>

        internal ProtectedMemoryString Data { get; set; }

        public string UserName { get; set; }

        /// <summary>
        /// Gets the blob as a secure string. 
        /// </summary>
        /// <param name="encoding">The text encoding. Defaults to UTF-8</param>
        /// <returns>A <cref="System.Security.SecurityString" /></returns>
        public SecureString GetBlobAsSecureString(Encoding encoding = null) {
            if(encoding == null)
                encoding = Encoding.UTF8;

            if(this.Data == null)
                return null;

            var bytes = this.Data.UnprotectAsBytes();
           
            var secureString = new SecureString();
            if(bytes.Length > 0)
            {
                var chars = encoding.GetChars(bytes);
                foreach (var c in chars)
                    secureString.AppendChar(c);

                Array.Clear(chars, 0, chars.Length);
            }

            Array.Clear(bytes, 0, bytes.Length);

            return secureString;
        }

        /// <summary>
        /// Gets the blob as binary
        /// </summary>
        /// <returns>A <cref="System.Security.SecurityString" /></returns>
        public byte[] GetBlob() {
            if(this.Data == null)
                return null;

            return this.Data.UnprotectAsBytes();
        }

        /// <summary>
        /// Gets the blob as an array of characters. 
        /// </summary>
        /// <param name="encoding">The text encoding. Defaults to UTF-8</param>
        /// <returns>A <cref="System.Char[]" /></returns>
        public char[] GetBlobAsChars(Encoding encoding = null)
        {
            if(encoding == null)
                encoding = Encoding.UTF8;

            if(this.Data == null)
                return null;

            var bytes = this.GetBlob();
            var chars = encoding.GetChars(bytes);
            Array.Clear(bytes, 0, bytes.Length);

            return chars;
        }

        /// <summary>
        /// Gets the blob as a string. This method should be used sparingly as
        /// strings are immutable and remain in memory longer. 
        /// </summary>
        /// <param name="encoding">The text encoding. Defaults to UTF-8</param>
        /// <returns>A <cref="System.String" /></returns>

        public string GetBlobAsString(Encoding encoding = null) 
        {
            if(this.Data == null)
                return null;

            if(encoding == null)
                encoding = Encoding.UTF8;

            var bytes = this.GetBlob();
            var result = encoding.GetString(bytes);
            Array.Clear(bytes, 0, bytes.Length);
            return result;
        }

        public void SetBlob(byte[] data) {
            this.Data = new ProtectedMemoryString(data, true);
            this.Length = this.Data.Length;
        }

        public void SetBlob(SecureString data, Encoding encoding = null) {
           if(encoding == null)
                encoding = System.Text.Encoding.UTF8;

            if (data != null)
            {
                IntPtr bstr = IntPtr.Zero;
                char[] charArray = new char[data.Length];

                try
                {
                    bstr = Marshal.SecureStringToBSTR(data);
                    Marshal.Copy(bstr, charArray, 0, charArray.Length);

                    var bytes = encoding.GetBytes(charArray);
                    this.Data = new ProtectedMemoryString(bytes, true);
                    this.Length = this.Data.Length;
                    bytes.Clear();
                    charArray.Clear();
                }
                finally
                {
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }
        }

        public void SetBlob(char[] data, Encoding encoding = null) {
            if(encoding == null) 
                encoding = Encoding.UTF8;
            
            var bytes = encoding.GetBytes(data);
            this.Data = new ProtectedMemoryString(bytes, true);
            Array.Clear(bytes, 0, bytes.Length);
            this.Length = this.Data.Length;
        } 

        public void SetBlob(string data, Encoding encoding = null ) {
            if(encoding == null)
                encoding = Encoding.UTF8;

            var bytes = encoding.GetBytes(data);
            this.Data = new ProtectedMemoryString(bytes, true);
            Array.Clear(bytes, 0, bytes.Length);
            this.Length = this.Data.Length;
        }

        public int Length { get; set; }
    }
}