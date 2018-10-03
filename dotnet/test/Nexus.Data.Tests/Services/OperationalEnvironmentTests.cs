using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Data;
using Nexus.Services;
using NerdyMishka.Security.Cryptography;
using Nexus.Api;
using System.Threading.Tasks;

namespace NerdyMishka.Nexus.Services
{
    public class OperationalEnvironmentServiceTests
    {
        
        [Fact]
        public async static void Sql_Crud()
        {
            var provider = Env.CreateSqlServerEnv("OperationalEnvironmentServiceTests");

            try {
                await RunTestAsync(provider);
            } finally {
                Env.CleanupSqlServerEnv("OperationalEnvironmentServiceTests");
            }
        }


        private async static Task RunTestAsync(ServiceProvider provider)
        {
            var db = provider.GetService<NexusDbContext>();
          
            var envService = new OperationalEnvironmentService(db);
            var data = new OperationalEnvironment() {
                Name = "Production",
                Alias ="prod",
                Description = "My awesome production env",
                UriPath = "production"
            };

            var result = await envService.SaveAsync(data);
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(data.Name, result.Name);
            Assert.Equal(data.Alias, result.Alias);
            Assert.Equal(data.Description, result.Description);
            Assert.Equal(data.UriPath, result.UriPath);
        }

    }
}