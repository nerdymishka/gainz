using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;

namespace NerdyMishka.Identity
{

    public partial class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :
        IUserTwoFactorStore<TUser>,
        IUserTwoFactorRecoveryCodeStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserLoginStore<TUser>
    {
        private const string InternalLoginProvider = "[NerdyMishkaUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string RecoveryCodeTokenName = "RecoveryCodes";

        protected virtual TUserToken CreateUserToken(TUser user, string loginProvider, string name, string value)
        {
            return new TUserToken
            {
                UserId = user.Id,
                ProviderName = loginProvider,
                Name = name,
                Value = value
            };
        }

        protected virtual TUserLogin CreateUserLogin(TUser user, UserLoginInfo login)
        {
            return new TUserLogin
            {
                UserId = user.Id,
                Key = login.ProviderKey,
                ProviderName = login.LoginProvider,
                DisplayName = login.ProviderDisplayName
            };
        }

        protected virtual Task AddUserTokenAsync(TUserToken token)
        {
            this.db.Set<UserToken>().Add(token);
            return Task.CompletedTask;
        }

        protected virtual Task RemoveUserTokenAsync(TUserToken token)
        {
            this.db.Set<UserToken>().Remove(token);
            return Task.CompletedTask;
        }

        public virtual Task<TUserToken> FindTokenAsync(
            TUser user,
            string loginProvider, 
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
            => this.UserTokens.FindAsync(new object[] { user.Id, loginProvider, name }, cancellationToken);

        public virtual async Task<int> CountCodesAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            if (mergedCodes.Length > 0)
            {
                return mergedCodes.Split(';').Length;
            }
            return 0;
        }

        public Task<string> GetAuthenticatorKeyAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public virtual async Task<string> GetTokenAsync(
            TUser user, 
            string loginProvider, 
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await this.FindTokenAsync(user, loginProvider, name, cancellationToken);
            return entry?.Value;
        }

        public virtual async Task<bool> GetTwoFactorEnabledAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            
            if(user.OrganizationId.HasValue)
            {
                var orgPolicy = await (from o in this.db.Set<MultiFactorPolicy>() 
                    join u in this.Users on o.Id equals u.MultiFactorPolicyId
                    select o).SingleOrDefaultAsync();

                if(orgPolicy != null && orgPolicy.IsEnabled)
                    return true;
            }

            var policy = await (from o in this.db.Set<MultiFactorPolicy>() 
                    join u in this.db.Set<TUser>() on o.Id equals u.MultiFactorPolicyId
                    select o).SingleOrDefaultAsync();

            if(policy != null && policy.IsEnabled)
                return true;

            return false;
        }

        public virtual async Task<bool> RedeemCodeAsync(
            TUser user, 
            string code, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
             cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var mergedCodes = await GetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, cancellationToken) ?? "";
            var splitCodes = mergedCodes.Split(';');
            if (splitCodes.Contains(code))
            {
                var updatedCodes = new List<string>(splitCodes.Where(s => s != code));
                await ReplaceCodesAsync(user, updatedCodes, cancellationToken);
                return true;
            }
            return false;
        }

        public virtual async Task RemoveTokenAsync(
            TUser user, 
            string loginProvider, 
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
             cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (entry != null)
            {
                await RemoveUserTokenAsync(entry);
            }
        }

        public Task ReplaceCodesAsync(
            TUser user, 
            IEnumerable<string> recoveryCodes, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var mergedCodes = string.Join(";", recoveryCodes);
            return this.SetTokenAsync(user, InternalLoginProvider, RecoveryCodeTokenName, mergedCodes, cancellationToken);
        }

        public Task SetAuthenticatorKeyAsync(
            TUser user, 
            string key, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public virtual async Task SetTokenAsync(
            TUser user, 
            string loginProvider, 
            string name, 
            string value, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var token = await FindTokenAsync(user, loginProvider, name, cancellationToken);
            if (token == null)
            {
                await AddUserTokenAsync(CreateUserToken(user, loginProvider, name, value));
            }
            else
            {
                token.Value = value;
            }
        }

        public virtual async Task SetTwoFactorEnabledAsync(
            TUser user, 
            bool enabled,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
   
            var policy = await (from o in this.db.Set<MultiFactorPolicy>() 
                         join u in this.db.Set<TUser>() on o.Id equals u.MultiFactorPolicyId
                         select o).SingleOrDefaultAsync();

            if(policy == null)
            {
                policy = new MultiFactorPolicy(){
                    IsEnabled = enabled,
                };
                this.db.Set<MultiFactorPolicy>().Add(policy);
                user.MultiFactorPolicyId = policy.Id;
            }

            policy.IsEnabled = enabled;
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return Task.FromResult(Guid.NewGuid().ToString());
        }

         /// <summary>
        /// Return a user login with the matching userId, provider, providerKey if it exists.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected virtual Task<TUserLogin> FindUserLoginAsync(
            int userId, 
            string loginProvider, 
            string providerKey, 
             CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.UserLogins.SingleOrDefaultAsync(userLogin => userLogin.UserId.Equals(userId) 
                && userLogin.ProviderName == loginProvider 
                && userLogin.Key == providerKey, cancellationToken);
        }

        /// <summary>
        /// Return a user login with  provider, providerKey if it exists.
        /// </summary>
        /// <param name="loginProvider">The login provider name.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The user login if it exists.</returns>
        protected virtual Task<TUserLogin> FindUserLoginAsync(
            string loginProvider, 
            string providerKey, 
             CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.UserLogins.SingleOrDefaultAsync(
                userLogin => userLogin.ProviderName == loginProvider 
                && userLogin.Key == providerKey, cancellationToken);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (login == null)
            {
                throw new ArgumentNullException(nameof(login));
            }
            this.db.Set<UserLogin>().Add(CreateUserLogin(user, login));
            return Task.FromResult(false);
        }

        public virtual async Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var entry = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
            if (entry != null)
            {
                this.UserLogins.Remove(entry);
            }
        }

      
        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userId = user.Id;
            return await this.UserLogins.Where(l => l.UserId.Equals(userId))
                .Select(l => new UserLoginInfo(l.ProviderName, l.Key, l.DisplayName)).ToListAsync(cancellationToken);
        }

        public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
             cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
            if (userLogin != null)
            {
                return await this.Users.SingleOrDefaultAsync( o => o.Id == userLogin.UserId, cancellationToken);
            }
            return null;
        }
    }

}