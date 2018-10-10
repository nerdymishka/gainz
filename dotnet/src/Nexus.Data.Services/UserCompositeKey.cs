using NerdyMishka.Security.Cryptography;
using System;


namespace Nexus.Services {

    public class UserCompositeKey : CompositeKey  
    {
        private static readonly byte[] salt = System.Text.Encoding.UTF8.GetBytes("4 score and 2 days ago I #lulz#");


        public UserCompositeKey(byte[] key, byte[] salt = null) : base() {
            if(salt == null)
                salt = UserCompositeKey.salt;
            
            var derivitive = PasswordAuthenticator.Pbkdf2(key, salt, 10010);

            // this will store only the hash in memory
            // using salsa20.
            this.AddPassword(derivitive);
           
            Array.Clear(derivitive, 0, derivitive.Length);
        }
    }
}