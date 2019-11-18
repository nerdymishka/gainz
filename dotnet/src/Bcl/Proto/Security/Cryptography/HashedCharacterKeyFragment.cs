using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Security.Cryptography;
using NerdyMishka.Text;
using NerdyMishka.Validation;

namespace NerdyMishka.Security.Cryptography
{
    public class HashedCharacterKeyFragment : CharacterKeyFragment, IDisposable
    {
        protected HashAlgorithm algorithm;

        public HashedCharacterKeyFragment(
            SecureString secureString, 
            Encoding encoding = null, 
            HashAlgorithm algorithm = null)
        {
            Check.NotNull(nameof(secureString), secureString);
            this.algorithm = algorithm;

            this.SetData(this.ConvertToBytes(secureString, encoding));
        }

     

        public HashedCharacterKeyFragment(
            string value, 
            Encoding encoding = null,
            HashAlgorithm algorithm = null)
        {
            Check.NotNullOrWhiteSpace(nameof(value), value);
            this.algorithm = algorithm;

            this.SetData(this.ConvertToBytes(value, encoding));
        }

        public HashedCharacterKeyFragment(
            char[] value, 
            Encoding encoding = null,
            HashAlgorithm algorithm = null)
        {
            Check.NotNull(nameof(value), value);
            this.algorithm = algorithm;

            this.SetData(this.ConvertToBytes(value, encoding));
        }

        public HashedCharacterKeyFragment(
            ReadOnlySpan<char> value, 
            Encoding encoding = null,
            HashAlgorithm algorithm = null)
        {
            if(value.Length == 0)
                throw new ArgumentNullOrEmptyException(nameof(value));
            this.algorithm = algorithm;

            this.SetData(this.ConvertToBytes(value, encoding));
        }


      

        protected  override byte[] ApplyTransform(byte[] data)
        {
            this.algorithm = this.algorithm ?? SHA256.Create();
            var bytes = this.algorithm.ComputeHash(data);
            return bytes;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if(disposing)
            {
                this.algorithm?.Dispose();
            }
        }

    }
}
