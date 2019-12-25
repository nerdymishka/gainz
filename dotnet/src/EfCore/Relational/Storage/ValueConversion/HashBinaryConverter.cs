using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NerdyMishka.Security.Cryptography;

namespace EfCore.Relational.Storage.ValueConversion
{
    public class HashBinaryConverter : ValueConverter<byte[], byte[]>
    {
        public HashBinaryConverter(
             IHashProvider provider
            ) : base(
                v => provider.ComputeHash(v),
                v => v)
        {

        }
    }
}