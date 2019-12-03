using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Data;
using Nexus.Services;
using NerdyMishka.Security.Cryptography;
using Nexus.Api;
using Humanizer;
using System.Threading.Tasks;

namespace NerdyMishka.Nexus.Services
{
    public static class Util
    {
        public async static Task<OperationalEnvironmentRecord>  CreateOpEnvAsync(
            this NexusDbContext db,
            string name, string alias)
        {
            var resource = new ResourceRecord() {
                KindId = 1000,
            };
            await db.Resources.AddAsync(resource);
            var env = new OperationalEnvironmentRecord(){
                Name = name,
                Alias = alias.ToLower(),
                UriPath = name.Hyphenate().ToLower(),
                Description = name,
                Resource = resource
             };
            await db.OperationalEnvironments.AddAsync(env);

            await db.SaveChangesAsync();

            resource.RowId = env.Id;

            await db.SaveChangesAsync();

            return env;
        }
    }
}