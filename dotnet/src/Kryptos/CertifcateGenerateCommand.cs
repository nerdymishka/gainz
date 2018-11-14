using System;
using System.Collections.Generic;
using System.IO;
using CommandLine;
using NerdyMishka.Security.Cryptography;

namespace Kryptos
{
    public class CertifcateGenerateCommand : Command
    {
        public override string Section => "cert";

        public override string Action => "new";

        [Option('p', "password", HelpText= "Optional. Set the password instead generating one.")]
        public bool Password { get; set; }

        [Option('k', "key", HelpText = "Optional. Creates a default certificate with a different name than kryptos_rsa")]
        public string Path { get; set; }

        public override bool Run(Config config)
        {

            var pem = $"{Settings.HomeDir}/.ssh/kryptos_rsa";

            if(!string.IsNullOrWhiteSpace(Path))
            {
                pem = FileOperations.ResolvePath(pem);
            }

            if(File.Exists(pem))
            {
                var current = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"kryptos_rsa already exists: {pem}");
                Console.ForegroundColor = current;

                return false;
            }

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
                    for(var z = 0; z < pw.Count; z++)
                    {
                        if(pw[z] != pwc[z])
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
            } else {
                var pw = PasswordGenerator.Generate(12);
                var bytes = System.Text.Encoding.UTF8.GetBytes(pw);
                config.DefaultCertificate.Password = bytes;

                var current = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("");
                Console.WriteLine("Save the phrase below to a password vault:");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(pw);
                Console.WriteLine("");

                Console.ForegroundColor = current;
            }

            // ick;
            var pws = System.Text.Encoding.UTF8.GetString(config.DefaultCertificate.Password);
         
            var fullPath = Env.GetFullPath("ssh-keygen.exe");
            if(string.IsNullOrEmpty(fullPath))
            {
                throw new FileNotFoundException(
                    "To convert a .pub file to a .pem file, kryptos requires ssh-keygen.exe to exist on the path",
                    "ssh-keygen.exe");
            }

         
            var p = new System.Diagnostics.Process();
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = "ssh-keygen.exe";
            p.StartInfo.Arguments = $"-t rsa -b 2048 -f \"{pem}\" -N \"{pws}\" ";

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            
            p.WaitForExit();
            Console.WriteLine(p.StandardOutput.ReadToEnd());
            Console.WriteLine(p.StandardError.ReadToEnd());

            config.DefaultCertificate.Path = pem;
            

            var exists = File.Exists(pem);
            if(exists)
            {
<<<<<<< HEAD
                var pw = PasswordGenerator.Generate(12);
                var bytes = System.Text.Encoding.UTF8.GetBytes(pw);
                config.DefaultCertificate.Password = bytes;

=======
>>>>>>> a161b37e30864c70f2acd784b86fe6c96cc82bb6
                var current = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("");
                Console.WriteLine("Save the files below to a password vault:");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(pem);
                Console.WriteLine($"{pem}.pub");

                Console.ForegroundColor = current;
                 Config.Save(config);
            }
               

            return exists;
        }
    }
}