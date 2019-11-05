using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;

namespace Kryptos
{

    public class CertificateOperations
    {

        public static RSA ImportPublicKey(Uri cert)
        {
            if(cert.IsFile)
            {
                return ImportPublicKey(cert.LocalPath);
            }

            var client = new System.Net.WebClient();
            var data = client.DownloadString(cert);
            var lines = data.Split('\n');
            foreach(var line in lines)
            {
                if(line.StartsWith("ssh-rsa"))
                {
                    var file = $"{Settings.HomeDir}/.config/kryptos/temp.pub";
                    System.IO.File.WriteAllText(file, line);

                    var cert2 = ImportPublicKey(file);
                    File.Delete(file);
                    return cert2;
                }
            }

            return null;
        }


        private class BinaryPasswordFinder : IPasswordFinder
        {
            private byte[] data;
            public BinaryPasswordFinder(byte[] data)
            {
                this.data = data;
            }

            public char[] GetPassword()
            {
                var chars = System.Text.Encoding.UTF8.GetChars(this.data);
                                

                return chars;
            }
        }

        
        public static RSA ReadPrivateKeyFromPem(string pem)
        {
            using (TextReader privateKeyTextReader = new StringReader(pem))  
            {  
                var reader  = new PemReader(privateKeyTextReader);
         
                var readKeyPair = (AsymmetricCipherKeyPair)reader.ReadObject();  
                var privateKeyParams = ((RsaPrivateCrtKeyParameters)readKeyPair.Private);  
                var cryptoServiceProvider = new RSACryptoServiceProvider();  
                var parameters = new RSAParameters();  
  
                parameters.Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned();  
                parameters.P = privateKeyParams.P.ToByteArrayUnsigned();  
                parameters.Q = privateKeyParams.Q.ToByteArrayUnsigned();  
                parameters.DP = privateKeyParams.DP.ToByteArrayUnsigned();  
                parameters.DQ = privateKeyParams.DQ.ToByteArrayUnsigned();  
                parameters.InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned();  
                parameters.D = privateKeyParams.Exponent.ToByteArrayUnsigned();  
                parameters.Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned();  
  
                cryptoServiceProvider.ImportParameters(parameters);  
  
                return cryptoServiceProvider;  
            }
        }

        public static RSA ReadPrivateKeyFromPem(string pem, byte[] password)
        {
            pem = pem.Replace("\n", "\r\n");
            using (TextReader privateKeyTextReader = new StringReader(pem))  
            {  
                //throw new Exception(pem);
               
                PemReader pemReader = new PemReader(privateKeyTextReader, new BinaryPasswordFinder(password));
                var privateKeyObject = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                var privateKeyParams = (RsaPrivateCrtKeyParameters)privateKeyObject.Private;
  
                
                var cryptoServiceProvider = new RSACryptoServiceProvider();  
                var parameters = new RSAParameters();  

        
                parameters.Modulus = privateKeyParams.Modulus.ToByteArrayUnsigned();  
                parameters.P = privateKeyParams.P.ToByteArrayUnsigned();  
                parameters.Q = privateKeyParams.Q.ToByteArrayUnsigned();  
                parameters.DP = privateKeyParams.DP.ToByteArrayUnsigned();  
                parameters.DQ = privateKeyParams.DQ.ToByteArrayUnsigned();  
                parameters.InverseQ = privateKeyParams.QInv.ToByteArrayUnsigned();  
                parameters.D = privateKeyParams.Exponent.ToByteArrayUnsigned();  
                parameters.Exponent = privateKeyParams.PublicExponent.ToByteArrayUnsigned();  
  
                cryptoServiceProvider.ImportParameters(parameters);  
  
                return cryptoServiceProvider;  
            }
        }

 
        public static RSA ReadPublicKeyFromPem(String pem)  
        {  
            using (TextReader publicKeyTextReader = new StringReader(pem))  
            {  
                var reader = new PemReader(publicKeyTextReader);
                var publicKeyParam = (RsaKeyParameters)reader.ReadObject();  
  
                var cryptoServiceProvider = new RSACryptoServiceProvider();  
                var parameters = new RSAParameters();  
  
  
  
                parameters.Modulus = publicKeyParam.Modulus.ToByteArrayUnsigned();  
                parameters.Exponent = publicKeyParam.Exponent.ToByteArrayUnsigned();  
  
                cryptoServiceProvider.ImportParameters(parameters);  
  
                return (RSA)cryptoServiceProvider;  
            }         
        }

        public static RSA ImportPublicKey(string cert)
        {
            if(cert.EndsWith(".pub"))
            {
                var fullPath = Env.GetFullPath("ssh-keygen.exe");
                if(string.IsNullOrEmpty(fullPath))
                {
                    throw new FileNotFoundException(
                        "To convert a .pub file to a .pem file, kryptos requires ssh-keygen.exe to exist on the path",
                        "ssh-keygen.exe");
                }

               var p = new System.Diagnostics.Process();
                p.StartInfo.CreateNoWindow= true;
                p.StartInfo.FileName = "ssh-keygen.exe";
                var dest = cert.Replace(".pub", ".pem");
                p.StartInfo.Arguments = $"-f \"{cert}\" -m pem -e";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.Start();
                
                p.WaitForExit();
                var pem = p.StandardOutput.ReadToEnd();
                return ReadPublicKeyFromPem(pem);
            }

            var bytes = File.ReadAllBytes(cert);
            return new X509Certificate2(bytes)
                .GetRSAPublicKey();
        }

        public static X509Certificate2 FindCertificateByThumbprint(string thumbprint)
        {
            using(var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var set = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

                if(set == null || set.Count == 0)
                    return null;

                return set[0];
            }
        }

        public static X509Certificate2 FindCertificateBySubject(string subject)
        {
            using(var store = new X509Store(StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);
                var set = store.Certificates.Find(X509FindType.FindBySubjectName, subject, false);

                if(set == null || set.Count == 0)
                    return null;

                return set[0];
            }
        }

        public static X509Certificate2 ReadCertificate(string path, string password)
        {
            path = FileOperations.ResolvePath(path);

            return ReadCertificate(path, password);
        }

        public static X509Certificate2 ReadCertificate(string path, byte[] password, bool clear = false)
        {
            var chars = System.Text.Encoding.UTF8.GetChars(password);
            var ss = new SecureString();
            foreach(var c in chars)
                ss.AppendChar(c);

            Array.Clear(chars, 0, chars.Length);
            
            if(clear)
                Array.Clear(password, 0, password.Length);

            return ReadCertificate(path, ss);
        }

        public static X509Certificate2 ReadCertificate(string path, SecureString password)
        {
            path = FileOperations.ResolvePath(path);

            return new X509Certificate2(path, password);
        }
    }
}