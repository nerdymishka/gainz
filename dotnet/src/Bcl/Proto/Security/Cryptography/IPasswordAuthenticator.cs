
namespace NerdyMishka.Security.Cryptography
{
    public interface IPasswordAuthenticator : IHashProvider
    {
        bool Verify(byte[] value, byte[] hash);
    }
}