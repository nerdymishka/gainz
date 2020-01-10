using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;

namespace NerdyMishka.Agent
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            IHost host = null;

            try {
                host =  Host.CreateDefaultBuilder(args)
                    
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<Worker>();
                    })
                    .ConfigureHostConfiguration((cb) => {   
                        cb.SetBasePath(Directory.GetCurrentDirectory());
                        cb.AddEnvironmentVariables("NERDY_");
                        cb.AddJsonFile("agent.json", optional: true);
                    })
                    .Build();

                await host.WaitForShutdownAsync();
                return 0;
                
            } catch(Exception ex) {
                var logger = host?.Services?.GetService<ILogger>();
                if(logger == null)
                    await Console.Error.WriteLineAsync($"Agent failure: {ex.Message} \n {ex.StackTrace}");
                else 
                    logger?.LogError(ex, "Agent failure");

                return 1;
            }
        }
           
    }
}
