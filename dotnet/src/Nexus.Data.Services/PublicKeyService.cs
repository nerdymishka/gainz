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
using System.Text;

namespace Nexus.Services
{
    public class PublicKeyService
    {
        private NexusDbContext db;

        public PublicKeyService(NexusDbContext db)
        {
            this.db = db;
        }
        
        public async Task<PublicKey> FindOneAsync(
            int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var record = await this.db.PublicKeys
                .Include(o => o.User)
                .SingleOrDefaultAsync(o => id == o.Id, cancellationToken)
                .ConfigureAwait(false);

            return Map(record);
        }

         public async Task<PublicKey> FindOneAsync(
            string uriPath, 
            int? userId,
            CancellationToken cancellationToken = default(CancellationToken))
        {         
            var record = await this.db.PublicKeys
                .Include(o => o.User)
                .Where(o => o.UriPath == uriPath && o.UserId == userId)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return Map(record);
        }

         public async Task<PublicKey[]> FindAllByUserAsync(
            string username,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = username.ToLower();
            var records = await this.db.PublicKeys
                .Include(o => o.User)
                .Where(o => o.User.Name == lowered)
                .SelectAsync(o => Map(o), cancellationToken)
                .ConfigureAwait(false);

            return records.ToArray();
        }

        public async Task<PublicKey> SaveAsync(
            PublicKey publicKey,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            PublicKeyRecord record = null;
            var id = 0;
            
            if(publicKey.Id.HasValue && publicKey.Id.Value > 0)
            {
                id = publicKey.Id.Value;
                record = await this.db.PublicKeys
                    .Include(pk => pk.User)
                    .SingleOrDefaultAsync(o => o.Id == id)
                    .ConfigureAwait(false);
            }

            if(record == null)
            {
                record = new PublicKeyRecord() {
                    Id = id,
                };
            
                await this.db.PublicKeys.AddAsync(record);
            }

            UserRecord user = null;
            if(!(string.IsNullOrWhiteSpace(publicKey.Username) 
                && (!publicKey.UserId.HasValue || publicKey.UserId < 1))) {
                
                user = await this.db
                    .FindUserAsync(publicKey.Username, cancellationToken);

                if(user != null)
                {
                    record.User = user;
                    record.UserId = user.Id;
                    publicKey.UserId = user.Id;
                }
            }

            if(publicKey.UserId != record.UserId)
                record.UserId = publicKey.UserId;

            record.UriPath = publicKey.UriPath;
            record.Blob = System.Text.Encoding.UTF8.GetBytes(publicKey.Blob);
            
            await this.db
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return Map(record);
        }


        private static PublicKey Map(PublicKeyRecord record)
        {
            var user = record.User;
            string name = record?.User?.Name;

            return new PublicKey() {
                Id = record.Id,
                UriPath = record.UriPath,
                Blob = System.Text.Encoding.UTF8.GetString(record.Blob),
                UserId = record.UserId,
                Username = name 
            };
        }
    }
}