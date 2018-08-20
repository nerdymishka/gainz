using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NerdyMishka.KeePass.Cryptography;
#if !NETCOREAPP
using System.Security.Cryptography;

#endif

namespace NerdyMishka.KeePass
{ 
    public class MasterKeyUserAccount : MasterKeyFragment
    {
        private static byte[] s_entropy = new byte[] {
            0xDE, 0x13, 0x5B, 0x5F,
            0x18, 0xA3, 0x46, 0x70,
            0xB2, 0x57, 0x24, 0x29,
            0x69, 0x88, 0x98, 0xE6
        };

        public static string Bin { get; set; }
        public static IProtectedDataProvider Provider { get; set; }

        static MasterKeyUserAccount()
        {
           
        }


        public MasterKeyUserAccount(string keyLocation = null)
        {
            if (Provider == null)
                throw new InvalidOperationException("MasterKeyUserAccount.Provider must be set before creating a MasterKeyUserAccount instnace");

            this.GenerateKey(keyLocation);
        }


        public void GenerateKey(string keyLocation)
        {
            var filePath = string.IsNullOrWhiteSpace(keyLocation) ? GetKeyFilePath() : keyLocation;
            var dir = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            byte[] key = null;
            if(File.Exists(filePath))
                key = this.GetKey(filePath);

            if (key == null)
                key = this.SetKey(filePath);

            this.SetData(key);
        }

        private static string GetKeyFilePath()
        {
            var roaming = Environment.GetEnvironmentVariable("APPDATA");
            if (string.IsNullOrWhiteSpace(roaming))
            {
                roaming = Environment.GetEnvironmentVariable("HOME");
        
            }

            if(string.IsNullOrWhiteSpace(roaming))
                throw new InvalidProgramException("Could not determine home directory");

            roaming = System.IO.Path.Combine(roaming, "KeePass", Bin);

            return roaming;
        }

        private byte[] GetKey(string filepath)
        {

            byte[] bytes = File.ReadAllBytes(filepath);

           return Provider.UnprotectData(bytes, s_entropy);
        }

        private byte[] SetKey(string filepath)
        {
            byte[] key = null;
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var randomByteGenerator = RandomByteGeneratorFactory.GetGenerator(2);
                var initializer = new byte[32];
                randomByteGenerator.Initialize(initializer);
                key = randomByteGenerator.NextBytes(64);
            }
               
            var bytes = Provider.ProtectData(key, s_entropy);

            File.WriteAllBytes(filepath, bytes);
            bytes.Clear();

            return key;
        }
    }
}
