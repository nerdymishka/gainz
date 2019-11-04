

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NerdyMishka.Extensions.Hosting;
using Xunit;

namespace Tests 
{
    public class StartupManagerTests
    {
        [Fact]
        public void ConventionalStartupClass_StartupServiceFilters_WrapsConfigureServicesMethod()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            var services = serviceCollection.BuildServiceProvider();
            serviceCollection.TryAddSingleton(new ServiceBefore { Message = "Configure services" });
            serviceCollection.TryAddSingleton(new ServiceAfter { Message = "Configure services" });

            var type = typeof(VoidReturningStartupServicesFiltersStartup);
            var startup = StartupLoader.LoadMethods(services, type, "");

            var applicationServices = startup.ConfigureServicesDelegate(serviceCollection);
            var before = applicationServices.GetRequiredService<ServiceBefore>();
            var after = applicationServices.GetRequiredService<ServiceAfter>();

            Assert.Equal("StartupServicesFilter Before 1", before.Message);
            Assert.Equal("StartupServicesFilter After 1", after.Message);
        }
    }


    public class StartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Action<IApplicationBuilder> wrap = (builder) => {
                builder[""]
                builder.ApplicationServices.Replace()
            }
        }
    }


    public class VoidReturningStartupServicesFiltersStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            
        }

        public void Configure(IApplicationBuilder builder)
        {
        }
    }

    public class ServiceBefore
    {
        public string Message { get; set; }
    }

    public class ServiceAfter
    {
        public string Message { get; set; }
    }
}