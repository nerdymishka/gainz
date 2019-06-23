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

namespace NerdyMishka.Identity
{


    public class UserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IUserRoleStore<TUser>
        where TUser : User, new()
    {
        private bool isDisposed = false;
        private DbContext db;

        private NerdyMishka.Security.Cryptography.IPasswordAuthenticator authenticator;

        public IQueryable<TUser> Users => this.db.Set<TUser>();

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

        public Task<int> CountCodesAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            var store = this.db.Set<TUser>();
            store.Add(user);

            await this.db.SaveChangesAsync(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
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


        protected void ThrowIfDisposed()
        {
            if(this.isDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RedeemCodeAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ReplaceCodesAsync(TUser user, IEnumerable<string> recoveryCodes, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            var lowered = email.ToLowerInvariant();
            var addresses = user.EmailAddresses;

            if(addresses == null) {
                addresses = await this.db.Set<EmailAddress>()
                    .Where(o => o.UserId == user.Id)
                    .ToListAsync();
            }

            if(string.IsNullOrWhiteSpace(email))
            {
                if(user.EmailHash != null)
                {
                    user.EmailHash = null;
                    var primary = addresses.SingleOrDefault(
                            o => o.Purpose == EmailPurpose.Primary &&
                            o.Value == email);
                            
                    if(primary != null)
                       this.db.Remove(primary);
                }
               
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(lowered);
            var hash = authenticator.ComputeHash(bytes);
            if(user.EmailHash != hash)
            {
                user.EmailHash = hash;
                var primary = addresses.SingleOrDefault(
                            o => o.Purpose == EmailPurpose.Primary &&
                            o.Value == email);

                if(primary == null)
                {
                    primary = new EmailAddress() { 
                        UserId = user.Id,
                        Name = "Primary",
                        Purpose = EmailPurpose.Primary,
                        Value = email 
                    };
                    
                    await this.db.AddAsync(primary,cancellationToken);
                }

                user.IsEmailConfirmed = false;

                await this.db.SaveChangesAsync();
            }
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetTokenAsync(TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }


}
