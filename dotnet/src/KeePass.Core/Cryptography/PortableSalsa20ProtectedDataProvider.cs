using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass.Cryptography
{
    public class PortableSalsa20ProtectedDataProvider : IProtectedDataProvider
    {
        private Salsa20 salsa20;
        private byte[] key;

        public static void Setup()
        {
            var provider = MasterKeyUserAccount.Provider = null;
            MasterKeyUserAccount.Bin = null;

            provider = new PortableSalsa20ProtectedDataProvider();
            ProtectedMemoryBinary.DataProtectionAction = (data, state, operation) =>
            {
                var protectedData = (ProtectedMemoryBinary)state;
                if (operation == DataProtectionActionType.Encrypt)
                    return provider.ProtectData(data, protectedData.Id);
                else
                    return provider.UnprotectData(data, protectedData.Id);
            };
        }

        public PortableSalsa20ProtectedDataProvider()
        {
            this.salsa20 = Salsa20.Create();
            salsa20.Rounds = Salsa20Rounds.Ten;
            //salsa20.SkipXor = true;
            salsa20.GenerateKey();
            this.key = salsa20.Key;
        }

        public byte[] ProtectData(byte[] data, byte[] optionalEntropy, bool isLocalMachine = false)
        {
            var buffer = new byte[data.Length];
            using (var transform =
                salsa20.CreateEncryptor(key, optionalEntropy))
            {
                transform.TransformBlock(data, 0, data.Length, buffer, 0);
            }

            return buffer;
        }

        public byte[] UnprotectData(byte[] data, byte[] optionalEntropy, bool isLocalMachine = false)
        {
            var buffer = new byte[data.Length];
            using (var transform = salsa20.CreateDecryptor(key, optionalEntropy))
            {
                transform.TransformBlock(data, 0, data.Length, buffer, 0);
            }
           
            return buffer;
        }
    }
}
