
namespace NerdyMishka.Security.Cryptography
{
    public interface IHashProvider
    {
        byte[] ComputeHash(byte[] value);
    }
}