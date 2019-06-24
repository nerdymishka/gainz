using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;
using NerdyClaimTypes = NerdyMishka.EfCore.Identity.ClaimTypes;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Identity
{

    public class UserStore : UserStoreBase<User, Role>
    {
        public UserStore(
            DbContext dbContext, 
            IPasswordAuthenticator authenticator = null) :base(dbContext, authenticator)
        {

        }
    }

    public class UserStoreBase<TUser, TRole> :

        IUserStore<TUser>,
        IQueryableUserStore<TUser>
        where TUser : User, new()
    {
        private bool isDisposed = false;
        private DbContext db;

        private IPasswordAuthenticator authenticator;

        public UserStoreBase(DbContext dbContext, IPasswordAuthenticator authenticator = null) {
            if(dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));
            
            this.db = dbContext;
            this.authenticator = authenticator;
        }

        public IQueryable<TUser> Users => this.db.Set<TUser>();

        /*
        public virtual async Task AddClaimsAsync(
            TUser user, 
            IEnumerable<Claim> claims, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            if(claims == null)
                throw new NullReferenceException(nameof(claims));

            var custom = new List<Claim>();
            foreach(var claim in claims)
            {
                
                switch(claim.Type)
                {
                    case NerdyClaimTypes.Role:
                        await this.AddToRoleAsync(user, claim.Value, cancellationToken);
                    break;
                    

                    case NerdyClaimTypes.DisplayName:
                    case ClaimTypes.Name:
                        user.DisplayName = claim.Value;
                    break;

                    case ClaimTypes.Email:
                    case NerdyClaimTypes.UPN:
                    case NerdyClaimTypes.Email:
                        await this.SetEmailAsync(user, claim.Value, cancellationToken);
                        break;

                    default:
                       custom.Add(claim);
                    break;
                }
            }

            if(custom.Count > 0)
            {
                var store = this.db.Set<UserClaim>();
                var set = await store
                    .Where(o => o.UserId == user.Id)
                    .ToListAsync();

              
                foreach(var claim in custom)
                {
                    if(!set.Any(o => o.Type == claim.Type && o.Value == claim.Value))
                    {
                       
                        await store.AddAsync(new UserClaim(){
                            UserId =user.Id,
                            Type = claim.Type,
                            Value = claim.Value
                        });
                    }
                }

                await this.db.SaveChangesAsync(cancellationToken);
            }            
        }
        
        

        
        public virtual async Task AddLoginAsync(
            TUser user, 
            UserLoginInfo login, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = this.db.Set<UserLogin>();

            var info = await store
                .SingleOrDefaultAsync(o => o.UserId == user.Id 
                    && o.ProviderName == login.LoginProvider);

            if(info == null)
            {
                await store.AddAsync(new UserLogin() {
                    UserId = user.Id,
                    DisplayName = login.ProviderDisplayName,
                    Key = login.ProviderKey,
                    ProviderName = login.LoginProvider
                }, cancellationToken);

                await this.db.SaveChangesAsync(cancellationToken);
            }
        }
        */

        public virtual async Task<IdentityResult> CreateAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            var store = this.db.Set<TUser>();
            store.Add(user);

            if(!string.IsNullOrWhiteSpace(user.Pseudonym))
            {
                if(user.DisplayName == null)
                    user.DisplayName = user.Pseudonym;

                user.Pseudonym = user.Pseudonym.ToLowerInvariant();
            }

            await this.db.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            var store = this.db.Set<TUser>();
            store.Remove(user);

            await this.db.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }


        

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<string> GetUserIdAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Id.ToString());
        }

        public virtual Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.DisplayName);
        }

        public virtual Task SetUserNameAsync(
            TUser user, 
            string userName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            if(!string.IsNullOrWhiteSpace(user.DisplayName) 
                && user.DisplayName.ToLowerInvariant() == user.Pseudonym)
            {
                user.DisplayName = userName;
            }
            user.Pseudonym = userName.ToLowerInvariant();

            

            return Task.CompletedTask;
        }

        public virtual Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            return Task.FromResult(user.Pseudonym);
        }

        public virtual Task SetNormalizedUserNameAsync(
            TUser user, 
            string normalizedName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            user.Pseudonym = normalizedName.ToLowerInvariant();

            return Task.CompletedTask;
        }

        public async virtual Task<IdentityResult> UpdateAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            var store = this.db.Set<TUser>();
            if(string.IsNullOrWhiteSpace(user.DisplayName))
            {
                user.DisplayName = user.Pseudonym;
            }

            user.Pseudonym = user.Pseudonym.ToLowerInvariant();

            store.Attach(user);
            //user.ConcurrencyStamp = Guid.NewGuid().ToString();
            store.Update(user);

            
            await this.db.SaveChangesAsync(cancellationToken);
           
           

            return IdentityResult.Success;
        }

        public virtual async Task<TUser> FindByIdAsync(
            int userId, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
           
            return await this.Users.SingleOrDefaultAsync(o => o.Id == userId);
        }

        public virtual Task<TUser> FindByIdAsync(
            string userId, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return FindByIdAsync(int.Parse(userId), cancellationToken);
        }

        public async Task<TUser> FindByNameAsync(
            string normalizedUserName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

             if(string.IsNullOrWhiteSpace(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            return await this.Users.SingleOrDefaultAsync(o => o.Pseudonym == normalizedUserName);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if(this.isDisposed)
                return;
            
            if(disposing)
            {
                this.db = null;
                this.authenticator = null;
            }

            this.isDisposed = true;
        }

        protected void ThrowIfDisposed()
        {
            if(this.isDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        ~UserStoreBase() {
            this.Dispose(false);
        }
    }


}
