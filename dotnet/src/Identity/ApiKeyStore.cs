using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;

namespace NerdyMishka.Identity
{


    public abstract class ApiKeyStoreBase<TApiKey, TApiKeyRole> 
        
        
        where TApiKey : ApiKey, new()
        where TApiKeyRole : ApiKeyRole, new()
    {
        private bool isDisposed;

        public IQueryable<TApiKey> ApiKeys => this.Db.Set<TApiKey>().AsQueryable();

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        protected DbContext Db { get; set; }

        public bool AutoSave { get; set; }

        public virtual async Task<IdentityResult> CreateAsync(TApiKey apiKey, CancellationToken cancellationToken = default(CancellationToken))
        {

           
            this.Db.Add(apiKey);
            await this.SaveChanges(cancellationToken);
           
            

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TApiKey apiKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            this.Db.Attach(apiKey);
            //apiKey.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Db.Update(apiKey);
            try
            {
                await this.SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }



        public virtual async Task<IdentityResult> DeleteAsync(TApiKey apiKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            this.Db.Remove(apiKey);
            try
            {
                await this.SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }
            return IdentityResult.Success;
        }

        public virtual Task<DateTime?> GetExpiresAtAsync(
            TApiKey apiKey, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(apiKey.ExpiresAt);
        }

        public virtual  Task<string> GetNameAsync(
            TApiKey apiKey, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(apiKey.Name);
        }

        public virtual Task<string> GetNormalizedNameAsync(
            TApiKey apiKey, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(apiKey.Code);
        }

        public virtual Task SetHashAsync(TApiKey apiKey, 
            string hash, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            apiKey.Value = hash;
            return Task.CompletedTask;
        }


        public virtual Task SetExpiresAtAsync(TApiKey apiKey, DateTime? expiresAt, CancellationToken cancellationToken = default(CancellationToken))
        {

            apiKey.ExpiresAt = expiresAt;

            return Task.CompletedTask;
        }

        public virtual Task SetNameAsync(TApiKey apiKey, string name, CancellationToken cancellationToken = default(CancellationToken))
        {
            apiKey.Name = name;
            return Task.CompletedTask;
        }

        public virtual Task SetNormalizedNameAsync(TApiKey apiKey, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            apiKey.Code = normalizedName;
            return Task.CompletedTask;
        }



        public virtual async Task<bool> HasApiKeyAsync(string clientId, string apiHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.ApiKeys.AnyAsync(o => o.ClientId == clientId);
        }

        public virtual async Task<IList<string>> GetApiKeysAsync(string clientId, CancellationToken cancellationToken = default (CancellationToken))
        {
            return await this.ApiKeys
                .Where(o => o.ClientId == clientId)
                .Select(o => o.Value)
                .ToListAsync();
        }

        public abstract Task<IList<Claim>> GetApiRoleClaimsAsync(ApiKey key, CancellationToken cancellationToken = default(CancellationToken));

        public abstract Task<IList<string>> GetApiRoleNamesAsync(ApiKey key, CancellationToken cancellationToken = default(CancellationToken));
      


        protected virtual async Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            if(this.AutoSave)
            {
                await this.Db.SaveChangesAsync(cancellationToken);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if(this.isDisposed)
                return;
            
            if(disposing)
            {
                
            }

            this.isDisposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if(this.isDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        ~ApiKeyStoreBase() {
            this.Dispose(false);
        }
    }
}