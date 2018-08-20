using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace NerdyMishka.KeePass
{
    public static class MasterKeyExtensions
    {
        public static MasterKey AddPassword(this MasterKey key, string password)
        {
            key.Add(new MasterKeyPassword(password));
            return key;
        }

        public static MasterKey AddPassword(this MasterKey key, byte[] password)
        {
            key.Add(new MasterKeyPassword(password));
            return key;
        }

        public static MasterKey AddPassword(this MasterKey key, SecureString password)
        {
            key.Add(new MasterKeyPassword(password));
            return key;
        }

        public static MasterKey AddKeyFile(this MasterKey key, string path)
        {
            key.Add(new MasterKeyFile(path));
            return key;
        }

        public static MasterKey AddUserAccount(this MasterKey key, string keyLocation = null)
        {
            key.Add(new MasterKeyUserAccount(keyLocation));
            return key;
        }
    }
}
