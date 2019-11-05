
using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using NerdyMishka.Flex;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class EncryptedBinaryConverter : ValueConverter<byte[], byte[]>
    {
        public EncryptedBinaryConverter(
             IFlexCryptoProvider provider
            ) : base(
                v => provider.EncryptBlob(v, null),
                v => provider.DecryptBlob(v, null))
        {
            
        }
    }
}