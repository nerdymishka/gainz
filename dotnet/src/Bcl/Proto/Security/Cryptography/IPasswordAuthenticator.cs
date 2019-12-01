
namespace NerdyMishka.Security.Cryptography
{
    public interface IPasswordAuthenticator
    {
        byte[] ComputeHash(byte[] value);

        bool Verify(byte[] value, byte[] hash);
    }
}