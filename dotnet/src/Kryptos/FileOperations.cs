using System.Security.Cryptography.X509Certificates;
using NerdyMishka.Security.Cryptography;
using System.IO;
using System.Security.Cryptography;

namespace Kryptos
{
    public class FileOperations
    {

        public static void EncryptFile(Stream reader, Stream writer, X509Certificate2 certificate)
        {
            DataProtection.EncryptStream(reader, writer, certificate.GetRSAPublicKey());
        }

        public static void EncryptFile(Stream reader, Stream writer, RSA publicKey)
        {
            DataProtection.EncryptStream(reader, writer, publicKey);
        }

        public static void EncryptFile(string source, string destination, bool force, RSA publicKey)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if(!File.Exists(source))
                throw new FileNotFoundException("source file does not exist", source);

            var rootDir = Directory.GetParent(destination).FullName;
            if(!Directory.Exists(rootDir))
            {
                if(force)
                    Directory.CreateDirectory(rootDir);
                else 
                    throw new DirectoryNotFoundException(
                        $"directory does not exist, to force the creation of the directory use the --force flag. Directory: {rootDir}");
            }

            if(File.Exists(destination) && !force)
            {
                throw new System.InvalidOperationException(
                    "the destination file already exists. Delete the file, use a new name, or use the --force flag to overwrite the file: " +
                    destination
                );
            }

            using(var rs = File.OpenRead(source))
            using(var ws = File.OpenWrite(destination))
            {
                EncryptFile(rs, ws, publicKey);
            }

            if(!File.Exists(destination))
                throw new FileNotFoundException("Encrypt method failed, the destination file does not exit", destination);
        }


        public static void EncryptFile(string source, string destination, bool force, X509Certificate2 certificate)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if(!File.Exists(source))
                throw new FileNotFoundException("source file does not exist", source);

            var rootDir = Directory.GetParent(destination).FullName;
            if(!Directory.Exists(rootDir))
            {
                if(force)
                    Directory.CreateDirectory(rootDir);
                else 
                    throw new DirectoryNotFoundException(
                        $"directory does not exist, to force the creation of the directory use the --force flag. Directory: {rootDir}");
            }

            if(File.Exists(destination) && !force)
            {
                throw new System.InvalidOperationException(
                    "the destination file already exists. Delete the file, use a new name, or use the --force flag to overwrite the file: " +
                    destination
                );
            }

            using(var rs = File.OpenRead(source))
            using(var ws = File.OpenWrite(destination))
            {
                EncryptFile(rs, ws, certificate);
            }

            if(!File.Exists(destination))
                throw new FileNotFoundException("Encrypt method failed, the destination file does not exit", destination);
        }

        public static void DecryptFile(Stream reader, Stream writer, X509Certificate2 certificate)
        {
            DataProtection.DecryptStream(reader, writer, certificate.GetRSAPrivateKey());
        }

        public static void DecryptFile(string source, string destination, bool force, X509Certificate2 certificate)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if(!File.Exists(source))
                throw new FileNotFoundException("source file does not exist", source);

            var rootDir = Directory.GetParent(destination).FullName;
            if(!Directory.Exists(rootDir))
            {
                if(force)
                    Directory.CreateDirectory(rootDir);
                else 
                    throw new DirectoryNotFoundException(
                        $"directory does not exist, to force the creation of the directory use the --force flag. Directory: {rootDir}");
            }

            if(File.Exists(destination) && !force)
            {
                throw new System.InvalidOperationException(
                    "the destination file already exists. Delete the file, use a new name, or use the --force flag to overwrite the file: " +
                    destination
                );
            }

            using(var rs = File.OpenRead(source))
            using(var ws = File.OpenWrite(destination))
            {
                DecryptFile(rs, ws, certificate);
            }

            if(!File.Exists(destination))
                throw new FileNotFoundException("Encrypt method failed, the destination file does not exit", destination);
        }

         public static void DecryptFile(Stream reader, Stream writer, RSA privateKey)
        {
            DataProtection.DecryptStream(reader, writer, privateKey);
        }

        public static void DecryptFile(string source, string destination, bool force, RSA privateKey)
        {
            source = ResolvePath(source);
            destination = ResolvePath(destination);

            if(!File.Exists(source))
                throw new FileNotFoundException("source file does not exist", source);

            var rootDir = Directory.GetParent(destination).FullName;
            if(!Directory.Exists(rootDir))
            {
                if(force)
                    Directory.CreateDirectory(rootDir);
                else 
                    throw new DirectoryNotFoundException(
                        $"directory does not exist, to force the creation of the directory use the --force flag. Directory: {rootDir}");
            }

            if(File.Exists(destination) && !force)
            {
                throw new System.InvalidOperationException(
                    "the destination file already exists. Delete the file, use a new name, or use the --force flag to overwrite the file: " +
                    destination
                );
            }

            using(var rs = File.OpenRead(source))
            using(var ws = File.OpenWrite(destination))
            {
                DecryptFile(rs, ws, privateKey);
            }

            if(!File.Exists(destination))
                throw new FileNotFoundException("Encrypt method failed, the destination file does not exit", destination);
        }

        public static string ResolvePath(string path)
        {
            if(string.IsNullOrWhiteSpace(path))
                return path;


            if((path[0] == '.' || path[0] == '~') && path[1] == '/' || path[0] == '\\')
            {
                return System.Environment.CurrentDirectory.Replace("\\", "/") + path.Substring(1);
            }

            return path;
        }
    }
}