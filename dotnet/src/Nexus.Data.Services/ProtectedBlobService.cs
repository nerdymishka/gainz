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


        public async Task<ProtectedBlob> FindOneAsync(
            string name,
            CryptoAction action = CryptoAction.None,
            CancellationToken cancellationToken = default(CancellationToken) 
        )
        {
            string vault = null;
            if(name.Contains("://")) {
                var index = name.IndexOf("://");
                vault = name.Substring(0, index);
                name = name.Substring(index + 3);

                return await FindOneAsync(name, vault, action,  cancellationToken);
            }

            throw new Exception("name must contain :// to note a vault uri");
        }

        public async Task<ProtectedBlob> FindOneAsync(
            string name,
            string vault,
            CryptoAction action = CryptoAction.None,
            CancellationToken cancellationToken = default(CancellationToken) 
        )
        {
            var result = await (from o in this.db.ProtectedBlobs.Include(c => c.Vault)
                where o.UriPath == name && o.Vault.Name == vault 
                select o)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            var kt = (ProtectedBlobKeyType)result.Vault.KeyType;

            return this.Map(result, kt, action);
        }


        public async Task<byte[]> DecryptAsync(
            string name,
            CancellationToken cancellationToken = default(CancellationToken) 
        )
        {
            string vault = null;
            if(name.Contains("://")) {
                var index = name.IndexOf("://");
                vault = name.Substring(0, index);
                name = name.Substring(index + 3);

                return await DecryptAsync(name, vault,  cancellationToken);
            }

            throw new Exception("name must contain :// to note a vault uri");
        }

        public async Task<byte[]> DecryptAsync(
            string name,
            string vault,
            CancellationToken cancellationToken = default(CancellationToken) 
        ){
            var result = await (from o in this.db.ProtectedBlobs.Include(c => c.Vault)
                where o.UriPath == name && o.Vault.Name == vault 
                select new { KeyType = o.Vault.KeyType, Blob = o.Blob })
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);
             
            var kt = (ProtectedBlobKeyType)result.KeyType;
            byte[] bytes = null;
            switch(kt)
            {
                case ProtectedBlobKeyType.Composite:
                    bytes = DataProtection.DecryptBlob(result.Blob, this.globalKey);
                    break;
                case ProtectedBlobKeyType.User:
                    bytes = DataProtection.DecryptBlob(result.Blob, this.userKey);
                    break;
                default:
                    throw new NotSupportedException(kt.ToString());
                    
            }
            
            return bytes;
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


      
      
        private ProtectedBlob Map(ProtectedBlobRecord record, ProtectedBlobKeyType kt, CryptoAction action = CryptoAction.None)
        {
            var bt = (ProtectedBlobType)record.BlobType;
            
            var bytes = record.Blob;

            if(action  == CryptoAction.Decrypt)
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
                BlobType = (bt).ToString(),
                Base64Blob = Convert.ToBase64String(bytes),
                VaultId = record.Vault?.Id,
                VaultName = record?.Vault?.Name,
                Private = false ,
                ExpiresAt = record.ExpiresAt,
                Tags = record.Tags
            };
        }

       
    }
}