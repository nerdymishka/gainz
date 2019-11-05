using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NerdyMishka.Flex;

namespace EfCore.Relational.Storage.ValueConversion
{
    public class HashBinaryConverter : ValueConverter<byte[], byte[]>
    {
        public HashBinaryConverter(
             IFlexHashProvider provider, int iterations = 64000
            ) : base(
                v => provider.ComputeHash(v, iterations),
                v => provider.ComputeHash(v, iterations))
        {

        }
    }
}