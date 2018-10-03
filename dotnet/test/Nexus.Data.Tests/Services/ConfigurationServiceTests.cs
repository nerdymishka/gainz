using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Data;
using Nexus.Services;
using NerdyMishka.Security.Cryptography;
using Nexus.Api;
using System.Threading.Tasks;
using System.Linq;

namespace NerdyMishka.Nexus.Services
{
    public class ConfigurationServiceTests
    {
        
        [Fact]
        public async static void Sql_Crud()
        {
            var provider = Env.CreateSqlServerEnv("ConfigurationServiceTests");

            try {
                await RunTestAsync(provider);
            } finally {
                Env.CleanupSqlServerEnv("ConfigurationServiceTests");
            }
        }


        private async static Task RunTestAsync(ServiceProvider provider)
        {
            var db = provider.GetService<NexusDbContext>();
            var key = new CompositeKey();

            var env = await db.CreateOpEnvAsync("Production", "prod");

            var env2 = db.OperationalEnvironments.FirstOrDefault(o => o.Name == "prod" || o.Alias == "prod");
            Assert.NotNull(env2);

            key.AddPassword("my-great-and-terrible-password");
            var configurationService = new ConfigurationService(db, key);

            var set = new ConfigurationSet() {
                Name = "Production Services",
                OperationalEnvironmentName = env.Alias
            };

            var result = await configurationService.SaveAsync(set);

            Assert.NotNull(result);
            Assert.Equal(set.Name, result.Name);
            Assert.Equal(set.OperationalEnvironmentName, result.OperationalEnvironmentName);
            Assert.Equal(env.Id, set.OperationalEnvironmentId);


            var set2 = await configurationService.FindSetByNameAsync("Production Services");

            Assert.NotNull(set2);
            Assert.Equal(set.Name, set2.Name);
            Assert.Equal(set.OperationalEnvironmentName, set2.OperationalEnvironmentName);
            Assert.Equal(env.Id, set2.OperationalEnvironmentId);
        }

    }
}