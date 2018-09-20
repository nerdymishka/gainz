using NerdyMishka;
using Nexus.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Api;
using System.Threading.Tasks;

namespace Nexus.Data.Tests
{
    /* 
    public class AdminResourceServiceTests
    {

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Db", "SqlServer")]
        public async void AdminResourcesApi_SqlServer()
        {
            if(Env.IsLocalDb && !Env.IsWindows)
                return;

           
            var provider = Env.CreateSqlServerEnv("AdminResources");
            //await TestAsync(provider).ConfigureAwait(false);
            Env.CleanupSqlServerEnv("AdminResources");
        }

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Db", "Sqlite")]
        public async void AdminResourcesApi_Sqlite()
        {
            var provider = Env.CreateSqliteEnv("AdminResources");
            //await TestAsync(provider).ConfigureAwait(false);
            Env.CleanupSqliteEnv("AdminResources");
        }

        private static async Task TestAsync(ServiceProvider provider)
        {
            var adminService = provider.GetRequiredService<IAdminResourceService>();

            Assert.NotNull(adminService);
            var findResponse = await adminService.FindAsync(1L)
                .ConfigureAwait(false);

            Assert.NotNull(findResponse);
            Assert.True(findResponse.Ok);

            var resource = findResponse.Result;
            Assert.NotNull(resource);
            Assert.Equal(1L, resource.Id.Value);
            Assert.Equal("/users", resource.Uri);
            Assert.Null(resource.Key);
            Assert.Equal("users", resource.Type);

            var resource2 = new ResourceModel(){
                Uri = "/groups",
                Type = "groups"
            };
            
            var response2 = await adminService.SaveAsync(new [] { resource2 });
            Assert.NotNull(findResponse);
            Assert.True(response2.Ok);

            var listResponse = await adminService.ListAsync();
            Assert.NotNull(listResponse);
            Assert.True(listResponse.Ok);
            Assert.True(listResponse.Hits.HasValue);
            Assert.Equal(2, listResponse.Hits.Value);
            Assert.Collection(listResponse.Results, 
                first => Assert.Equal(1, first.Id),
                second => Assert.Equal(2, second.Id));

            var deleteResponse = await adminService.DeleteAsync(new []{ 2L });

            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.Ok);

            listResponse = await adminService.ListAsync();
            Assert.NotNull(listResponse);
            Assert.Equal(1, listResponse.Hits.Value);
        } 
    }*/
}