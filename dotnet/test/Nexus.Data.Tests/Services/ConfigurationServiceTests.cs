using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Data;
using Nexus.Services;
using NerdyMishka.Security.Cryptography;
using Nexus.Api;
using System.Threading.Tasks;
using System.Linq;
using System;

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

            var file1 = new ConfigurationFile(
                "cloud-services/client.cscfg",
                "this is a test"
            );

            var result1 = await configurationService.SaveAsync(file1);
            Assert.NotNull(result1);
            Assert.NotNull(result1.UriPath);
            Assert.Equal(file1.Base64Content, result1.Base64Content);
            Assert.Equal(file1.Encoding, result1.Encoding);
            Assert.Equal(file1.MimeType, result1.MimeType);
            Assert.Equal(file1.IsEncrypted, result1.IsEncrypted);
            Assert.Equal(true, result1.IsEncrypted);
            Assert.Equal(false, result1.IsKeyExternal);
            Assert.Equal(false, result1.IsTemplate);
            var bytes = Convert.FromBase64String(result1.Base64Content);
            var text  = System.Text.Encoding.UTF8.GetString(bytes);
            Assert.Equal("this is a test", text);

            var cf1 = db.ConfigurationFiles.FirstOrDefault();
            // assert encryption.
            Assert.NotEqual(cf1.Blob, bytes);

            result1.ConfigurationSetName = result.Name;
            result1 = await configurationService.SaveAsync(result1);

            Assert.Equal(result.Name, result1.ConfigurationSetName);
            Assert.Equal(result.Id, result1.ConfigurationSetId);

            var result2 = await configurationService.FindOne("cloud-services/client.cscfg");
            Assert.NotNull(result2);
            Assert.NotNull(result2.UriPath);
            Assert.Equal(result1.Base64Content, result2.Base64Content);
            Assert.Equal(result1.Encoding, result2.Encoding);
            Assert.Equal(result1.MimeType, result2.MimeType);
            Assert.Equal(result1.IsEncrypted, result2.IsEncrypted);
            Assert.Equal(true, result2.IsEncrypted);
            Assert.Equal(false, result2.IsKeyExternal);
            Assert.Equal(false, result2.IsTemplate);
        }

    }
}