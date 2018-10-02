using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Data;
using Nexus.Services;
using NerdyMishka.Security.Cryptography;
using Nexus.Api;

namespace NerdyMishka.Nexus.Services
{
    public class UserServiceTests
    {
        
        [Fact]
        public async static void Crud()
        {
            var services = Env.CreateSqlServerEnv("UserServiceTests");
            var db = services.GetService<NexusDbContext>();
            Assert.NotNull(db);
            try {
                var userService = new UserService(db, new PasswordAuthenticator());
                var registration = new UserRegistration() {
                    Name = "iggy@pop.com",
                    DisplayName = "Iggy",
                    IsAdmin = true,
                    IconUri = "test",
                    GenerateApiKey = true,
                    GeneratePassword = true
                };
                var response =  await userService.RegisterAsync(registration);

                Assert.NotNull(response);
                Assert.NotNull(response.Password);
                Assert.NotNull(response.ApiKey);
                Assert.Equal("iggy@pop.com", response.Name);
                Assert.True(response.IsAdmin);

                var pw = response.Password.ToBytes();
                var apiKey = response.ApiKey.ToBytes();

                var (user, verified) = await userService.VerifyAsync(response.Name, pw);
                Assert.NotNull(user);
                Assert.True(verified);
              
                var user2 = await userService.FindOneAsync("iggy@pop.com");

                Assert.Equal(user.Id, user2.Id); 
                Assert.Equal(user.ResourceId, user2.ResourceId);
                Assert.Equal(user.Name, user2.Name);
                Assert.Equal(user.DisplayName, user2.DisplayName);
                Assert.Equal(user.IconUri, user2.IconUri);

                var (user3, verified3) = await userService.VerifyApiKeyAsync(response.Name, apiKey);

                Assert.NotNull(user3);
                Assert.True(verified3);
                Assert.Equal(user.Id, user3.Id); 
                Assert.Equal(user.ResourceId, user3.ResourceId);
                Assert.Equal(user.Name, user3.Name);
                Assert.Equal(user.DisplayName, user3.DisplayName);
                Assert.Equal(user.IconUri, user3.IconUri);
            } finally {
                Env.CleanupSqlServerEnv("UserServiceTests");
            }
        }


    }
}