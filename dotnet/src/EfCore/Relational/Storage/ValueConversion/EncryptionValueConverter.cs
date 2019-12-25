
using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NerdyMishka.Security.Cryptography;
using System.Text;
using System.Security;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class SecureStringEncryptionConverter : ValueEncryptionConverter<SecureString, string>
    {
        public SecureStringEncryptionConverter()
            :base(
            (to) => ValueEncryptionProvider.EncryptString(to), 
            (from) =>ValueEncryptionProvider.DecryptSecureString(from))
        {
          
        }
    }

    public class StringEncryptionConverter : ValueEncryptionConverter<string, string>
    {
        public StringEncryptionConverter(
            ) : base(
                (to) => ValueEncryptionProvider.EncryptString(to),
                (from) => ValueEncryptionProvider.DecryptString(from)
            )
        {
        }

        
    }

    public class StringToBytesEncryptionConverter : ValueEncryptionConverter<string, byte[]>
    {

        public StringToBytesEncryptionConverter(
            ) : base(
                (to) => ValueEncryptionProvider.Encrypt(to), 
                (from) => ValueEncryptionProvider.DecryptString(from))
        {
   
        }

        
    }

    public class BytesToStringEncryptionConverter : ValueEncryptionConverter<byte[], string>
    {
         public BytesToStringEncryptionConverter(
         
            ) : base((to) => ValueEncryptionProvider.EncryptString(to),
            (from) => ValueEncryptionProvider.Decrypt(from))
        {
   
        }

    }

    public class ByteEncryptionConverter : ValueEncryptionConverter<byte[], byte[]>
    {
        public ByteEncryptionConverter() : base(
                (to) => ValueEncryptionProvider.Encrypt(to),
                (from) =>  ValueEncryptionProvider.Decrypt(from),
                null
            )
        {
            
        }
    }

    public static class ValueEncryptionProvider
    {
        public static ISymmetricEncryptionProvider EncryptionProvider { get; set; }

        public static string EncryptString(SecureString ss)
        {
            var bytes = ss.ToBytes();
            return EncryptString(bytes);
        }

        public static string EncryptString(byte[] value)
        {
            return Convert.ToBase64String(
                Encrypt(value));
        }

        public static string EncryptString(string value)
        {
            return Convert.ToBase64String(
                Encrypt(value));
        }

        public static byte[] Encrypt(byte[] value)
        {
            return EncryptionProvider.Encrypt(value);
        }

        public static byte[] Encrypt(string value)
        {
            var bytes = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes(value);
            return EncryptionProvider.Encrypt(bytes);
        }

        public static SecureString DecryptSecureString(string value)
        {
            var bytes = Decrypt(value);
            var chars = NerdyMishka.Text.Encodings.Utf8NoBom.GetChars(bytes);
            var ss = new SecureString();
            foreach(var c in chars)
                ss.AppendChar(c);

            chars.Clear();
            bytes.Clear();

            return ss;
        }

        public static string DecryptString(string value)
        {
            var bytes = Decrypt(value);
            return  NerdyMishka.Text.Encodings.Utf8NoBom.GetString(bytes);
        }

        public static string DecryptString(byte[] value)
        {
            var bytes = Decrypt(value);
            return NerdyMishka.Text.Encodings.Utf8NoBom.GetString(bytes);
        }

        public static byte[] Decrypt(byte[] value)
        {
            return EncryptionProvider.Decrypt(value);
        }

        public static byte[] Decrypt(string value)
        {
            var bytes = System.Convert.FromBase64String(value);
            return EncryptionProvider.Decrypt(bytes);
        }
    }


    public abstract class ValueEncryptionConverter<TFrom, TTo> : ValueConverter<TFrom, TTo>
    {

        public static ISymmetricEncryptionProvider EncryptionProvider { get; set; }


        static ValueEncryptionConverter()
        {
            var key = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes("!@#dfa2-0daa 2  daax23 xwcxXcwe2");
            ValueEncryptionProvider.EncryptionProvider = new SymmetricEncryptionProvider(new SymmetricEncryptionProviderOptions(){
                Key = key,
                Iterations = 20000
            });
        }

        public ValueEncryptionConverter(
            Expression<Func<TFrom, TTo>> convertToProviderExpression, 
            Expression<Func<TTo, TFrom>> convertFromProviderExpression, 
            ConverterMappingHints mappingHints = null) : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
        {

        }


       
    }
}