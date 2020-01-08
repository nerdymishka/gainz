

using System;
using System.Linq.Expressions;
using System.Security;
using System.Text;
using NerdyMishka.Security.Cryptography;
using NerdyMishka.Text;

namespace NerdyMishka.ComponentModel.ValueConversion
{

  

    public class SecureStringEncryptionConverter : ValueEncryptionConverter<SecureString, string>
    {
        private Encoding encoding;

        public SecureStringEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null) : base(encryptionProvider, encoding)
        {
            this.encoding = encoding ?? Encodings.Utf8NoBom;
        }

        

        
        public override object ConvertFrom(object value)
        {
           if(value == null)
                return null;

            var encryptedValue = (string)value;
            var bytes = this.Decrypt(encryptedValue);
            var chars = this.encoding.GetChars(bytes);
            var ss = new SecureString();
            foreach(var c in chars)
                ss.AppendChar(c);

            bytes.Clear();
            chars.Clear();

            return ss;
        }

        // string
        public override object ConvertTo(object value)
        {
            if(value == null)
                return null;

            var ss = (SecureString)value;
            var bytes = ss.ToBytes(this.encoding);
            return this.EncryptString((byte[])bytes);
        }
    }

    public class StringEncryptionConverter : ValueEncryptionConverter<string, string>
    {
        public StringEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null) : base(encryptionProvider, encoding)
        {
   
        }

        // string
        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            var str = value.ToString();

            if(str == string.Empty)
                return str;

            return this.DecryptString(str);
        }

        // byte
        public override object ConvertTo(object value)
        {
           if(value == null)
                return null;

            var str = value.ToString();

            if(str == string.Empty)
                return str;

            return this.EncryptString((string)value); 
        }
    }

    public class StringToBytesEncryptionConverter : ValueEncryptionConverter<string, byte[]>
    {

        public StringToBytesEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null) : base(encryptionProvider, encoding)
        {
   
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            var bytes = (byte[])value;
            if(bytes.Length == 0)
                return string.Empty;

            return this.DecryptString(bytes);
        }

        // byte
        public override object ConvertTo(object value)
        {
           if(value == null)
                return null;

            var str = value.ToString();
            if(str == string.Empty)
                return Array.Empty<byte>();

            return this.Encrypt(str);
        }
    }

    public class BytesToStringEncryptionConverter : ValueEncryptionConverter<byte[], string>
    {
         public BytesToStringEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null) : base(encryptionProvider, encoding)
        {
   
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            var str = value.ToString();
            if(str == string.Empty)
                return Array.Empty<byte>();

            return this.Decrypt((string)value);
        }

        // byte
        public override object ConvertTo(object value)
        {
            if(value == null)
                return null;

            byte[] bytes = (byte[])value;
            if(bytes.Length == 0)
                return string.Empty;

            return this.EncryptString(bytes);
        }
    }

    public class ByteEncryptionConverter : ValueEncryptionConverter<byte[], byte[]>
    {
        public ByteEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null) : base(encryptionProvider, encoding)
        {
   
        }

        // string
        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            byte[] bytes = (byte[])value;
            if(bytes.Length == 0)
                return bytes;

            return this.Decrypt((byte[])value);
        }

        // byte
        public override object ConvertTo(object value)
        {
           if(value == null)
                return null;

            byte[] bytes = (byte[])value;
            if(bytes.Length == 0)
                return bytes;

            return this.Encrypt((byte[])value);
        }
    }

    public abstract class ValueEncryptionConverter : ValueConverter
    {
        /// <summary>
        /// Gets or sets the encryption provider for children of the <see cref="ValueEncryptionConverter" />
        /// </summary>
        /// <value></value>
        public static ISymmetricEncryptionProvider EncryptionProvider { get; set; }


        private ISymmetricEncryptionProvider encryptionProvider; 


        public void SetEncryptionProvider(ISymmetricEncryptionProvider provider)
        {
            this.encryptionProvider = provider;
        }
    }

    public class ValueEncryptionConverter<TFrom, TTo> : ValueEncryptionConverter
    {

        private ISymmetricEncryptionProvider encryptionProvider; 

        private Encoding encoding;

        static ValueEncryptionConverter()
        {
            // default for testing.  
            var key = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes("!@#dfa2-0daa 2  daax23 xwcxXcwe2");
            EncryptionProvider = new SymmetricEncryptionProvider(new SymmetricEncryptionProviderOptions(){
                Key = key,
                Iterations = 20000
            });
        } 

        public ValueEncryptionConverter(
            ISymmetricEncryptionProvider encryptionProvider = null, 
            Encoding encoding = null)
        {
            this.encryptionProvider = encryptionProvider ?? EncryptionProvider;
            this.encoding = encoding ?? NerdyMishka.Text.Encodings.Utf8NoBom;
        }

        public string EncryptString(byte[] value)
        {
            return Convert.ToBase64String(
                this.Encrypt(value));
        }

        public string EncryptString(string value)
        {
            return Convert.ToBase64String(
                this.Encrypt(value));
        }

        public virtual byte[] Encrypt(byte[] value)
        {
            return EncryptionProvider.Encrypt(value);
        }

        public virtual byte[] Encrypt(string value)
        {
            var bytes = this.encoding.GetBytes(value);
            return EncryptionProvider.Encrypt(bytes);
        }

        public virtual string DecryptString(string value)
        {
            var bytes = this.Decrypt(value);
            return this.encoding.GetString(bytes);
        }

        public virtual string DecryptString(byte[] value)
        {
            var bytes = this.Decrypt(value);
            return this.encoding.GetString(bytes);
        }

        public virtual byte[] Decrypt(byte[] value)
        {
            return EncryptionProvider.Decrypt(value);
        }

        public virtual byte[] Decrypt(string value)
        {
            var bytes = System.Convert.FromBase64String(value);
            return EncryptionProvider.Decrypt(bytes);
        }

        public override bool CanConvertFrom(Type type)
        {
            return type == typeof(TFrom);
        }

        public override bool CanConvertTo(Type type)
        {
            return type == typeof(TTo);
        }

        public override object ConvertFrom(object value)
        {
            return null;
        }

        public override object ConvertTo(object value)
        {
           return null;
        }
    }
}