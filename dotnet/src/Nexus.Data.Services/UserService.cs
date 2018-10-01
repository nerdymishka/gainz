using Microsoft.Extensions.Logging;
using Nexus.Api;
using Nexus.Data;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

using NerdyMishka.Security.Cryptography;

namespace Nexus.Services
{
    public class UserService 
    {
        private NexusDbContext db;

        private IPasswordAuthenticator authenticator;

        public UserService(NexusDbContext dbContext, IPasswordAuthenticator authenticator) {

        }

        public Task<UserRegistration> RegisterAsync(UserRegistration registration)
        {
            bool generateApiKey = registration.GenerateApiKey.HasValue 
                && registration.GenerateApiKey.Value;
            
            bool generatePassword = registration.GeneratePassword.HasValue 
                && registration.GeneratePassword.Value;

            if(generateApiKey)
            {

            }

            if(generatePassword)
            {

            }

            return null;
        }
 


        public class EnhancedUserRegistration
        {
            
            public int? Id { get; set; }

            public string Name { get; set; }

            public string DisplayName { get; set; }

            public bool? IsAdmin { get; set; }

            public string IconUri { get; set; }

            public bool? GeneratePassword { get; set; }

            public bool? GenerateApiKey { get; set; }

            public SecureString ApiKey { get; set; }

            public SecureString Password { get; set; }

        } 
    }
}