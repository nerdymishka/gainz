﻿using System;
using Microsoft.Extensions.Hosting;
using NerdyMishka.Extensions.Hosting.Console;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleTemplate
{
    class Program
    {
        static int Main(string[] args)
        {
            var builder = new HostBuilder().UseConsoleProgram(args, (ctx) => {
                
             
                log.LogInformation("Hellooooow from logging");
                Console.WriteLine("Hello World!");


                return 0;
            });

            var host = builder.Build().AsConsoleHost();
            host.Run();

            return host.ExitCode;
        }
    }
}
