using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;
using NerdyClaimTypes = NerdyMishka.EfCore.Identity.ClaimTypes;
using ClaimTypes = System.Security.Claims.ClaimTypes;
using System.Linq;

namespace NerdyMishka.Identity
{

   
    public abstract partial class UserStoreBase<TUser, TUserClaim, TUserLogin, TUserToken> :
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>
    {
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

            var store = this.Db.Set<UserClaim>();
            var set = await this.UserClaims
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            foreach(var claim in claims)
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
                 
        }

        public abstract Task AddToRoleAsync(
            TUser user, 
            string roleName, 
            CancellationToken cancellationToken = default(CancellationToken));

      

        public async virtual Task<IList<Claim>> GetClaimsAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
           

            var userClaims = await (from o in this.Users
                    join uc in this.UserClaims on o.Id equals uc.UserId
                    where o.Id == user.Id
                    select uc).ToListAsync();

            var list = new List<Claim>();

            foreach(var c in userClaims)
                list.Add(c.ToClaim());

           
            return list;
        }


        public abstract Task<IList<string>> GetRolesAsync(
            TUser user, 
            CancellationToken cancellationToken = default(CancellationToken));

        public async virtual Task<IList<TUser>> GetUsersForClaimAsync(
            Claim claim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(claim == null)
                throw new ArgumentNullException(nameof(claim));

            switch(claim.Type)
            {
                case NerdyClaimTypes.Role:
                case ClaimTypes.Role:
                    return await (from o in this.Db.Set<TUser>()
                      join c in this.Db.Set<UserRole>() on o.Id equals c.UserId
                      join r in this.Db.Set<Role>() on c.RoleId equals r.Id
                      where r.Name == claim.Value || r.Code == claim.Value
                      select o).ToListAsync();
               

                case NerdyClaimTypes.Permission:
                     return await (from o in this.Db.Set<TUser>()
                      join c in this.Db.Set<UserRole>() on o.Id equals c.UserId
                      join r in this.Db.Set<Role>() on c.RoleId equals r.Id
                      join rp in this.Db.Set<RolePermission>() on r.Id equals rp.RoleId
                      join p in this.Db.Set<Permission>() on rp.PermissionId equals p.Id
                      where p.Name == claim.Value || p.Code == claim.Value
                      select o).ToListAsync();

                default:
                    return await (from o in this.Db.Set<TUser>()
                      join c in this.Db.Set<UserClaim>() on o.Id equals c.UserId
                      where c.Type == claim.Type && c.Value == claim.Value
                      select o).ToListAsync();
                
            }
        }

        public virtual async Task<IList<TUser>> GetUsersInRoleAsync(
            string roleName, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(roleName == null)
                throw new ArgumentNullException(nameof(roleName));

            return await (from o in this.Db.Set<TUser>()
                      join c in this.Db.Set<UserRole>() on o.Id equals c.UserId
                      join r in this.Db.Set<Role>() on c.RoleId equals r.Id
                      where r.Name == roleName || r.Code == roleName
                      select o).ToListAsync();
        }

       

        public abstract Task<bool> IsInRoleAsync(
            TUser user, 
            string roleName, 
            CancellationToken cancellationToken = default(CancellationToken));

        public virtual async Task RemoveClaimsAsync(
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
                    case ClaimTypes.Role:
                        await this.RemoveFromRoleAsync(user, claim.Value, cancellationToken);
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

                var remove = new List<UserClaim>();
                foreach(var item in set)
                {
                    foreach(var c in custom)
                    {
                        if(item.Type == c.Type && item.Value == c.Value)
                            remove.Add(item);
                    }
                }

                store.RemoveRange(remove);
            }            
        }

        public abstract Task RemoveFromRoleAsync(
            TUser user, 
            string roleName,
            CancellationToken cancellationToken = default(CancellationToken));

        public virtual async Task ReplaceClaimAsync(
            TUser user, 
            Claim claim, 
            Claim newClaim, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            if(claim == null)
                throw new ArgumentNullException(nameof(claim));

            if(newClaim == null)
                throw new ArgumentNullException(nameof(newClaim));

    
            var current = await (from uc in this.Db.Set<UserClaim>()
                    where uc.UserId == user.Id && uc.Type == claim.Type &&
                    uc.Value == claim.Value
                    select uc).SingleOrDefaultAsync();

            if(current == null)
                return;

            current.Type = newClaim.Type;
            current.Value = newClaim.Value;
        }
    }

}