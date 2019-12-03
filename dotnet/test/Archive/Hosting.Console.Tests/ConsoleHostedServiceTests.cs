using System;
using Xunit;
using NerdyMishka.Extensions.Hosting.Console;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Mettle;

namespace Tests 
{

    [Trait("tag", "unit")]
    //[Unit]
    public class ConsoleHostServiceTests
    {

        [Fact]
        public static void Constructor()
        {
            var service = CreateService();
            Assert.NotNull(service);
        }

        [Fact]
        public static async void ExecuteService()
        {
            var called = false;
            var service = CreateService((c) => {
                called = true;
                return 0;
            });
           
            Assert.False(called);
            await service.StartAsync().ConfigureAwait(false);
            await Task.Delay(500);
            
            Assert.True(called);
            Assert.Equal(0, service.ExitCode);
            Assert.Null(service.Exception);
        }



        private static ConsoleHostedService CreateService(Func<IHostContext, int> execute = null, Action<IServiceProvider> resolveServices = null)
        {
            if(execute == null)
                execute =  (c) => { Console.WriteLine("Hello"); return 0; };
         
            var servicesList = new ServiceCollection();
            servicesList.AddSingleton<IHostApplicationLifetime, ApplicationLifetime>();
           
            servicesList.AddLogging((o) => o.AddConsole());
            var services = servicesList.BuildServiceProvider();
            var env = new Microsoft.Extensions.Hosting.Internal.HostingEnvironment();
            var lifetime = services.GetRequiredService<IHostApplicationLifetime>();
            var options = services.GetRequiredService<IOptions<ConsoleHostOptions>>();
            var program = new DelegateConsoleProgram(execute);

            var service = new ConsoleHostedService(
                    program,
                    services, 
                    env, 
                    lifetime, 
                    options, new ConsoleArguments(new string[0]));

            if(resolveServices != null)
                resolveServices(services);

            return service;
        }

    }
}
