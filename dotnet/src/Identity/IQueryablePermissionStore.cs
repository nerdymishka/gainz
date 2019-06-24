
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NerdyMishka.Identity
{
    public interface IQueryablePermissionStore<TPermission>
    {
        IQueryable<TPermission> Permissions { get; }


    }

    public interface IUserPermissionStore<TUser>
    {
        Task<IList<string>> GetPermissionNamesAsync(TUser user, CancellationToken cancellationToken);


        Task<IList<Claim>> GetPermissionClaimsAsync(TUser user, CancellationToken cancellationToken);

        Task<bool> IsInPermissionAsync(TUser user, string permissionName, CancellationToken cancellationToken);
    }

    public interface IRolePermissionStore<TRole>
    {
        Task<IdentityResult> AddToRoleAsync(TRole role, string permissionName, CancellationToken cancellationToken);

        Task<IdentityResult> AddToRoleAsync(TRole role, IEnumerable<string> permissionNames, CancellationToken cancellationToken);


        Task<IdentityResult> RemoveFromRoleAsync(TRole role, string permissionName, CancellationToken cancellationToken);

        Task<IdentityResult> RemoveFromRoleAsync(TRole role, IEnumerable<string> permissionNames, CancellationToken cancellationToken);

        Task<IList<string>> GetPermissionsAsync(TRole role, CancellationToken cancellationToken);

        Task<IList<string>> GetPermissionsAsync(string[] roles, CancellationToken cancellationToken);

        Task<IList<string>> GetPermissionsByIdsAsync(string[] roleIds, CancellationToken cancellationToken);
    }


    public interface IPermissionStore<TPermission> : IDisposable
    {

        Task<IdentityResult> CreateAsync(TPermission role, CancellationToken cancellationToken);

        Task<IdentityResult> DeleteAsync(TPermission role, CancellationToken cancellationToken);

        Task<TPermission> FindPermissionByIdAsync(string permissionId, CancellationToken cancellationToken);
        
        Task<TPermission> FindPermissionByNameAsync(string normalizedRoleName, CancellationToken cancellationToken);
        

        Task<string> GetNormalizedPermissionNameAsync(TPermission role, CancellationToken cancellationToken);
      
        Task<string> GetPermissionIdAsync(TPermission role, CancellationToken cancellationToken);

        Task<string> GetPermissionNameAsync(TPermission role, CancellationToken cancellationToken);

        Task SetNormalizedPermissionNameAsync(TPermission role, string normalizedName, CancellationToken cancellationToken);
       
        Task SetPermissionNameAsync(TPermission role, string roleName, CancellationToken cancellationToken);

        Task<IdentityResult> UpdateAsync(TPermission role, CancellationToken cancellationToken);
        
    }
}