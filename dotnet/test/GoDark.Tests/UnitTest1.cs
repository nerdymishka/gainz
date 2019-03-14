using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Xunit;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.GoDark.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void LoadCertFromFile()
        {
            var pfx = Env.ResolvePath("./Resources/to-nerdy.messages.nerdymishka.com.pfx");
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfx, "my-great-and-terrible-pw");
            var temp = cert.Export(X509ContentType.Cert);
            System.IO.File.WriteAllBytes(Env.ResolvePath("./Resources/test.cer"), temp);

            Assert.NotNull(cert);

            var bytes = System.Text.Encoding.UTF8.GetBytes("TESTkey");
            var data = cert.GetRSAPublicKey().Encrypt(bytes, RSAEncryptionPadding.Pkcs1);

            Assert.NotEqual(data, bytes);

            var decrypted =  cert.GetRSAPrivateKey().Decrypt(data, RSAEncryptionPadding.Pkcs1);
            Assert.Equal(decrypted, bytes);
        }

      

        [Fact]
        public void Simple()
        {
            var encryptedFile = Env.ResolvePath("./Resources/test-1.txt.godark");
            var decryptedFile = Env.ResolvePath("./Resources/test-1.decrypted.txt");

            if(System.IO.File.Exists(encryptedFile))
            {
                System.IO.File.Delete(encryptedFile);
            }

            if(System.IO.File.Exists(decryptedFile))
            {
                System.IO.File.Delete(decryptedFile);
            }

            var pfx = Env.ResolvePath("./Resources/to-nerdy.messages.nerdymishka.com.pfx");
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfx, "my-great-and-terrible-pw");

            using(var read  = System.IO.File.OpenRead(Env.ResolvePath("./Resources/test-1.txt")))
            using(var write = System.IO.File.OpenWrite(encryptedFile))
            {
                 DataProtection.EncryptStream(read, write, cert.GetRSAPublicKey());
            }

            Assert.True(System.IO.File.Exists(encryptedFile));

            Assert.NotEqual(
                System.IO.File.ReadAllText(encryptedFile),  
                System.IO.File.ReadAllText(Env.ResolvePath("./Resources/test-1.txt")));

            
            using(var read = System.IO.File.OpenRead(encryptedFile))
            using(var write = System.IO.File.OpenWrite(decryptedFile))
            {
                DataProtection.DecryptStream(read, write, cert.GetRSAPrivateKey());
            }


            Assert.True(System.IO.File.Exists(decryptedFile));

             Assert.Equal(
                System.IO.File.ReadAllText(decryptedFile),  
                System.IO.File.ReadAllText(Env.ResolvePath("./Resources/test-1.txt")));

        }


        [Fact]
        public void Zip()
        {
            var encryptedFile = Env.ResolvePath("./Resources/Dev.1.0.0.nupkg.godark");
            var decryptedFile = Env.ResolvePath("./Resources/Dev.1.0.0.nupkg.decrypted");
            var zip = Env.ResolvePath("./Resources/Dev.1.0.0.zip");

            if(System.IO.File.Exists(encryptedFile))
            {
                System.IO.File.Delete(encryptedFile);
            }

            if(System.IO.File.Exists(decryptedFile))
            {
                System.IO.File.Delete(decryptedFile);
            }

            var pfx = Env.ResolvePath("./Resources/to-nerdy.messages.nerdymishka.com.pfx");
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(pfx, "my-great-and-terrible-pw");

            using(var read  = System.IO.File.OpenRead(zip))
            using(var write = System.IO.File.OpenWrite(encryptedFile))
            {
                 DataProtection.EncryptStream(read, write, cert.GetRSAPublicKey());
            }

            Assert.True(System.IO.File.Exists(encryptedFile));
            
            using(var read = System.IO.File.OpenRead(encryptedFile))
            using(var write = System.IO.File.OpenWrite(decryptedFile))
            {
                DataProtection.DecryptStream(read, write, cert.GetRSAPrivateKey());
            }

            Assert.True(System.IO.File.Exists(decryptedFile));
        }


        [Fact]
        public void Zip2()
        {
            var encryptedFile = Env.ResolvePath("./Resources/Dev.1.0.0.nupkg.godark2");
            var decryptedFile = Env.ResolvePath("./Resources/Dev.1.0.0.nupkg.decrypted2");
            var zip = Env.ResolvePath("./Resources/Dev.1.0.0.zip");

            if(System.IO.File.Exists(encryptedFile))
            {
                System.IO.File.Delete(encryptedFile);
            }

            if(System.IO.File.Exists(decryptedFile))
            {
                System.IO.File.Delete(decryptedFile);
            }

            var composite = new CompositeKey();
            composite.AddPassword("my-great-and-terrible-pw");

            using(var read  = System.IO.File.OpenRead(zip))
            using(var write = System.IO.File.OpenWrite(encryptedFile))
            {
                 DataProtection.EncryptStream(read, write, composite);
            }

            Assert.True(System.IO.File.Exists(encryptedFile));
            
            using(var read = System.IO.File.OpenRead(encryptedFile))
            using(var write = System.IO.File.OpenWrite(decryptedFile))
            {
                DataProtection.DecryptStream(read, write, composite);
            }

            Assert.True(System.IO.File.Exists(decryptedFile));
        }
    }
}
