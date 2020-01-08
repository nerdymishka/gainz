

using System;
using Microsoft.Extensions.Hosting;
using NerdyMishka.Extensions.Hosting.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Options;

namespace NerdyMishka.Extensions.Hosting.Console
{
    public static class HostBuilderExtensions
    {

        public static IConsoleHost AsConsoleHost(this IHost host)
        {
            return host as IConsoleHost;
        }

        public static IHostBuilder UseConsoleProgram(
            this IHostBuilder builder, 
            string[] args, 
            Func<IHostContext, int> execute, 
            ConsoleHostOptions options = null)
        {
            builder.ConfigureLogging(logging => {
                logging.AddConsole();
            });
        
            builder.ConfigureServices((c, services) => {
                var hosts = services.Where(o => o.ServiceType == typeof(IHost)).ToList();
                if(hosts.Count > 0) {
                    foreach(var host in hosts) 
                        services.Remove(host);
                }

                var hostLifetime = services.Where(o => o.ServiceType == typeof(IHostLifetime)).ToList();
                if(hostLifetime.Count == 0)
                {
                    foreach(var lt in hostLifetime)
                        services.Remove(lt);
                }

                services.AddSingleton<IOptions<ConsoleHostOptions>>((s) => Options.Create<ConsoleHostOptions>(options));
                services.AddSingleton<IConsoleArguments, ConsoleArguments>((s) => new ConsoleArguments(args));
                services.AddSingleton<IHostLifetime, ConsoleLifetime>();
                services.AddSingleton<IHost, ConsoleHost>();
                services.AddSingleton<IConsoleHost, ConsoleHost>();
                services.AddSingleton<IConsoleProgram, DelegateConsoleProgram>((s) => {
                    return new DelegateConsoleProgram(execute);
                });
                
                services.AddHostedService<ConsoleHostedService>();
            });

            return builder;
        }


        public static IHostBuilder UseConsoleProgram(
            this IHostBuilder builder, 
            Func<IHostContext, int> execute,
            ConsoleHostOptions options = null)
        {
            builder.ConfigureLogging(logging => {
                logging.AddConsole();
            });
        
            builder.ConfigureServices((c, services) => {
                var hosts = services.Where(o => o.ServiceType == typeof(IHost)).ToList();
                if(hosts.Count > 0) {
                    foreach(var host in hosts) 
                        services.Remove(host);
                }

                var hostLifetime = services.Where(o => o.ServiceType == typeof(IHostLifetime)).ToList();
                if(hostLifetime.Count == 0)
                {
                    foreach(var lt in hostLifetime)
                        services.Remove(lt);
                }

                services.AddSingleton<IOptions<ConsoleHostOptions>>(
                    (s) => Options.Create<ConsoleHostOptions>(options ?? new ConsoleHostOptions()));
                services.AddSingleton<IConsoleArguments, ConsoleArguments>();
                services.AddSingleton<IHostLifetime, ConsoleLifetime>();
                services.AddSingleton<IHost, ConsoleHost>();
                services.AddSingleton<IConsoleHost, ConsoleHost>();
                services.AddSingleton<IConsoleProgram, DelegateConsoleProgram>((s) => {
                    return new DelegateConsoleProgram(execute);
                });
                
                services.AddHostedService<ConsoleHostedService>();
            });

            return builder;
        }


        public static IHostBuilder UseConsoleProgram<T>(this IHostBuilder builder, 
            string[] arguments, 
            ConsoleHostOptions options = null) where T: IConsoleProgram
        {
            builder.ConfigureLogging(logging => {
                logging.AddConsole();
            });
        
            builder.ConfigureServices((c, services) => {
                var hosts = services.Where(o => o.ServiceType == typeof(IHost)).ToList();
                if(hosts.Count > 0) {
                    foreach(var host in hosts) 
                        services.Remove(host);
                }

                var hostLifetime = services.Where(o => o.ServiceType == typeof(IHostLifetime)).ToList();
                if(hostLifetime.Count == 0)
                {
                    foreach(var lt in hostLifetime)
                        services.Remove(lt);
                }

                services.AddSingleton<IOptions<ConsoleHostOptions>>(
                    (s) => Options.Create<ConsoleHostOptions>(options ?? new ConsoleHostOptions()));

                services.AddSingleton<IConsoleArguments, ConsoleArguments>((s) => new ConsoleArguments(arguments));
                services.AddSingleton<IHostLifetime, ConsoleLifetime>();
                services.AddSingleton<IHost, ConsoleHost>();
                services.AddSingleton<IConsoleHost, ConsoleHost>();
                services.AddSingleton(typeof(IConsoleProgram), typeof(T));
                services.AddHostedService<ConsoleHostedService>();
            });

            return builder;
        }


        public static IHostBuilder UseConsoleProgram<T>(this IHostBuilder builder, ConsoleHostOptions options = null) where T: IConsoleProgram
        {
            builder.ConfigureLogging(logging => {
                logging.AddConsole();
            });
        
            builder.ConfigureServices((c, services) => {
                var hosts = services.Where(o => o.ServiceType == typeof(IHost)).ToList();
                if(hosts.Count > 0) {
                    foreach(var host in hosts) 
                        services.Remove(host);
                }

                var hostLifetime = services.Where(
                    o => o.ServiceType == typeof(IHostLifetime))
                    .ToList();

                if(hostLifetime.Count == 0)
                {
                    foreach(var lt in hostLifetime)
                        services.Remove(lt);
                }

                services.AddSingleton<IOptions<ConsoleHostOptions>>(
                    (s) => Options.Create<ConsoleHostOptions>(options ?? new ConsoleHostOptions()));

                services.AddSingleton<IConsoleArguments, ConsoleArguments>();
                services.AddSingleton<IHostLifetime, ConsoleLifetime>();
                services.AddSingleton<IHost, ConsoleHost>();
                services.AddSingleton<IConsoleHost, ConsoleHost>();
                services.AddSingleton(typeof(IConsoleProgram), typeof(T));
                services.AddHostedService<ConsoleHostedService>();
            });

            return builder;
        }
    }
}