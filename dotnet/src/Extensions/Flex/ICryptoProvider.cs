

namespace NerdyMishka.Flex
{
    public interface IFlexCryptoProvider
    {
        string EncryptString(string value, byte[] privateKey = null);

        byte[] EncryptBlob(byte[] blob, byte[] privateKey = null);

        string DecryptString(string value, byte[] privateKey = null);

        byte[] DecryptBlob(byte[] blob, byte[] privateKey = null);       
    }
}