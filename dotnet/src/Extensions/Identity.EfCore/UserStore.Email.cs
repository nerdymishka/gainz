using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;
using System.Linq;
using System.Text;

namespace NerdyMishka.Identity
{

    public partial class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :
       IUserEmailStore<TUser>,
       IUserPhoneNumberStore<TUser>
    {
        protected bool IsProtected => this.Privacy != null && this.Privacy.IsProtected && this.EmailHasher != null;

        public virtual async Task<TUser> FindByEmailAsync(
            string normalizedEmail, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(string.IsNullOrWhiteSpace(normalizedEmail))
                throw new ArgumentNullException(nameof(normalizedEmail));

           
            if(this.IsProtected)
            {
                var hash = this.EmailHasher.ComputeHash(normalizedEmail, 
                    System.Text.Encoding.UTF8.GetBytes(this.Privacy.Salt));

                return await this.Store.FirstOrDefaultAsync(o => o.Email == hash);
            }

            return await this.Store.FirstOrDefaultAsync(o => o.Email == normalizedEmail);
        }

        public virtual async Task<string> GetEmailAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));
            
            var store = this.Db.Set<EmailAddress>();
            int purpose = (int)EmailPurpose.Primary;
            var email = await store.SingleOrDefaultAsync(o => 
                o.UserId == user.Id && 
                o.Purpose == purpose);

            return email?.Value;
        }

        public virtual Task<bool> GetEmailConfirmedAsync(
            TUser user,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.IsEmailConfirmed);
        }

        public virtual async Task<string> GetNormalizedEmailAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(!this.IsProtected)
                return user.Email;

            var store = this.Db.Set<EmailAddress>();
            int purpose = (int)EmailPurpose.Primary;
            var email = await store.SingleOrDefaultAsync(o => 
                o.UserId == user.Id && 
                o.Purpose == purpose);

           
            return email?.Value?.ToLowerInvariant();
        }

        public async virtual Task<string> GetPhoneNumberAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var store = this.Db.Set<Phone>();
            var phone = await store.SingleOrDefaultAsync(o => 
                o.UserId == user.Id && 
                o.Purpose == PhonePurpose.Mobile);

            if(phone == null)
                return null;

            return phone?.Value;
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));
                
            return Task.FromResult(user.IsPhoneConfirmed);
        }

        public virtual async Task SetEmailAsync(
            TUser user, 
            string email,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));


           

            var store = this.Db.Set<EmailAddress>();
            int id = user.Id;
            int purpose = (int)EmailPurpose.Primary;
            var model = store.SingleOrDefault(
                    o => o.UserId == id && o.Purpose == purpose);

          
             
               

            if(model == null){
                model = new EmailAddress() {
                    UserId = user.Id,
                    Value = email,
                    Purpose = (int)EmailPurpose.Primary
                };

                this.Db.Add(model);
            } else {
                model.Value = email;
            }

            await SetNormalizedEmailAsync(user, email.ToLowerInvariant(), cancellationToken);
        }

        public virtual Task SetEmailConfirmedAsync(
            TUser user, 
            bool confirmed,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            user.IsEmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        public virtual Task SetNormalizedEmailAsync(
            TUser user, 
            string normalizedEmail, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));
        

            normalizedEmail = normalizedEmail?.ToLowerInvariant();
            if(string.IsNullOrWhiteSpace(normalizedEmail))
            {
                user.Email = normalizedEmail;
                return Task.CompletedTask;
            }
        
            if(this.IsProtected)
            {
                var hash = this.EmailHasher.ComputeHash(normalizedEmail, 
                    System.Text.Encoding.UTF8.GetBytes(this.Privacy.Salt));

                user.Email = hash;

                return Task.CompletedTask;    
            }

            user.Email = normalizedEmail;

            return Task.CompletedTask;
        }

        public async virtual Task SetPhoneNumberAsync(
            TUser user, 
            string phoneNumber, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var store = this.Db.Set<Phone>();
            var phone = await store.SingleOrDefaultAsync(o => 
                o.UserId == user.Id && 
                o.Purpose == PhonePurpose.Mobile, 
                cancellationToken);

            if(phone == null)
            {
                phone = new Phone(){
                    UserId = user.Id,
                    Purpose = PhonePurpose.Mobile,
                    Value = phoneNumber
                };

                store.Add(phone);
            }

            phone.Value =phoneNumber;
        }

        public Task SetPhoneNumberConfirmedAsync(
            TUser user, 
            bool confirmed, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));
                
            user.IsPhoneConfirmed = confirmed;

            return Task.CompletedTask;
        }
    }
}