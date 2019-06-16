
using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using NerdyMishka.Flex;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class EncryptedStringConverter : ValueConverter<string, string>
    {
        public EncryptedStringConverter(
             IFlexCryptoProvider provider
            ) : base(
                v => provider.EncryptString(v, null),
                v => provider.DecryptString(v, null))
        {

        }
    }
}