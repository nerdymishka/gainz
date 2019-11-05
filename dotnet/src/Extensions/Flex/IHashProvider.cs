

namespace NerdyMishka.Flex
{
    public interface IFlexHashProvider
    {
        string ComputeHash(string value, int iterations = 64000);

        char[] ComputeHash(char[] value, int iterations = 64000);

        byte[] ComputeHash(byte[] blob, int iterations = 64000);

          
    }
}