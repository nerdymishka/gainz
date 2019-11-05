using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;

namespace NerdyMishka.Identity
{

    public partial class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :
       IUserPasswordStore<TUser>,
       IUserLockoutStore<TUser>
    {
        public async virtual Task<int> GetAccessFailedCountAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null || !login.FailureCount.HasValue)
                return 0;

            return login.FailureCount.Value;
        }

        public async virtual Task<bool> GetLockoutEnabledAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            
            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);

            if(login == null)
                return false;

            return login.IsLockedOut;
        }

        public async virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(
            TUser user,  
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

          
            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);

            if(login == null && !login.LockOutEndedAt.HasValue)
                return null;

            return login.LockOutEndedAt;
        }

        public virtual async Task<string> GetPasswordHashAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

           
            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            return login?.Password;
        }

        public virtual async Task<bool> HasPasswordAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
             cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return await this.PasswordLogins.CountAsync(o => o.UserId == user.Id && o.Password != null) > 0;
        }

        public async virtual Task<int> IncrementAccessFailedCountAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

          
            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null)
                return 0;

            short count = 0;
            if(login.FailureCount.HasValue)
                count = login.FailureCount.Value;
            
            count += 1;
            login.FailureCount = count;
            return count;
        }

        public async virtual Task ResetAccessFailedCountAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
             cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));


            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null)
                return;

            login.FailureCount = 0;
        }

        public async virtual Task SetLockoutEnabledAsync(
            TUser user, 
            bool enabled, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null)
                return;

            if(!login.IsLockedOut && enabled)
            {
                login.IsLockedOut = true;
                login.LockOutStartedAt = DateTime.UtcNow;
                return;
            }

            if(login.IsLockedOut && !enabled)
            {
                login.IsLockedOut = false;
                login.LockOutStartedAt = null;
                login.LockOutEndedAt = null;
            }
        }

        public async virtual Task SetLockoutEndDateAsync(
            TUser user, 
            DateTimeOffset? lockoutEnd, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null)
                return;

            if(!lockoutEnd.HasValue)
            {
                login.LockOutEndedAt = null;
                login.LockOutStartedAt = null;
                return;
            }

            login.LockOutEndedAt = lockoutEnd.Value.DateTime;
        }

        public virtual async Task SetPasswordHashAsync(
            TUser user, 
            string passwordHash, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException(nameof(passwordHash));

            var login = await this.PasswordLogins.SingleOrDefaultAsync(o => o.UserId == user.Id);
            if(login == null)
            {
                login  = new PasswordLogin() {
                    UserId = user.Id,  
                };
                this.PasswordLogins.Add(login);
            }

            login.Password = passwordHash;
            login.UpdatedAt = DateTime.UtcNow;
        }
    }

}