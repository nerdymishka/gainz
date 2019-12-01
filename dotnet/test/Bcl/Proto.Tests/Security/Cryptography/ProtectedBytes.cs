
using NerdyMishka.Security.Cryptography;
using Xunit;

namespace Tests 
{
    public class ProtectedBytesTests
    {

        [Fact]
        public void Ctor()
        {
            var bytes = new ProtectedBytes();
            Assert.Equal(0, bytes.Length);
        }
    }
}