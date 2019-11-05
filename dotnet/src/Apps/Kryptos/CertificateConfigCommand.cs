using System;
using System.Collections.Generic;
using CommandLine;

namespace Kryptos
{

    public interface ICommand
    {
        string Section { get; }

        string Action {get; }

        bool Run(Config config);
    }

    public class Command : ICommand
    {
        public virtual string Section => null;

        public virtual string Action => null;

        public virtual bool Run(Config config) => false;
    }

    public class CertificateConfigCommand : Command 
    {
        
        public override string Section => "cert";

        public override string Action  => "config";

        [Option('t', "thumbprint", HelpText = "Optional. The thumprint value used to retrieve default certificate used for decryption.")]
        public string Thumbprint { get; set; }

        [Option('n', "subject", HelpText = "Optional. The subject value used to retrieve the default certificate use for decryption.")]
        public string Subject { get; set; }

        [Option('k', "key", HelpText = "Optional. The path to the default private key certificate (.pem or .pfx)")]
        public string Path { get; set; }

        [Option('p', "password", HelpText = "Optional. Required if -key is used. The password for the certificate")]
        public bool Password { get; set; }

        public override bool Run(Config config)
        {
            bool save = false;
            if(Password)
            {
                var confirmed = false;
                var pw = new List<char>();
                var pwc = new List<char>();
                var attempts = 0;
                while(!confirmed)
                {
                    Console.Write("enter password:");
                    
                    do
                    {
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        

                        if (info.Key != ConsoleKey.Backspace && info.Key != ConsoleKey.Enter)
                        {
                            pw.Add(info.KeyChar);
                            Console.Write("*");
                        } else
                        {
                            if (info.Key == ConsoleKey.Backspace && pw.Count > 0)
                            {
                                pw.RemoveAt(pw.Count - 1);
                                Console.Write("\b \b");
                            }
                            else if(info.Key == ConsoleKey.Enter)
                            {
                                Console.WriteLine("");
                                break;
                            }
                        }
                       
                    } while(true);
                    
                    Console.Write("confirm password:");
                    do
                    {
                        ConsoleKeyInfo info = Console.ReadKey(true);
                        

                        if (info.Key != ConsoleKey.Backspace && info.Key != ConsoleKey.Enter)
                        {
                            pwc.Add(info.KeyChar);
                            Console.Write("*");
                        } else
                        {
                            if (info.Key == ConsoleKey.Backspace && pw.Count > 0)
                            {
                                pwc.RemoveAt(pwc.Count - 1);
                                Console.Write("\b \b");
                            }
                            else if(info.Key == ConsoleKey.Enter)
                            {
                                Console.WriteLine("");
                                break;
                            }
                        }
                       
                    } while(true);
                    

                    if(pw.Count != pwc.Count)
                    {
                        var current = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("passwords do not match.");
                        if(attempts > 3)
                        {
                            Console.WriteLine("exit.");
                            Console.ForegroundColor = current;
                            return false;
                        }
                        Console.ForegroundColor = current;
                        attempts++;
                        continue;
                    }

                    var match = true;
                    for(var p =0; p < pw.Count; p++)
                    {
                        if(pw[p] != pwc[p])
                        {
                            match = false;
                            break;
                        }
                    }

                    if(!match)
                    {
                        var current = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("passwords do not match.");
                        if(attempts > 3)
                        {
                            Console.WriteLine("exit.");
                            Console.ForegroundColor = current;
                            return false;
                        }

                        
                        Console.ForegroundColor = current;
                        attempts++;
                        continue;
                    }

                    confirmed = true;
                }

                
                config.DefaultCertificate.Password = System.Text.Encoding.UTF8.GetBytes(pw.ToArray());
                save = true;
            }

            if(config.DefaultCertificate.Path != this.Path)
            {   
                config.DefaultCertificate.Path = this.Path;
                save = true;
            }

            if(config.DefaultCertificate.Thumbprint != this.Thumbprint)
            {
                config.DefaultCertificate.Thumbprint = this.Thumbprint;
                save = true;
            }

            if(config.DefaultCertificate.Subject != this.Subject)
            {
                config.DefaultCertificate.Subject = this.Subject;
                save = true;
            }

            if(save)
            {
                Config.Save(config);
            }
            return true;
        }
    }
}