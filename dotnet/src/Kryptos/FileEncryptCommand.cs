using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CommandLine;

namespace Kryptos
{
    public class FileEncryptCommand : Command
    {
        public override string Section => "file";
        
        public override string Action => "encrypt";
        
        
        [Option('s', "src", HelpText = "Required. The path to the file that should be encrypted.")]
        [Value(0)]
        public string Source  { get; set; }

        public string SourceOrdinal { get; set;}

        [Option('o', "dest", 
        HelpText = "Optional. The path to the encrypted file. The .krypt extension is appended if it doesn't exist,")]
        public string Destination { get; set; }

        [Option('n', "name",
            HelpText = "Optional. The subject/name for a certificate in the user's cert store to encrypt the message.")]
        public string Subject { get; set; }

        [Option('t', "thumbprint", 
            HelpText = "optional. The thumprint for a certificate in the user's cert store to encrypt the message.")
            ]
        public string Thumbprint { get; set; }

        [Option('f', "force", HelpText = "Otional. forces the command to run")]
        public bool Force { get; set; }

        [Option('k', "key", HelpText = "Optional. The path to the public key (.pem or .pfx)")]
        public string PublicKey { get; set; }

        [Option('a', "all")]
        public bool All { get; set; }

        public override bool Run(Config config)
        {
            var certValues = config.DefaultCertificate;
            RSA publicKey = null;

            var src = FileOperations.ResolvePath(this.Source ?? this.SourceOrdinal);
            
            

            if(string.IsNullOrWhiteSpace(src))
            {
                Console.Error.WriteLine("A file or directory path is required for the encrypt command. ");
                Console.Error.WriteLine("Try running kryptos file encrypt -s \"c:/path/to/file\" ");
                return false;
            }

            if(!File.Exists(src))
            {
                Console.Error.WriteLine($"Cannot find file source file: {src}");
                return false;
            }

            if(string.IsNullOrWhiteSpace(this.Thumbprint) &&
                string.IsNullOrWhiteSpace(this.PublicKey) &&
                string.IsNullOrWhiteSpace(this.Subject)) {
                
                if(!this.Force)
                {
                    var current = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("files can only be decrypted by the local private key.");
                    Console.WriteLine("if you're encrypting a file for someone else, you'll ");
                    Console.WriteLine("to encrypt a file using their public key.");
                    Console.WriteLine("");
                    Console.WriteLine("use the --key, --thumprint, or --name to use another key");
                    Console.WriteLine("tip: the --key parameter can be a uri such as https://gitlab/user.keys");
                    Console.ForegroundColor = current;
                }


                if(!string.IsNullOrWhiteSpace(certValues.Path))
                {
                    if(string.IsNullOrWhiteSpace(Path.GetExtension(certValues.Path))) {
                        var path = certValues.Path + ".pub";

                        publicKey = CertificateOperations.ImportPublicKey(path);
                    } else {
                        if(certValues.Path.EndsWith(".pfx"))
                        {
                            if(certValues.Password == null || certValues.Password.Length == 0)
                            {
                                Console.Error.WriteLine("Invalid configuration: Certificate path and encrypted password must be set.");
                                Console.Error.WriteLine("run kryptos config cert -s \"c:/path/to/cert\" -p");
                                return false;
                            }

                            publicKey = CertificateOperations
                                .ReadCertificate(certValues.Path, certValues.Password)
                                .GetRSAPublicKey();
                        }

                        if(certValues.Path.EndsWith(".pem"))
                        {
                            var pem = File.ReadAllText(certValues.Path);
                            publicKey = CertificateOperations.ReadPublicKeyFromPem(pem);
                        }
                    }

                  
                }

                if(!string.IsNullOrWhiteSpace(certValues.Thumbprint))
                {
                    publicKey = CertificateOperations
                        .FindCertificateByThumbprint(certValues.Thumbprint)
                        .GetRSAPublicKey();

                }

                if(!string.IsNullOrWhiteSpace(certValues.Subject))
                {
                    publicKey = CertificateOperations
                        .FindCertificateBySubject(certValues.Subject)
                        .GetRSAPrivateKey();
                }

            } else {

                if(!string.IsNullOrWhiteSpace(this.PublicKey))
                {
                    if(this.PublicKey.StartsWith("http://"))
                        throw new NotSupportedException("only http over TLS is supported");

                    if(this.PublicKey.StartsWith("https://"))
                        publicKey = CertificateOperations.ImportPublicKey(new Uri(this.PublicKey));
                    else 
                        publicKey = CertificateOperations.ImportPublicKey(this.PublicKey);
                }

                if(!string.IsNullOrWhiteSpace(this.Subject))
                {
                    publicKey = CertificateOperations
                        .FindCertificateBySubject(this.Subject)
                        .GetRSAPublicKey();
                }

                if(!string.IsNullOrWhiteSpace(this.Thumbprint))
                {
                    publicKey = CertificateOperations
                        .FindCertificateByThumbprint(this.Thumbprint)
                        .GetRSAPublicKey();
                }
            }



            
            FileAttributes attr = File.GetAttributes(src);
            var isDirectory = (attr & FileAttributes.Directory) == FileAttributes.Directory;

            if(isDirectory && !this.Force)
            {
               
                var confirmed = false;
                bool stop = false;
                while(!confirmed)
                {
                    Console.Write("encrypt all files in folder? y or n:");
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

           
            if(publicKey == null)
            {
                Console.Error.WriteLine("Could not load X509 Certificate public. Verify parameters and configuration");
                return false;
            }

            var dest = this.Destination;
            if(string.IsNullOrWhiteSpace(dest))
            {
                dest= src;
                if(!isDirectory)
                {
                    dest += ".krypt";
                }
            }

            if(!isDirectory)
            {
                FileOperations.EncryptFile(src, dest, this.Force, publicKey);
                return true;
            }

            var files = Directory.GetFiles(src);
            foreach(var file in files)
            {
                var d = Path.Combine(dest, Path.GetFileName(file) + ".krypt");
                FileOperations.EncryptFile(file, d, this.Force, publicKey);
            }

            return true;
        }
    }
}