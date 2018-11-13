using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CommandLine;

namespace Kryptos
{
    public class FileDecryptCommand : Command
    {
        public override string Section => "file";
        
        public override string Action => "decrypt";
        
        [Option('k', "key", HelpText = "Optional. The path to the private key (.pem or .pfx)")]
        public string PrivateKey { get; set;}
        

        [Option('s', "src", HelpText = "Required. The path for the encrypted file or folder with files.")]
        [Value(0)]
        public string Source  { get; set; }

        public string SourceOrdinal { get; set; }

        [Option('o', "dest", HelpText = "Optional. The path for the decrypted file or for decrypted files.")]
        [Value(1)]
        public string Destination { get; set; }

        [Option('f', "force", HelpText = "Optional. forces the command to run")]
        public bool Force { get; set; }


        public override bool Run(Config config)
        {
            

            var certValues = new Config.CertificateSection() {
                Path = config.DefaultCertificate.Path,
                Password = config.DefaultCertificate.Password,
                Thumbprint = config.DefaultCertificate.Thumbprint,
                Subject = config.DefaultCertificate.Subject
            };

            if(!string.IsNullOrWhiteSpace(this.PrivateKey))
                certValues.Path = FileOperations.ResolvePath(this.PrivateKey);

            var src = FileOperations.ResolvePath(this.Source ?? this.SourceOrdinal);

            RSA privateKey = null;

            if(string.IsNullOrWhiteSpace(src))
            {
                Console.Error.WriteLine("A file or directory path is required for the encrypt command. ");
                Console.Error.WriteLine("Try running: kryptos file decrypt -s \"c:/path/to/file\" ");
                return false;
            }

            FileAttributes attr = File.GetAttributes(src);
            var isDirectory = (attr & FileAttributes.Directory) == FileAttributes.Directory;

            if(isDirectory && !this.Force)
            {
               
                var confirmed = false;
                bool stop = false;
                while(!confirmed)
                {
                    Console.Write("decrypt all files in folder? y or n:");
                    int k = -1;
                   
                    while((k = Console.Read()) > -1)
                    {
                        char c = (char)k;
                        var answer = c.ToString().ToLower();
                        if(answer == "y")
                        {
                            confirmed = true;
                            break;
                        }
                        if(answer == "n")
                        {
                            confirmed = true;
                            stop = true;
                            break;
                        }
                    }
                }

                if(stop)
                {
                    return true;
                }
            }

            

            if(!string.IsNullOrWhiteSpace(certValues.Path))
            {
                if(certValues.Password == null || certValues.Password.Length == 0)
                {
                    Console.Error.WriteLine("Invalid configuration: Certificate path and encrypted password must be set.");
                    Console.Error.WriteLine("run kryptos config cert -s \"c:/path/to/cert\" -p");
                    return false;
                }



                if(certValues.Path.EndsWith(".pem") ||
                    string.IsNullOrWhiteSpace(Path.GetExtension(certValues.Path)))
                {
                    var bytes = File.ReadAllBytes(certValues.Path);
                    var text = System.Text.Encoding.ASCII.GetString(bytes);
                    privateKey = CertificateOperations.ReadPrivateKeyFromPem(text, certValues.Password);
                }

                if(certValues.Path.EndsWith(".pfx"))
                {
                    privateKey = CertificateOperations
                        .ReadCertificate(certValues.Path, certValues.Password)
                        .GetRSAPrivateKey();
                }

                if(privateKey == null)
                {
                    throw new NotSupportedException(Path.GetExtension(certValues.Path) + " is not currrently supported");
                }
            }

            if(!string.IsNullOrWhiteSpace(certValues.Thumbprint))
            {
                privateKey = CertificateOperations
                    .FindCertificateByThumbprint(certValues.Thumbprint)
                    .GetRSAPrivateKey();
            }

            if(!string.IsNullOrWhiteSpace(certValues.Subject))
            {
                privateKey = CertificateOperations
                    .FindCertificateBySubject(certValues.Subject)
                    .GetRSAPrivateKey();
            }

            if(privateKey == null)
            {
                Console.Error.WriteLine("Could not load X509 Certificate Private. Verify configuration and parametesr");
                return false;
            }

            var dest = this.Destination;
            if(string.IsNullOrWhiteSpace(dest))
            {
                dest= src;
                if(!isDirectory)
                {
                    if(dest.EndsWith(".krypt"))
                        dest = dest.Substring(0, dest.Length - 6);
                }
            }

            if(!isDirectory)
            {
                FileOperations.DecryptFile(src, dest, this.Force, privateKey);
                return true;
            }

            var files = Directory.GetFiles(src);
            foreach(var file in files)
            {
                if(!file.EndsWith(".krypt"))
                    continue;

                var d = Path.Combine(dest, Path.GetFileName(file));
                if(d.EndsWith(".krypt"))
                    d = d.Substring(0, d.Length - 6);

                FileOperations.DecryptFile(file, d, this.Force, privateKey);
            }

            return true;
        }
    }
}