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
    public class ProtectedBlobService
    {
        private NexusDbContext db;

    
        
        public async Task<ProtectedBlobVault> SaveAsync(
            ProtectedBlobVault vault,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ProtectedBlobVaultRecord record = null;
            int id = vault.Id.HasValue ? (int)vault.Id : 0;
            if(id > 0) {
                record = await this.db.Vaults
                    .SingleOrDefaultAsync(o => o.Id == id, cancellationToken)
                    .ConfigureAwait(false);
            }

            if(record == null)
            {
                record = new ProtectedBlobVaultRecord() {
                    Id = id
                };

                await this.db.Vaults.AddAsync(record);
                
                // assigned users/environments, etc shouldn't
                // change after its created. if it does need to 
                // change a different call should be required.
                UserRecord user = null;
                if(!string.IsNullOrWhiteSpace(vault.Username) &&
                    (!vault.UserId.HasValue || vault.UserId < 0)) {
                    
                    user = await this.db.FindUserAsync(vault.Username, cancellationToken);
                    if(user != null)
                    {
                        record.User = user;
                        record.UserId= user.Id;
                        vault.UserId = user.Id;
                    } else {
                        vault.UserId = null;
                    }
                }

                if(record.UserId != vault.UserId)
                    record.UserId = vault.UserId;


                OperationalEnvironmentRecord env = null;
                if(!string.IsNullOrWhiteSpace(vault.OperationalEnvironmentName)
                    && (!vault.OperationalEnvironmentId.HasValue 
                    || vault.OperationalEnvironmentId < 1)) {

                    env = await this.db.FindOperationalEnvironmentAsync(
                        vault.OperationalEnvironmentName, cancellationToken);

                    if(env != null)
                    {
                        record.OperationalEnvironment = env;
                        record.OperationalEnvironmentId = env.Id;
                        vault.OperationalEnvironmentId = env.Id;
                    } else {
                        vault.OperationalEnvironmentId = null;
                    }
                }

                if(record.OperationalEnvironmentId != vault.OperationalEnvironmentId)
                    record.OperationalEnvironmentId = vault.OperationalEnvironmentId;

                PublicKeyRecord key = null;
                if(!string.IsNullOrWhiteSpace(vault.PublicKeyUriPath)
                    && (!vault.PublicKeyId.HasValue || vault.PublicKeyId < 1)) {

                    key = await this.db.PublicKeys
                            .FirstOrDefaultAsync(o => o.UriPath == vault.PublicKeyUriPath)
                            .ConfigureAwait(false);

                    if(key != null)
                    {
                        record.KeyType = (short)ProtectedBlobKeyType.Certificate;
                        record.PublicKey = key;
                        record.PublicKeyId = key.Id;
                        vault.PublicKeyId = key.Id;
                    } else {
                        vault.PublicKeyId = null;
                    }
                }

                if(record.PublicKeyId != vault.PublicKeyId) {
                    record.PublicKeyId = vault.PublicKeyId;
                    record.KeyType = (short)ProtectedBlobKeyType.Certificate;
                }
            }

            record.Name = vault.Name;
            

            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

                        
        }


        private static ProtectedBlobVault Map()
    }
}