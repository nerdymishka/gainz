using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NerdyMishka.ComponentModel.DataAnnotations;
using NerdyMishka.EfCore.Storage.ValueConversion;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.EfCore.Metadata
{
    public class EncryptPropertyConvention : IPropertyConvention
    {
        private IHashProvider hashProvider;
       
        public EncryptPropertyConvention(
            ISymmetricEncryptionProvider provider,
            IHashProvider hashProvider)
        {
            ValueEncryptionProvider.EncryptionProvider = provider;
            this.hashProvider = hashProvider;
        }

        public void Apply(IMutableProperty mutableProperty)
        {
            var attrs = mutableProperty.PropertyInfo.GetCustomAttributes(true);
            if(attrs.Any(o => o is EncryptAttribute))
            {
                bool unsupported = true;
                var pt = mutableProperty.PropertyInfo.PropertyType;
                if(pt == typeof(string)) {
                    mutableProperty.SetValueConverter(new StringEncryptionConverter());
                    unsupported = false;
                }

                if(pt == typeof(byte[])) {
                    mutableProperty.SetValueConverter(new ByteEncryptionConverter());
                    unsupported = false;
                }

                if(unsupported)
                    throw new NotSupportedException($"Encryption is not supported for property type {pt.FullName}");
            }

            if(attrs.Any(o => o is HashAttribute))
            {
                bool unsupported = true;
                var pt = mutableProperty.PropertyInfo.PropertyType;

                if(pt == typeof(String)) {
                    mutableProperty.SetValueConverter(new HashStringConverter(this.hashProvider));
                    unsupported = false;
                }

                if(unsupported)
                    throw new NotSupportedException($"Encryption is not supported for property type {pt.FullName}");
            }
        }
    }
}