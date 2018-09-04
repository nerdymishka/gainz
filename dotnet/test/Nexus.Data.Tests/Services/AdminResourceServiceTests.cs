using NerdyMishka;
using Nexus.Services;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Nexus.Api;

namespace Nexus.Data.Tests
{
    public class AdminResourceServiceTests
    {

        [Fact]
        [Trait("Category", "Integration")]
        [Trait("Db", "SqlServer")]
        public async void AdminResourcesApi_SqlServer()
        {
            if(!Env.IsWindows)
                return;

            var provider = Env.CreateSqlServerEnv("AdminResources");
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
            Assert.Equal(null, resource.Key);
            Assert.Equal("users", resource.Type);
        }
    }
}