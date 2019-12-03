

using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;

namespace NerdyMishka.KeePass
{
    public interface IKeePassPackageSerializer 
    {
         string Extension { get; }

         IDictionary<Type, Type> Mappings { get; }
         
         IDictionary<string, ProtectedMemoryBinary> Binaries { get; set; }

         void Read(IKeePassPackage package,  Stream stream);

         void Write(IKeePassPackage package, Stream stream);
    }
}