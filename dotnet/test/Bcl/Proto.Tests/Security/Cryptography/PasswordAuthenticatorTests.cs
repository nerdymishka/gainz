
using NerdyMishka.Security.Cryptography;
using Xunit;

namespace Tests
{
    public class PasswordAuthenticatorTests
    {
        [Fact]
        public void Test()
        {
            var authenticator = new PasswordAuthenticator();
            var pw = PasswordGenerator.GenerateAsBytes(30);
            var myHash = authenticator.ComputeHash(pw);
            Assert.NotNull(myHash);
            Assert.NotEqual(pw, myHash);

            Assert.True(authenticator.Verify(pw, myHash));
        }
    }
}