using System;
using System.Diagnostics;
using System.Runtime.InteropServices;




namespace NerdyMishka.KeePass.Cryptography
{
#if NETCOREAPP
    // BORROWED FROM MONO

    using global::System.Security.Cryptography;


   


    public static partial class ProtectedData
    {
        public static byte[] Protect(byte[] userData, byte[] optionalEntropy, DataProtectionScope scope)
        {
            if (userData == null)
                throw new ArgumentNullException(nameof(userData));

            return ManagedProtection.Protect(userData, optionalEntropy, scope);
        }

        public static byte[] Unprotect(byte[] encryptedData, byte[] optionalEntropy, DataProtectionScope scope)
        {
            if (encryptedData == null)
                throw new ArgumentNullException(nameof(encryptedData));

            return ManagedProtection.Unprotect(encryptedData, optionalEntropy, scope);
        }
    }
#endif
}
