using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NerdyMishka.EfCore.Storage.ValueConversion;
using NerdyMishka.Flex;

namespace NerdyMishka.EfCore.Metadata
{
    public class EncryptPropertyConvention : IPropertyConvention
    {
        private IFlexCryptoProvider cryptoProvider;
        private IFlexHashProvider hashProvider;

        public EncryptPropertyConvention(IFlexCryptoProvider cryptoProvider, IFlexHashProvider hashProvider)
        {
            this.cryptoProvider = cryptoProvider;
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
                    mutableProperty.SetValueConverter(new EncryptedStringConverter(cryptoProvider));
                    unsupported = false;
                }

                if(pt == typeof(byte[])) {
                    mutableProperty.SetValueConverter(new EncryptedBinaryConverter(cryptoProvider));
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
                    mutableProperty.SetValueConverter(new HashStringConverter(hashProvider));
                    unsupported = false;
                }

                if(unsupported)
                    throw new NotSupportedException($"Encryption is not supported for property type {pt.FullName}");
            }
        }
    }
}