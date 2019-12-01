using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Windows
{
    /// <summary>
    /// Represents a Windows Vault Credential
    /// </summary>
    [CLSCompliant(false)]
    public class VaultCredential
    {
        /// <summary>
        /// Gets or sets the number of attributes for this entry.
        /// </summary>
        /// <value>The number of attributes for this entry.</value>
        public int AttributeCount { get; set; }

        /// <summary>
        /// Gets or sets the attributes associated with this entry.
        /// </summary>
        /// <value>The attributes associated with this entry.</value>
        public IntPtr Attributes { get; set; } 
        
        /// <summary>
        ///  Gets or sets the comment that descripts this entry.
        /// </summary>
        /// <value>The attributes associated with this entry.</value>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets <see cref="CredentialFlags" /> for this entry. Default is None.
        /// </summary>
        /// <value></value>
        public CredentialFlags Flags { get; set; }

        /// <summary>
        ///  Gets or sets last time this entry was updated.
        /// </summary>
        /// <value> The last time this entry was updated.</value>
        public DateTime LastWritten { get; set; }

        /// <summary>
        ///  Gets or sets the data persistence option for this entry. Default is LocalMachine.
        /// </summary>
        /// <value></value>
        public Persistence Persistence { get; set; }

        /// <summary>
        ///  Gets or sets the alias for this entry.
        /// </summary>
        /// <value></value>
        public string Alias { get; set; }

        /// <summary>
        ///  Gets or sets unique key for this entry.
        /// </summary>
        /// <value></value>
        public string Key { get; set; }

        /// <summary>
        ///  Gets or sets the type of credential this entry stores. Defaults to generic.
        /// </summary>
        /// <value></value>
        public CredentialsType Type { get; set; }

        /// <summary>
        /// Gets or sets the data. The data protected in memory by Salsa20
        /// </summary>
        /// <value></value>

        internal ProtectedString Data { get; set; }

        /// <summary>
        /// Gets or sets the username associated with this entry.
        /// </summary>
        /// <value>The username.</value>
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

            var bytes = this.Data.ToArray();
           
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

            return this.Data.ToArray();
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

        /// <summary>
        /// Sets the data the that should be securely stored.
        /// </summary>
        /// <param name="data">The data to be securely stored.</param>
        public void SetBlob(byte[] data) {
            this.Data = new ProtectedString(data);
            this.Length = this.Data.Length;
        }   
        
        /// <summary>
        /// Sets the data using a secure string.
        /// </summary>
        /// <param name="data">The data to be securely stored.</param>
        /// <param name="encoding">The text encoding that should be used. Defaults to UTF-8.</param>
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
                    this.Data = new ProtectedString(bytes);
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

        /// <summary>
        /// Sets the data that should be securely stored.
        /// </summary>
        /// <param name="data">The data to be securely stored.</param>
        /// <param name="encoding">The ecoding that is used to stored the data. Defaults to UTF-8</param>
        public void SetBlob(char[] data, Encoding encoding = null) {
            if(encoding == null) 
                encoding = Encoding.UTF8;
            
            var bytes = encoding.GetBytes(data);
            this.Data = new ProtectedString(bytes);
            Array.Clear(bytes, 0, bytes.Length);
            this.Length = this.Data.Length;
        } 

        /// <summary>
        /// Sets the data should be securely stored.
        /// </summary>
        /// <param name="data">The data to be securely stored.</param>
        /// <param name="encoding">The ecoding that is used to stored the data. Defaults to UTF-8</param>
        public void SetBlob(string data, Encoding encoding = null ) {
            if(encoding == null)
                encoding = Encoding.UTF8;

            var bytes = encoding.GetBytes(data);
            this.Data = new ProtectedString(bytes);
            Array.Clear(bytes, 0, bytes.Length);
            this.Length = this.Data.Length;
        }

        /// <summary>
        /// The length of the data stored.
        /// </summary>
        /// <value></value>
        public int Length { get; set; }
    }
}