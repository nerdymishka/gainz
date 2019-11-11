

using Xunit;
using NerdyMishka;

namespace Tests 
{
    public class MurMurHash3Tests
    {
        [Fact]
        public static void Verify_Consistent_Hash32()
        {
            var bytes = new byte[16];
            var result = MurMurHash3.ComputeHash(bytes, 1234);
            var result2 = MurMurHash3.ComputeHash(bytes, 1234);
            Assert.Equal(-1726509725, result);
            Assert.Equal(-1726509725, result2);
        }
    }
}