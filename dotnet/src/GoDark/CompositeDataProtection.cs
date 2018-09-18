using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public static class CompositeDataProtection
    {
        public static string EncryptString(string value, CompositeKey key, Encoding encoding = null)
        {
            if(value == null)
                return null;

            if(key == null)
                throw new ArgumentNullException(nameof(key));

            if(encoding == null)
                encoding = Encoding.UTF8;

            var bytes = EncryptBlob(encoding.GetBytes(value), key);
            return Convert.ToBase64String(bytes);
        }

        public static byte[] EncryptBlob(byte[] blob, CompositeKey key)
        {
            if(blob == null || blob.Length == 0)
                return null;

            if(key == null)
                throw new ArgumentNullException(nameof(key));

            var symKey = PasswordGenerator.GenerateAsBytes(32);
        
            var privateKey = key.AssembleKey(symKey);

            var encryptedBytes = DataProtection.EncryptBlob(blob, privateKey);
            privateKey.Clear();

            var result = new byte[encryptedBytes.Length + 32];
            Array.Copy(symKey, 0, result, 0, 32);
            Array.Copy(encryptedBytes, 0, result, 32, encryptedBytes.Length);
            //symKey.Clear();
            encryptedBytes.Clear();
            return result;
        }

        public static byte[] DecryptBlob(byte[] encryptedBlob, CompositeKey key)
        {
            if(encryptedBlob == null || encryptedBlob.Length == 0)
                return null;

            if(key == null)
                throw new ArgumentNullException(nameof(key));

            var symmetricKey = new byte[32];
            var encryptedBytes = new byte[encryptedBlob.Length - 32];
            Array.Copy(encryptedBlob, 0, symmetricKey, 0, 32);
            Array.Copy(encryptedBlob, 32, encryptedBytes, 0, encryptedBytes.Length);

            var privateKey = key.AssembleKey(symmetricKey);
           
            symmetricKey.Clear();
            var decrypted = DataProtection.DecryptBlob(encryptedBytes, privateKey);
            encryptedBytes.Clear();

            return decrypted;
        }

        public static string DecryptString(string encryptedString, CompositeKey key, Encoding encoding = null)
        {
            if(encryptedString == null || encryptedString.Length == 0)
                return null;

            if(key == null)
                throw new ArgumentNullException(nameof(key));

            if(encoding == null)
                encoding = Encoding.UTF8;

            var encryptedBlob = Convert.FromBase64String(encryptedString);
            var decryptedBytes = DecryptBlob(encryptedBlob, key);
            encryptedBlob.Clear();
            var value = encoding.GetString(decryptedBytes);
            decryptedBytes.Clear();

            return value;
        }

    }
}