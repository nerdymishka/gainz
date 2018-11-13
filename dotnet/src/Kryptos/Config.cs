using NerdyMishka.Flex;
using NerdyMishka.Flex.Yaml;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using NerdyMishka.Security.Cryptography;
using System.IO;
using NerdyMishka.Windows;
using System.Security.Cryptography;

namespace Kryptos
{

    public static class Settings
    {
        public static byte[] DefaultPhrase { get; set; }

        public static string DefaultFileName { get; set; }

        public static string HomeDir { get; set; }

        public static byte[] Key { get; set; }

        static Settings()
        {
             var chars = new char[] { 'n', '#', '%', '4','O', 'x', '9', ' ', ' ', '3', '5', 'g' };
            DefaultPhrase = System.Text.Encoding.UTF8.GetBytes(chars);
            Array.Clear(chars, 0, chars.Length);

            var home = System.Environment.GetEnvironmentVariable("HOME");
            if(string.IsNullOrWhiteSpace(home)) {
                if(System.Environment.OSVersion.Platform == System.PlatformID.Win32NT)
                {
                    home = System.Environment.GetEnvironmentVariable("USERPROFILE");
                    if(string.IsNullOrWhiteSpace(home)) {
                        home = System.Environment.GetEnvironmentVariable("SystemDrive");
                        var username = System.Environment.GetEnvironmentVariable("USERNAME");
                        home = $"{home}/Users/{username}";
                    }
                } else {
                    var username = System.Environment.GetEnvironmentVariable("USERNAME");
                    home = $"/home/{username}";
                }
            }

            HomeDir = home.Replace("\\", "/");

            DefaultFileName = $"{HomeDir}/.config/kryptos/key.txt";
        }

        public static void EnsureKey()
        {
            if(Key != null && Key.Length > 0)
                return;

            var envKey = System.Environment.GetEnvironmentVariable("KRYPTOS_KEY");
            var isWindows = System.Environment.OSVersion.Platform == System.PlatformID.Win32NT;
            if(!string.IsNullOrWhiteSpace(envKey))
            {
                Key = System.Text.Encoding.UTF8.GetBytes(envKey);
                return;
            }

            var file = DefaultFileName;

            if(File.Exists(file))
            {
                var content = File.ReadAllBytes(file);

                if(isWindows) {
                   //content = ProtectedData.Unprotect(content, DefaultPhrase, DataProtectionScope.CurrentUser);
                }

                Key = content;
                return;
            }

            if(isWindows)
            {
                var credentials = VaultManager.List();
                foreach(var cred in credentials)
                {
                    if(cred.Key == "kryptos")
                    {
                        Key = cred.GetBlob();
                        return;
                    }
                }
            }

            var rootDir = Directory.GetParent(DefaultFileName).FullName;

            if(!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }

            var key = PasswordGenerator.GenerateAsBytes(40);
            Key = key;

            if(isWindows)
            {
                //var bytes = ProtectedData.Protect(key, DefaultPhrase, DataProtectionScope.CurrentUser);
                //File.WriteAllBytes(DefaultFileName, bytes);
                File.WriteAllBytes(DefaultFileName, key);
                return;
            }

            File.WriteAllBytes(DefaultFileName, key);
        }
    }

    public class Config
    {
     

      

        public Config()
        {
            this.DefaultCertificate = new CertificateSection();
            this.Layout = new LayoutSection();
            this.Logging = new LoggingSection();
        }

       
    

        public static Config Load(string path = null)
        {
            if(path == null)
            {
                var userPath = $"{Settings.HomeDir}/.config/kryptos/kryptos.yml";
                if(!File.Exists(userPath))
                    path = Env.ResolveAppPath("./kryptos.yml");
                else 
                    path = userPath;
            }

            var builder = new FlexBuilder()
                .SetCryptoProvider(new FlexCryptoProvider());

            return builder.FromYamlFile<Config>(path);
        }

        public static bool Save(Config config, string path  = null)
        {
            if(path == null)
            {
                path = $"{Settings.HomeDir}/.config/kryptos/kryptos.yml";
                var rootDir = Path.GetDirectoryName(path);
                if(!Directory.Exists(rootDir))
                    Directory.CreateDirectory(rootDir); 
            }
           
            var builder = new FlexBuilder()
                .SetCryptoProvider(new FlexCryptoProvider());
            
            if(File.Exists(path))
                File.Delete(path);

            builder.ToYamlFile<Config>(path, config);
            return true;
        }

        [Symbol("certificate")]
        public CertificateSection DefaultCertificate {  get; set; }


        [Symbol("layout")]
        public LayoutSection Layout {get; set; }

        [Symbol("logging")]
        public LoggingSection Logging { get; set; }
        

        public class CertificateSection
        {
            [Symbol("name")]
            public string Name { get; set; }

            [Symbol("password")]
            [Encrypt]
            public byte[] Password { get; set; }

            [Symbol("path")]
            public string Path { get; set; }

            [Symbol("subject")]
            public string Subject { get; set; }

            [Symbol("thumbprint")]        
            public string Thumbprint { get; set; }
        }

       
        public class LayoutSection
        {   
            
            [Symbol("source")]
            public string SourceDirectory {get; set; }

            [Symbol("encrypt")]
            public string EncryptDirectory { get; set; }

            [Symbol("decrypt")]
            public string DecryptDirectory { get; set; }
        }


        public class LoggingSection
        {   
            [Symbol("enabled")]
            public bool Enabled { get; set; }

            [Symbol("dir")]
            public string Directory  { get; set; }

            [Symbol("appInsightsKey")]
            [Encrypt]
            public string AppInsightsKey { get; set; }
        }


        public class FlexCryptoProvider : IFlexCryptoProvider
        {
        

            public byte[] DecryptBlob(byte[] blob, byte[] privateKey = null)
            {
                Settings.EnsureKey();
                return DataProtection.DecryptBlob(blob, Settings.Key);
            }

            public string DecryptString(string value, byte[] privateKey = null)
            {
                Settings.EnsureKey();
                return DataProtection.DecryptString(value, Settings.Key);
            }

            public byte[] EncryptBlob(byte[] blob, byte[] privateKey = null)
            {
                Settings.EnsureKey();
                return DataProtection.EncryptBlob(blob, Settings.Key);
            }

            public string EncryptString(string value, byte[] privateKey = null)
            {
                Settings.EnsureKey();
                return DataProtection.EncryptString(value, Settings.Key);
            }
        }
    }
}