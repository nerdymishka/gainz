namespace NerdyMishka.Security.Cryptography
{
    public interface IEncryptionProvider
    {
        byte[] Encrypt(byte[] data);

        byte[] Decrypt(byte[] encryptedData);
    }
}