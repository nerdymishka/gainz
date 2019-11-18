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
    public class CharacterKeyFragment : CompositeKeyFragment
    {

        protected CharacterKeyFragment()
        {
            
        }

        public CharacterKeyFragment(SecureString secureString, Encoding encoding = null)
        {
            Check.NotNull(nameof(secureString), secureString);

            this.SetData(this.ConvertToBytes(secureString, encoding));
        }

        public CharacterKeyFragment(string value, Encoding encoding = null)
        {
            Check.NotNullOrWhiteSpace(nameof(value), value);

            this.SetData(this.ConvertToBytes(value, encoding));
        }

        public CharacterKeyFragment(char[] value, Encoding encoding = null)
        {
            Check.NotNull(nameof(value), value);

            this.SetData(this.ConvertToBytes(value, encoding));
        }

        public CharacterKeyFragment(ReadOnlySpan<char> value, Encoding encoding = null)
        {
            if(value.Length == 0)
                throw new ArgumentNullOrEmptyException(nameof(value));

            this.SetData(this.ConvertToBytes(value, encoding));
        }

        protected virtual byte[] ConvertToBytes(SecureString secureString, Encoding encoding = null)
        {
            var bytes = secureString.ToBytes(encoding);
            return this.ApplyTransform(bytes);
        }

        protected virtual byte[] ConvertToBytes(string value, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encodings.Utf8NoBom).GetBytes(value);
            return this.ApplyTransform(bytes);
        }

        protected virtual byte[] ConvertToBytes(ReadOnlySpan<char> value, Encoding encoding = null)
        {
            var chars = value.ToArray();
            var bytes = (encoding ?? Encodings.Utf8NoBom).GetBytes(chars);
            chars.Clear();
            return this.ApplyTransform(bytes);
        }

        protected virtual byte[] ConvertToBytes(char[] value, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encodings.Utf8NoBom).GetBytes(value);
            return this.ApplyTransform(bytes);
        }

        protected virtual byte[] ApplyTransform(byte[] data)
        {
            return data;
        }

     

    }
}
