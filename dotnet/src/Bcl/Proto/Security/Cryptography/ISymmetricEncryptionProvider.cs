using System;

namespace NerdyMishka.Security.Cryptography
{
    public interface ISymmetricEncryptionProvider : IDisposable 
    {
        byte[] Encrypt(
            byte[] data, 
            byte[] privateKey = null, 
            byte[] symmetricKey = null,
            IEncryptionProvider symmetricKeyEncryptionProvider = null);     

        byte[] Decrypt(
            byte[] data, 
            byte[] privateKey = null, 
            IEncryptionProvider symmetricKeyEncryptionProvider = null);  
    }
}