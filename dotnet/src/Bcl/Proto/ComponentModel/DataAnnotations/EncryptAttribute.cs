using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.ComponentModel.DataAnnotations

{

    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EncryptAttribute : ValueConverterAttribute
    {
        public string Encoding { get; set; }

        public static List<ValueConverter> Converters { get; } = 
            new List<ValueConversion.ValueConverter>() {
                new StringEncryptionConverter(),
                new ByteEncryptionConverter(),
                new SecureStringEncryptionConverter(),
                new StringToBytesEncryptionConverter(),
                new BytesToStringEncryptionConverter(),
            };

        public EncryptAttribute(Type valueConverter = null, string encoding = null) :base(valueConverter)
        {
            this.Encoding = encoding ?? "UTF-8";
        }

      
    }

}


   