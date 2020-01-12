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

    public class UserStore : 
        UserStore<User, Role, Permission, IdentityDbContext>
    {
    
        public UserStore(
            IdentityDbContext dbContext) :base(dbContext)
        {

        }
    }


    public class UserStore<TUser, TRole, TPermission, TContext> : 
        UserStoreBase<User, Role, UserClaim, UserRole, UserLogin, UserToken, RoleClaim, Permission, RolePermission>
        where TContext : DbContext
    {
        

        public UserStore(
            TContext dbContext) :base()
        {
            this.Db = (DbContext)dbContext;
        }
    }

    public abstract partial class UserStoreBase<TUser, TRole,  TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim, TPermission, TRolePermission>
        : UserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>,
        IUserPermissionStore<TUser>
        where TUser : User, new()
        where TUserClaim: UserClaim, new()
        where TUserLogin: UserLogin, new()
        where TUserToken: UserToken, new()
        where TRole : Role, new()
        where TUserRole: UserRole, new()
        where TRoleClaim: RoleClaim, new()
        where TPermission: Permission, new()
        where TRolePermission: RolePermission, new()
    {

        protected virtual DbSet<TPermission> Permissions => this.Db.Set<TPermission>();

        protected virtual DbSet<TRolePermission> RolePermissions => this.Db.Set<TRolePermission>();

        public virtual async Task<IList<Claim>> GetPermissionClaimsAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var permissions = await GetPermissionAsync(user);
            return permissions.Select(o => o.ToClaim())
                .ToArray();
        }

          public virtual async Task<IList<TPermission>> GetPermissionAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                join ur in this.UserRoles on rp.RoleId equals ur.RoleId
                where ur.UserId == user.Id
                select p).Distinct()
                .ToListAsync();
        }

        public virtual async Task<IList<string>> GetPermissionNamesAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                join ur in this.UserRoles on rp.RoleId equals ur.RoleId
                where ur.UserId == user.Id
                select p.Code).Distinct().ToListAsync();
        }

        public virtual async Task<bool> IsInPermissionAsync(
            TUser user, 
            string permissionName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                join ur in this.UserRoles on rp.RoleId equals ur.RoleId
                where ur.UserId == user.Id && p.Code == permissionName
                select p.Code).CountAsync() > 0;
        }
    } 


    public abstract partial class UserStoreBase<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> 
        : UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken>
        where TUser : User, new()
        where TUserClaim: UserClaim, new()
        where TUserLogin: UserLogin, new()
        where TUserToken: UserToken, new()
        where TRole : Role, new()
        where TUserRole: UserRole, new()
        where TRoleClaim: RoleClaim, new()
    {

        protected DbSet<TRole> Roles => this.Db.Set<TRole>();

        protected DbSet<TUserRole> UserRoles => this.Db.Set<TUserRole>();

        public override async Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken = default(CancellationToken))
         {
             cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            
            var role = await this.Roles.SingleOrDefaultAsync(o => o.Name == roleName || o.Code == roleName);
            if(role == null)
                throw new InvalidOperationException($"Role '{roleName}' is undefined");

            var result = await this.UserRoles
                .SingleOrDefaultAsync(o => o.UserId == user.Id && o.RoleId == role.Id);

            if(result != null)
                return;

            this.UserRoles.Add(new TUserRole() {
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        public async override Task<IList<string>> GetRolesAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return await (from o in this.UserRoles
                        join r in this.Roles on o.RoleId equals r.Id
                        where o.UserId == user.Id
                        select o.Role.Name).ToListAsync();

        }


        public override async Task<bool> IsInRoleAsync(
            TUser user, 
            string roleName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(roleName == null)
                throw new ArgumentNullException(nameof(roleName));

            var count = await (from o in this.UserRoles
                      join r in this.Roles on o.RoleId equals r.Id
                      where r.Name == roleName || r.Code == roleName
                      select o).CountAsync();

            return count > 0;
        }


        public override async Task RemoveFromRoleAsync(
            TUser user, 
            string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException(nameof(roleName));

            var roles = await (from o in this.UserRoles
                            join r in this.Roles on o.RoleId equals r.Id
                            where r.Name == roleName || r.Code == roleName &&
                                o.UserId == user.Id
                            select o).ToListAsync();

            if(roles != null)
                this.UserRoles.RemoveRange(roles);
        }
    }


    public abstract partial class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :

        IUserStore<TUser>,
        IQueryableUserStore<TUser>
        where TUser : User, new()
        where TUserClaim: UserClaim, new()
        where TUserLogin: UserLogin, new()
        where TUserToken: UserToken, new()
    {
        private bool isDisposed = false;

            /// <summary>
        /// Gets the database context for this store.
        /// </summary>
        protected virtual DbContext Db { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public bool AutoSave { get; set; } = true;

        public PrivacyOptions Privacy { get; protected set; }

        public virtual IEmailHash EmailHasher { get; protected set; }

        private IPasswordAuthenticator authenticator;

        public IQueryable<TUser> Users => this.Db.Set<TUser>();

        protected DbSet<TUser> Store => this.Db.Set<TUser>();

        protected DbSet<TUserClaim> UserClaims => this.Db.Set<TUserClaim>();

        protected DbSet<TUserLogin> UserLogins => this.Db.Set<TUserLogin>();

        protected DbSet<TUserToken> UserTokens => this.Db.Set<TUserToken>();

        protected DbSet<MultiFactorPolicy> MfaPolicies => this.Db.Set<MultiFactorPolicy>();

        protected DbSet<PasswordLogin> PasswordLogins => this.Db.Set<PasswordLogin>();

        protected DbSet<PasswordPolicy> PasswordPolicies => this.Db.Set<PasswordPolicy>();

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
                var store = this.Db.Set<UserClaim>();
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

                await this.Db.SaveChangesAsync(cancellationToken);
            }            
        }
        
        

        
        public virtual async Task AddLoginAsync(
            TUser user, 
            UserLoginInfo login, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            var store = this.Db.Set<UserLogin>();

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

                await this.Db.SaveChangesAsync(cancellationToken);
            }
        }
        */

        public virtual async Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            if(this.AutoSave)
            {
                await this.Db.SaveChangesAsync(cancellationToken);
            }

            return;
        }

        public virtual async Task<IdentityResult> CreateAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if(user == null)
                throw new NullReferenceException(nameof(user));

            this.Store.Add(user);

            if(!string.IsNullOrWhiteSpace(user.Pseudonym))
            {
                if(user.DisplayName == null)
                    user.DisplayName = user.Pseudonym;

                user.Pseudonym = user.Pseudonym.ToLowerInvariant();
            }

            await this.SaveChanges(cancellationToken);

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

            this.Store.Remove(user);

            await this.Db.SaveChangesAsync(cancellationToken);

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

            if(string.IsNullOrWhiteSpace(user.DisplayName))
            {
                user.DisplayName = user.Pseudonym;
            }

            user.Pseudonym = user.Pseudonym.ToLowerInvariant();

            this.Store.Attach(user);
            //user.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Store.Update(user);

            
            await this.Db.SaveChangesAsync(cancellationToken);

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
           
            return await this.Store.SingleOrDefaultAsync(o => o.Id == userId);
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

            return await this.Store.SingleOrDefaultAsync(o => o.Pseudonym == normalizedUserName);
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if(this.isDisposed)
                return;
            
            if(disposing)
            {
                this.Db = null;
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
