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
using Microsoft.EntityFrameworkCore;

namespace Nexus.Services
{
    public class UserService 
    {
        private NexusDbContext db;

        private IPasswordAuthenticator authenticator;

        private ResourceService resourceService;

        public UserService(
            NexusDbContext dbContext, 
            IPasswordAuthenticator authenticator) {
            
            this.db = dbContext;
            this.resourceService = new ResourceService(dbContext);
            this.authenticator = authenticator;
        }

        public async Task<User> FindOne(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loweredName = name.ToLower();
            var record = await this.db.Users
                .SingleOrDefaultAsync(o => loweredName == o.Name);

            return Map(record);
        }

        public async Task<User> FindOne(
            long resourceId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var record = await this.db.Users
                .SingleOrDefaultAsync(o => resourceId == o.ResourceId);

            return Map(record);
        }

        public async Task<User> FindOne(
            int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var record = await this.db.Users
                .SingleOrDefaultAsync(o => id == o.Id);

            return Map(record);
        }

        public async Task<User> SaveAsync(
            User user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            UserRecord record = null;
            int id = 0;
            bool added = false;
            if(user.Id.HasValue && user.Id.Value > 0) 
            {
                id = user.Id.Value;
                record = await this.db.Users.SingleOrDefaultAsync(o => o.Id == user.Id.Value);
            }   
        
            if(record == null)
            {
                record = new UserRecord();
                this.db.Users.Add(record);
                added = true;
            }
                
            Map(user, record);

            if(added)
            {
                var k = await this.resourceService.GetOrAddKindAsync<User>(cancellationToken)
                            .ConfigureAwait(false);

                record.Resource = new ResourceRecord() {
                    KindId = k.Id
                };

                this.db.Resources.Add(record.Resource);
            }
           
            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            if(added)
            {
                record.Resource.RowId = record.Id;
                await this.db.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            
            return Map(record);
        }

        public async Task<Tuple<User, bool>> VerifyApiKeyAsync(
           string name, 
           byte[] apiKey,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var loweredName = name.ToLowerInvariant();
            var record = await this.db.Users.Include(o => o.ApiKeys)
                .SingleOrDefaultAsync(o => loweredName == o.Name);
                
            if(record == null)
                return new Tuple<User, bool>(null, false);

            var utcNow = DateTime.UtcNow;
            foreach(var userApiKey in record.ApiKeys)
            {
                if(userApiKey.ExpiresAt.HasValue && userApiKey.ExpiresAt.Value <= utcNow)
                    continue;

                if(this.authenticator.Verify(apiKey, Convert.FromBase64String(userApiKey.ApiKey)))
                {
                    
                    return new Tuple<User, bool>(
                        Map(record), true);
                }
            }

            return new Tuple<User, bool>(null, false);
        }


       public async Task<Tuple<User, bool>> VerifyAsync(
           string name, 
           byte[] password,
           CancellationToken cancellationToken = default(CancellationToken))
        {
            var loweredName = name.ToLowerInvariant();
            var record = await this.db.Users
                .SingleOrDefaultAsync(o => loweredName == o.Name);

            if(record == null)
                return new Tuple<User, bool>(null, false);

            if(this.authenticator.Verify(password, Convert.FromBase64String(record.Password)))
            {
                var user = new User() {
                    Id = record.Id,
                    Name = record.Name,
                    DisplayName = record.DisplayName,
                    IconUri = record.IconUri
                };
                return new Tuple<User, bool>(user, true);
            }

            return new Tuple<User, bool>(null, false);
        }

        public async Task<EnhancedUserRegistration> RegisterAsync(
            UserRegistration registration,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var loweredName = registration.Name.ToLower();

            var count = await this.db.Users
                .Where(o => loweredName == o.Name)
                .CountAsync();

            if(count > 0)
                throw new Exception($"user name is already taken {loweredName}");

            bool generateApiKey = registration.GenerateApiKey.HasValue 
                && registration.GenerateApiKey.Value;
            
            bool generatePassword = registration.GeneratePassword.HasValue 
                && registration.GeneratePassword.Value;

            bool isAdmin = registration.IsAdmin.HasValue 
                && registration.IsAdmin.Value;

            var result = new EnhancedUserRegistration();
            var user = new UserRecord() {
                Name = loweredName,
                DisplayName = registration.DisplayName,
                IsAdmin = isAdmin,
                IconUri = registration.IconUri,
            };
            
            await this.db.AddAsync(user);
           

            if(generateApiKey) {
                var apiKey = new UserApiKeyRecord();
                result.ApiKey = this.GenerateApiKey(apiKey);
                
                user.ApiKeys.Add(apiKey);
                await this.db.AddAsync(apiKey);
            }

            var kind = await this.resourceService
                .GetOrAddKindAsync<User>(cancellationToken)
                .ConfigureAwait(false);

            user.Resource = new ResourceRecord() {
                KindId = kind.Id,
            };

            await this.db.AddAsync(user);


            if(generatePassword)
                result.Password = this.GenerateTempPassword(user);

            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            user.Resource.RowId = user.Id;
            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return result;
        }
 
        private SecureString GenerateTempPassword(UserRecord user)
        {
            int length = 12;
            if(user.IsAdmin)
                length = 30;

            var pw = PasswordGenerator.Generate(length);
            var ss = new SecureString();
            foreach(var c in pw)
                ss.AppendChar(c);

            var bytes = System.Text.Encoding.UTF8.GetBytes(pw);
            var hash = this.authenticator.ComputeHash(bytes);
            user.Password = Convert.ToBase64String(hash);

            Array.Clear(pw, 0, pw.Length);
            Array.Clear(bytes, 0, bytes.Length);
            Array.Clear(hash, 0, hash.Length);

            return ss;
        }

        private SecureString GenerateApiKey(UserApiKeyRecord apiKey)
        {
            int length = 30;
           
            var pw = PasswordGenerator.Generate(length);
            var ss = new SecureString();
            foreach(var c in pw)
                ss.AppendChar(c);

            var bytes = System.Text.Encoding.UTF8.GetBytes(pw);
            var hash = this.authenticator.ComputeHash(bytes);
            apiKey.ApiKey = Convert.ToBase64String(hash);

            Array.Clear(pw, 0, pw.Length);
            Array.Clear(bytes, 0, bytes.Length);
            Array.Clear(hash, 0, hash.Length);

            return ss;
        }

        private static User Map(UserRecord record)
        {
            return new User() {
                Id = record.Id,
                Name = record.Name,
                DisplayName = record.DisplayName,
                IconUri = record.IconUri,
                ResourceId = record.ResourceId
            };
        }

        private static void Map(User user, UserRecord record)
        {
            if(user.Id.HasValue && user.Id.Value > 0) 
                record.Id = user.Id.Value;

            record.DisplayName = user.DisplayName;
            record.Name = user.Name;
            record.IconUri = user.IconUri;
        }

        public class EnhancedUserRegistration :  UserRegistration
        {
            public SecureString ApiKey { get; set; }

            public SecureString Password { get; set; }

        } 
    }
}