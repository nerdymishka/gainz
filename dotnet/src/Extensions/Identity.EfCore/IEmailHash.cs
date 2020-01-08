
namespace NerdyMishka.Identity
{
    public interface IEmailHash
    {
        string ComputeHash(string value, byte[] salt);
    }
}