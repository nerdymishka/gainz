using System;
using System.Collections.Generic;
using System.Reflection;

[assembly:AssemblyVersionAttribute("1.1.0")]

namespace Kryptos
{
    class Program
    {
        static int Main(string[] args)
        {
            
            var commands = new List<Command>() {
                new CertificateConfigCommand(),
                new FileEncryptCommand(),
                new FileDecryptCommand(),
                new CertifcateGenerateCommand(),
            };

            if(args.Length == 0)
            {
                Console.WriteLine("kryptos version: 0.1.0");
                return 0;
            }

            var section = args[0].ToLowerInvariant();
            var help = new List<string>() {"-h", "--help", "/h", "/?", "-?", "help" };

            if(help.Contains(section) || args.Length < 2)
            {
                Console.WriteLine(@"
kryptos
version: 0.1.0

Kryptos is an encryption utilitiy for encrypting and decryptions messages and files
using public and private keys.

Currently only RSA certificates are supported. Kryptos will accept .pfx and .pems
and can access the current user's certificate store to retrieve certificates by
thumprint or subject name.

commands: 
  cert         
    - config      configures certificate values. 
                  $ kryptos cert config --help
    - new         generates a new cert using ssh-keygen
                  $ kryptos cert new --help

  file 
    - encrypt     encrypts a file. $ kryptos file encrypt --help
    - decrypt     decrypts a file. $ kryptos file decrypt --help
            ");
                return 0;
            }

            Settings.EnsureKey();
            
            
            var action = args[1].ToLowerInvariant();

            foreach(var command in commands)
            {
                if(command.Section == section && action == command.Action)
                {
                    switch(command)
                    {
                        case CertificateConfigCommand certConfigCommand:
                            CommandLine.Parser.Default.ParseArguments(() => certConfigCommand, args);
                        break;
                        case FileEncryptCommand fileEncryptCommand:
                            CommandLine.Parser.Default.ParseArguments(() => fileEncryptCommand, args);
                            if(args.Length > 2 && !args[2].StartsWith("-"))
                                fileEncryptCommand.SourceOrdinal= args[2];
                        break;

                        case FileDecryptCommand fileDecryptCommand:
                            CommandLine.Parser.Default.ParseArguments(() => fileDecryptCommand, args);
                            if(args.Length > 2 && !args[2].StartsWith("-"))
                                fileDecryptCommand.SourceOrdinal= args[2];
                        break;

                        case CertifcateGenerateCommand certifcateGenerateCommand:
                            CommandLine.Parser.Default.ParseArguments(() => certifcateGenerateCommand, args);
                        break;
                    }
                   
                
                  
                    var config = Config.Load();
                    bool result = false;
                    try {
                        if(args.Length >= 3)
                        {
                            var third = args[2];
                            if(third == "help" || third == "-h" || third == "--help")
                                return 0;
                        }

                         result = command.Run(config);
                    } catch(Exception ex) {
                        var current = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Error.WriteLine(ex.Message);
                        Console.Error.WriteLine(ex.StackTrace);
                        Console.ForegroundColor = current;
                        return 1;
                    }
                   
                    if(result)
                        return 0;
                    else 
                        return 1;
                }
            }

            Console.Error.WriteLine($"Unknown section {section} and action {action}. ");
            Console.Error.WriteLine("Run 'kryptos --help' to see a list of potential options");

            return 1;
        }
    }
}
