using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NerdyMishka.Identity
{
    public class UserManager<TUser> : Microsoft.AspNetCore.Identity.UserManager<TUser>
        where TUser: class 
    {
        public UserManager(
            IUserStore<TUser> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<TUser> passwordHasher, 
            IEnumerable<IUserValidator<TUser>> userValidators, 
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<Microsoft.AspNetCore.Identity.UserManager<TUser>> logger) : base(
                store, optionsAccessor, passwordHasher, userValidators, 
                passwordValidators,  keyNormalizer, errors, services, logger)
        {

        }

        public bool SupportsPermissionClaims => this.Store is IUserPermissionStore<TUser>;


        public virtual async Task<IList<Claim>> GetPermissionClaimsAsync(TUser user)
        {
            if(!this.SupportsPermissionClaims)
                return Array.Empty<Claim>();

            return await ((IUserPermissionStore<TUser>)this.Store)
                .GetPermissionClaimsAsync(user, this.CancellationToken);
        }
    }
}