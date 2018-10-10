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
using System.Collections.ObjectModel;

namespace Nexus.Services
{
    public class ProtectedBlobService
    {
        private NexusDbContext db;

        private CompositeKey globalKey;

        private UserCompositeKey userKey;


        
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


            return this.Map(record);

        }

         public async Task<ProtectedBlob> SaveAsync(
            ProtectedBlob blob,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ProtectedBlobRecord record = null;
            int id = blob.Id.HasValue ? (int)blob.Id : 0;

        

            if(id > 0) {
                record = await this.db.ProtectedBlobs
                    .SingleOrDefaultAsync(o => o.Id == id, cancellationToken)
                    .ConfigureAwait(false);
            }

            if(record == null)
            {
                if(string.IsNullOrWhiteSpace(blob.VaultName) && 
                    (!blob.VaultId.HasValue || blob.VaultId  < 1)) {
                    throw new InvalidOperationException("Blob must belong to a vault");
                }

                record = new ProtectedBlobRecord() {
                    Id = id
                };

                await this.db.ProtectedBlobs.AddAsync(record);
                
                // assigned users/environments, etc shouldn't
                // change after its created. if it does need to 
                // change a different call should be required.
                
                ProtectedBlobVaultRecord vault = null;
                if(!string.IsNullOrWhiteSpace(blob.VaultName) &&
                    (!blob.VaultId.HasValue || blob.VaultId < 0)) {
                    
                    vault = await this.db.Vaults
                        .FirstOrDefaultAsync(o => o.Name == blob.VaultName)
                        .ConfigureAwait(false);

                    if(vault != null)
                    {
                        record.Vault = vault;
                        blob.VaultId = vault.Id;
                        blob.VaultName = vault.Name;
                    } else {
                        blob.VaultId = null;
                    }
                }
            }

            if(record.ProtectedBlobVaultId != blob.VaultId)
                record.ProtectedBlobVaultId = blob.VaultId.Value;

            if(record.Vault == null)
            {
                record.Vault = await this.db.Vaults
                    .FirstOrDefaultAsync(o => o.Name == blob.VaultName)
                    .ConfigureAwait(false);
            }


            record.BlobType = (short)Enum.Parse(typeof(ProtectedBlobType), blob.BlobType);
            var bytes = Convert.FromBase64String(blob.Base64Blob);
            var kt = (ProtectedBlobKeyType)record.Vault.KeyType;
            switch(kt)
            {
                case ProtectedBlobKeyType.Composite:
                    bytes = DataProtection.EncryptBlob(bytes, this.globalKey);
                    break;
                case ProtectedBlobKeyType.User:
                    bytes = DataProtection.EncryptBlob(bytes, this.userKey);
                    break;

                default:
                    throw new NotSupportedException($"{kt.ToString()} is not currently supported.");
            }

            record.Blob = bytes;
            record.Tags = blob.Tags;
            record.ExpiresAt = blob.ExpiresAt;
            record.UriPath = blob.UriPath;
            
            // vault must never be updated.

            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);


            return this.Map(record, kt, CryptoAction.Clear);

        }

        public enum CryptoAction
        {
            None,
            Decrypt,
            Encrypt,
            Clear 
        }

        private ProtectedBlob Map(ProtectedBlobRecord record, ProtectedBlobKeyType kt, CryptoAction action = CryptoAction.Decrypt)
        {
            var bt = (ProtectedBlobType)record.BlobType;
            
            var bytes = record.Blob;

            if(action != CryptoAction.None)
            {
                switch(kt)
                {
                    case ProtectedBlobKeyType.Composite:
                        bytes = DataProtection.DecryptBlob(bytes, this.globalKey);
                        break;
                    case ProtectedBlobKeyType.User:
                        bytes = DataProtection.DecryptBlob(bytes, this.userKey);
                        break;
                    default:
                        break;
                }
            }

            if(action == CryptoAction.Clear)
            {
                bytes = Array.Empty<byte>();
            }

            return new ProtectedBlob() {
                Id = record.Id,
                UriPath = record.UriPath,
                BlobType = (record.BlobType).ToString(),
                Base64Blob = Convert.ToBase64String(bytes),
                VaultId = record.Vault?.Id,
                VaultName = record?.Vault?.Name,
                Private = false ,
                ExpiresAt = record.ExpiresAt,
                Tags = record.Tags
            };
        }

        private  ProtectedBlobVault Map(ProtectedBlobVaultRecord record)
        {
            var col = new Collection<ProtectedBlob>();

            if(record.ProtectedBlobs != null & record.ProtectedBlobs.Count > 0)
            {
                // don't send the data unless specifically requested.
                foreach(var blob in record.ProtectedBlobs)
                    col.Add(Map(blob, (ProtectedBlobKeyType)record.KeyType, CryptoAction.Clear));
            }

            return new ProtectedBlobVault() {
                Id = record.Id,
                Name = record.Name,
                KeyType = ((ProtectedBlobKeyType)record.KeyType).ToString(),
                PublicKeyId = record?.PublicKeyId,
                PublicKeyUriPath = record?.PublicKey?.UriPath,
                Entropy = record.Entropy,
                UserId = record.UserId,
                Username = record.User?.Name,
                OperationalEnvironmentId = record.OperationalEnvironmentId,
                OperationalEnvironmentName = record.OperationalEnvironment?.Name,
                Blobs = col 
            };
        }
    }
}