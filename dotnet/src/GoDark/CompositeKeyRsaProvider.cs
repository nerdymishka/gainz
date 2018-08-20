using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyRsaProvider : CompositeKeyFragment
    {
        private const string signature = "P2bOiuVLdMo2eSZ95y3Yi8bMumrZIWzQBV1otF2N";

        public CompositeKeyRsaProvider(X509Certificate2 certificate, HashAlgorithm hash = null)
        {
            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(signature);

#if NET451
            var rsa = ((RSACryptoServiceProvider)certificate.PrivateKey);
            var parameters = rsa.ExportParameters(true);
            var data = rsa.SignData(bytes, parameters);  
                  
#else
            var data = certificate.GetRSAPrivateKey().SignData(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
#endif             
            var key = hash.ComputeHash(data);
            data.Clear();

            this.SetData(key);

            if (createdHash)
                hash.Dispose();
        }


        public CompositeKeyRsaProvider(X509Certificate2 certificate, string file, HashAlgorithm hash = null)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException(nameof(file));

            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

            var data = CompositeKeyFileProvider.GetDataFromFile(file);

#if NET451
            data = ((RSACryptoServiceProvider)certificate.PrivateKey).Decrypt(data, true);
#else
            data = certificate.GetRSAPrivateKey().Decrypt(data, RSAEncryptionPadding.OaepSHA256);
#endif 
            var key = hash.ComputeHash(data);
            data.Clear();

            this.SetData(key);

            if (createdHash)
                hash.Dispose();
        }

        public CompositeKeyRsaProvider(X509Certificate2 certificate, byte[] encryptedKey, HashAlgorithm hash = null)
        {
            bool createdHash = false;
            if (hash == null)
            {
                createdHash = true;
                hash = SHA256.Create();
            }

#if NET451
            var data = ((RSACryptoServiceProvider)certificate.PrivateKey).Decrypt(encryptedKey, true);
#else
            var data = certificate.GetRSAPrivateKey().Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
#endif 
            var key = hash.ComputeHash(data);
            data.Clear();

            this.SetData(key);

            if (createdHash)
                hash.Dispose();
        }


        public static byte[] Encrypt(X509Certificate2 certificate, byte[] key)
        {
#if NET451
            var data = ((RSACryptoServiceProvider)certificate.PrivateKey).Encrypt(key, true);
#else
            var data = certificate.GetRSAPrivateKey().Encrypt(key, RSAEncryptionPadding.OaepSHA256);
#endif   
            return data;
        }


        public static void Generate(X509Certificate2 certificate, string path, byte[] key, HashAlgorithm hash = null)
        {
#if NET451
            var data = ((RSACryptoServiceProvider)certificate.PrivateKey).Encrypt(key, true);
#else
            var data = certificate.GetRSAPrivateKey().Encrypt(key, RSAEncryptionPadding.OaepSHA256);
#endif   
          
            CreateFile(path, data);
        }

        private static void CreateFile(string path, byte[] data)
        {
            var enc = new UTF8Encoding(false, false);
            var content = $"key: {Convert.ToBase64String(data)}";
            File.WriteAllText(path, content, enc);
        }

    }
}
