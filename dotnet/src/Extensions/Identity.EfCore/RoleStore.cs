
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

    public class RoleStore : 
        RoleStore<Role, Permission, IdentityDbContext>
    {
        public RoleStore(
            IdentityDbContext dbContext) :base(dbContext)
        {

        }
    }

    public class RoleStore<TRole, TPermission, TContext> :
        RoleStoreBase<TRole, UserRole, RoleClaim, TPermission, RolePermission> 
        where TRole : Role, new()
        where TPermission : Permission, new()
        where TContext : DbContext
    {

         public RoleStore(
            TContext dbContext) :base()
        {
            this.Db = (DbContext)dbContext;
        }
    }
 


    public abstract class RoleStoreBase<TRole, TUserRole, TRoleClaim, TPermission, TRolePermission> :
        RoleStoreBase<TRole, TUserRole, TRoleClaim>,
        IQueryablePermissionStore<TPermission>,
        IPermissionStore<TPermission>,
        IRolePermissionStore<TRole>
        where TRole : Role, new()
        where TUserRole : UserRole, new()
        where TRoleClaim : RoleClaim, new()
        where TPermission : Permission, new()
        where TRolePermission : RolePermission, new()
       
    {
        public IQueryable<TPermission> Permissions => this.Db.Set<TPermission>();

        protected DbSet<TRolePermission> RolePermissions => this.Db.Set<TRolePermission>();

        public virtual async Task<TRolePermission> FindRolePermissionAsync(int roleId, string permissionName)
        {
            return await (from rp in this.RolePermissions
                join p in this.Permissions on rp.PermissionId equals p.Id
                where rp.RoleId == roleId && p.Name == permissionName || p.Code == permissionName
                select rp).SingleOrDefaultAsync();
        }

        public virtual async Task<TPermission> FindPermissionAsync(
            string permissionName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await (from o in this.Permissions 
            where o.Code == permissionName || o.Name == permissionName
            select o).SingleOrDefaultAsync(cancellationToken);
        }

        protected virtual async Task<IList<TPermission>> FindPermissonsByNameAsync(
            IEnumerable<string> permissionNames,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return await (from o in this.Permissions 
            where permissionNames.Contains(o.Code)
            select o).ToListAsync(cancellationToken);
        }

        public virtual async Task<IdentityResult> AddToRoleAsync(
            TRole role, 
            string permissionName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var rp = await this.FindRolePermissionAsync(role.Id, permissionName);
            if(rp == null)
            {
                var p = await this.FindPermissionAsync(permissionName, cancellationToken);
                if(p == null)
                    return IdentityResult.Failed(new IdentityError(){ 
                        Description = $"Permission does not exist {permissionName}" });

                this.RolePermissions.Add(new TRolePermission(){
                    RoleId = role.Id,
                    PermissionId = p.Id
                });
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> AddToRoleAsync(
            TRole role, 
            IEnumerable<string> permissionNames, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var permissions = await this.FindPermissonsByNameAsync(permissionNames, cancellationToken);
            if(permissions.Count == 0)
                return IdentityResult.Failed(new IdentityError() { Description = "None of the permissions listed were found"});

            var existing = await this.GetPermissionsAsync(role, cancellationToken);
            var permissionsToAdd = permissions.Except(
                    permissions.Where(o => existing.Contains(o.Code)));
        
            
            foreach(var p in permissionsToAdd)
            {
                this.RolePermissions.Add(new TRolePermission(){
                    RoleId = role.Id,
                    PermissionId = p.Id 
                });    
            }
                
                

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> CreateAsync(
            TPermission permission,  
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.Db.Add(permission);
            await this.SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(
            TPermission permission, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            this.Db.Remove(permission);
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

        public Task<string> GetNormalizedPermissionNameAsync(
            TPermission permission, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return Task.FromResult(permission.Code);
        }

        public Task<string> GetPermissionIdAsync(TPermission permission, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return Task.FromResult(permission.Id.ToString());
        }

        public virtual async Task<IList<string>> GetPermissionsAsync(TRole role, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            
            return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                where rp.RoleId == role.Id
                select p.Code).ToListAsync();
        }

        public virtual async Task<IList<string>> GetPermissionsAsync(
            string[] roles, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
             return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                join r in this.Roles on rp.RoleId equals r.Id
                where roles.Contains(r.Code)
                select p.Code).ToListAsync();
        }

        public virtual async Task<IList<string>> GetPermissionsByIdsAsync(
            string[] roleIds,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ids = roleIds.Cast<int>();
            return await (from p in this.Permissions
                join rp in this.RolePermissions on p.Id equals rp.PermissionId
                where ids.Contains(rp.RoleId)
                select p.Code).ToListAsync();
        }

        public Task<string> GetPermissionNameAsync(
            TPermission permission, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            
            return Task.FromResult(permission.Name);
        }

        public virtual async Task<IdentityResult> RemoveFromRoleAsync(
            TRole role, 
            string permissionName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
             var permissionToRemove = await (from rp in this.RolePermissions
                join p in this.Roles on rp.PermissionId equals p.Id
                where rp.RoleId == role.Id && permissionName == p.Code
                select rp).SingleOrDefaultAsync();

            if(permissionToRemove != null)
                this.RolePermissions.Remove(permissionToRemove);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> RemoveFromRoleAsync(
            TRole role, 
            IEnumerable<string> permissionNames,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var permissionsToRemove = await (from rp in this.RolePermissions
                join p in this.Roles on rp.PermissionId equals p.Id
                where rp.RoleId == role.Id && permissionNames.Contains(p.Code)
                select rp).ToListAsync();
            
            this.RolePermissions.RemoveRange(permissionsToRemove);

            return IdentityResult.Success;
        }

        public Task SetNormalizedPermissionNameAsync(
            TPermission permission, 
            string normalizedName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();

            permission.Code = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPermissionNameAsync(
            TPermission permission, 
            string permissionName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.ThrowIfDisposed();

            permission.Name = permissionName;
            return Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> UpdateAsync(
            TPermission permission, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (permission == null)
            {
                throw new ArgumentNullException(nameof(permission));
            }
            this.Db.Attach(permission);
            //role.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Db.Update(permission);
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

        public Task<TPermission> FindPermissionByIdAsync(
            string permissionId, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var id = int.Parse(permissionId);
            
            return this.Permissions.SingleOrDefaultAsync(
                o => o.Id == id, cancellationToken);
        }

        public Task<TPermission> FindPermissionByNameAsync(
            string normalizedRoleName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.Permissions.SingleOrDefaultAsync(
                o => o.Code == normalizedRoleName, cancellationToken);
        }
    }

    public abstract class RoleStoreBase<TRole, TUserRole, TRoleClaim> :
        IQueryableRoleStore<TRole>,
        IRoleClaimStore<TRole>
        where TRole : Role, new()
        where TUserRole : UserRole, new()
        where TRoleClaim: RoleClaim, new() 
    {
        private bool isDisposed = false;
        public IQueryable<TRole> Roles => this.Db.Set<TRole>().AsQueryable();

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public DbContext Db { get; set; }

        public bool AutoSave { get; set; }

        public bool SupportsPermissions  { get; set; }

        protected DbSet<TRoleClaim> RoleClaims => this.Db.Set<TRoleClaim>();

        protected DbSet<TUserRole> UserRoles => this.Db.Set<TUserRole>();




        protected virtual async Task SaveChanges(CancellationToken cancellationToken = default(CancellationToken))
        {
            if(this.AutoSave)
            {
                await this.Db.SaveChangesAsync(cancellationToken);
            }
        }

        public Task AddClaimAsync(
            TRole role, 
            Claim claim,
            CancellationToken cancellationToken = default(CancellationToken))
        {
           this.ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            RoleClaims.Add(CreateRoleClaim(role, claim));
            return Task.FromResult(false);
        }

        public virtual async Task<IdentityResult> CreateAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            this.Db.Add(role);
            await this.SaveChanges(cancellationToken);
            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(
            TRole role, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            this.Db.Remove(role);
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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<TRole> FindByIdAsync(
            string roleId, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
           cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            int id = int.Parse(roleId);
            return Roles.FirstOrDefaultAsync(u => u.Id.Equals(roleId), cancellationToken);
        }

        public Task<TRole> FindByNameAsync(
            string normalizedRoleName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Roles.FirstOrDefaultAsync(r => r.Code == normalizedRoleName, cancellationToken);
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(
            TRole role,  
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            return await RoleClaims
                .Where(rc => rc.RoleId.Equals(role.Id))
                .Select(c => new Claim(c.Type, c.Value))
                .ToListAsync(cancellationToken);
        }

        public Task<string> GetNormalizedRoleNameAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Code);
        }

        public Task<string> GetRoleIdAsync(TRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(
            TRole role,  
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return Task.FromResult(role.Name);
        }

        public virtual async Task RemoveClaimAsync(
            TRole role, 
            Claim claim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
             ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            var claims = await RoleClaims
                .Where(
                    rc => rc.RoleId.Equals(role.Id) && 
                    rc.Value == claim.Value && 
                    rc.Type == claim.Type)
                .ToListAsync(cancellationToken);

            foreach (var c in claims)
            {
                RoleClaims.Remove(c);
            }
        }

        public Task SetNormalizedRoleNameAsync(
            TRole role, 
            string normalizedName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Code = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(
            TRole role, 
            string roleName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public virtual async Task<IdentityResult> UpdateAsync(
            TRole role,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            this.Db.Attach(role);
            //role.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Db.Update(role);
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

        /// <summary>
        /// Creates an entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual TRoleClaim CreateRoleClaim(TRole role, Claim claim)
            => new TRoleClaim { RoleId = role.Id, Type = claim.Type, Value = claim.Value };

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

        ~RoleStoreBase() {
            this.Dispose(false);
        }
    }
}