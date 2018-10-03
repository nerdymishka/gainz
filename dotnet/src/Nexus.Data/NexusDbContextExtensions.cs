using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public static class NexusDbContextExtensions
    {
        private static readonly ConcurrentDictionary<Type, ResourceKindRecord> resourceKindCache = 
            new ConcurrentDictionary<Type, ResourceKindRecord>();

        public static async Task<UserRecord> FindUserAsync(
            this NexusDbContext dbContext, 
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = name.ToLowerInvariant();
            return await dbContext.Users
                    .SingleOrDefaultAsync(o => o.Name == lowered, cancellationToken)
                    .ConfigureAwait(false);

        }

          public static async Task<OperationalEnvironmentRecord> FindOperationalEnvironmentAsync(
            this NexusDbContext dbContext, 
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = name.ToLowerInvariant();
            return await dbContext.OperationalEnvironments
                    .SingleOrDefaultAsync(o => o.Name == name || o.Alias == lowered, cancellationToken)
                    .ConfigureAwait(false);
        }


         public static async Task<ConfigurationSetRecord> FindConfigurationSetAsync(
            this NexusDbContext dbContext, 
            string name, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return await dbContext.ConfigurationSets
                    .Include(cs => cs.OperationalEnvironment)
                    .SingleOrDefaultAsync(o => o.Name == name, cancellationToken)
                    .ConfigureAwait(false);

        }
    }
}