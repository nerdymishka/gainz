
using System;
using System.Security;
using System.Text;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.ComponentModel.ValueConversion
{

    public class SecureStringHashConverter: ValueHashConverter<SecureString, string>
    {
        private Encoding encoding;

        public SecureStringHashConverter(
            IPasswordAuthenticator authenticator = null, 
            Encoding encoding = null)
            :base(authenticator, encoding)
        {
           this.encoding = encoding ?? NerdyMishka.Text.Encodings.Utf8NoBom;
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            var ss = (SecureString)value;
            var bytes = ss.ToBytes(this.encoding);
            var hash = this.ComputeHashAsString((char[])value);

            bytes.Clear();
            return hash;
        }

        public override object ConvertTo(object value)
        {
            return value;
        }
    }


    public class CharsHashConverter: ValueHashConverter<char[], string>
    {
        public CharsHashConverter(
            IPasswordAuthenticator authenticator = null, 
            Encoding encoding = null)
            :base(authenticator, encoding)
        {
           
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            return this.ComputeHashAsString((char[])value);
        }

        public override object ConvertTo(object value)
        {
            return value;
        }
    }

    public class BytesHashConverter: ValueHashConverter<byte[], string>
    {
        public BytesHashConverter(
            IPasswordAuthenticator authenticator = null, 
            Encoding encoding = null)
            :base(authenticator, encoding)
        {
           
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            return this.ComputeHashAsString((byte[])value);
        }

        public override object ConvertTo(object value)
        {
            return value;
        }
    }


    public class StringHashConverter: ValueHashConverter<string, string>
    {
        public StringHashConverter(
            IPasswordAuthenticator authenticator = null, 
            Encoding encoding = null)
            :base(authenticator, encoding)
        {
           
        }

        public override object ConvertFrom(object value)
        {
            if(value == null)
                return null;

            return this.ComputeHashAsString((string)value);
        }

        public override object ConvertTo(object value)
        {
            return value;
        }
    }


    public abstract class ValueHashConverter<TFrom, TTo> : ValueConverter
    {
        public static IPasswordAuthenticator Authenticator { get; set; }

        private IPasswordAuthenticator authenticator;
        private Encoding encoding;

        public ValueHashConverter(IPasswordAuthenticator authenticator = null, Encoding encoding = null)
        {
            this.authenticator = authenticator ?? new PasswordAuthenticator();
            this.encoding = encoding ?? NerdyMishka.Text.Encodings.Utf8NoBom;
        }

        public virtual string ComputeHashAsString(string value)
        {
            var bytes = this.ComputeHash(value);
            return Convert.ToBase64String(bytes);
        }

        public virtual string ComputeHashAsString(byte[] value)
        {
            var bytes = this.ComputeHash(value);
            return Convert.ToBase64String(bytes);
        }

        public virtual string ComputeHashAsString(char[] value)
        {
            var bytes = this.ComputeHash(value);
            return Convert.ToBase64String(bytes);
        }
        

        public virtual byte[] ComputeHash(string value)
        {
            var bytes = this.encoding.GetBytes(value);
            return this.authenticator.ComputeHash(bytes);
        }

        public virtual byte[] ComputeHash(char[] value)
        {
            var bytes = this.encoding.GetBytes(value);
            return this.authenticator.ComputeHash(bytes);
        }

        public virtual byte[] ComputeHash(byte[] value)
        {
            return this.authenticator.ComputeHash(value);
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