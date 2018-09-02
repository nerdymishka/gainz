using System;
using NerdyMishka.FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using System.Reflection;
using System.Collections.Generic;
using NerdyMishka.FluentMigrator;
using Extensions = NerdyMishka.FluentMigrator.NerdyMishkaMigrationExtensions;

namespace NerdyMishka.FluentMigrator
{
    public class ConsoleRunner
    {

        public static Assembly DefaultAssembly { get; set;}

        public static void ListMigrations(
            string module = null,
            string connectionString = "Data Source=:memory:",
            string provider = "SQLite",
            params Assembly[] assemblies) {

            UseRunner(
                runner => runner.ListMigrations(module),
                connectionString,
                module,
                provider,
                assemblies
            );
        }

        public static void MigrateTo(
            long version = long.MaxValue, 
            string module = null,
            string connectionString = "Data Source=:memory:",
            string provider = "SQLite",
            params Assembly[] assemblies)
        {
            UseRunner(
                runner => runner.MigrateTo(version, module),
                connectionString,
                module,
                provider,
                assemblies
            );
        }

        private static void UseRunner(
            Action<NerdyMishkaMigrationRunner> invoker,
            string connectionString,
            string module = null,
            string provider = "SQLite",
            IList<Assembly> assemblies = null
           )
        {
            var list = new List<Assembly>();
            if(assemblies != null && assemblies.Count > 0)
                list.AddRange(assemblies);

            if((assemblies == null || assemblies.Count == 0) && ConsoleRunner.DefaultAssembly == null)
                throw new NullReferenceException($"DefaultAssembly must be set");
            else 
                list.Add(DefaultAssembly);
                
            var serviceProvider = CreateServices(connectionString, provider, list);

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServiceProvider =  scope.ServiceProvider;
                var runner = (NerdyMishkaMigrationRunner)scopedServiceProvider.GetRequiredService<IMigrationRunner>(); 
                invoker.Invoke(runner);
            }
        }

        private static IServiceProvider CreateServices(
            string connectionString = null,
            string provider = "SQLite",
            IList<Assembly> assemblies = null)
        {
            return new ServiceCollection()
                // Add common FluentMigrator services
                
                .ConfigureNerdyMishkaMigrationRunner(rb => { 

                    provider = provider.ToLowerInvariant();
                    switch(provider)
                    {
                        case "mysql":
                            rb.AddMySql5();
                            break; 
                        case "postgres":
                            rb.AddPostgres();
                            break;
                        case "sqlserver":
                        case "mssql":
                            rb.AddSqlServer2016();
                            break;
                        case "sqlite":
                            Extensions.DefaultSchema = null;
                            rb.AddSQLite();
                            break;
                        default:
                            Extensions.DefaultSchema = null;
                            rb.AddSQLite();
                            break;
                    }

                    rb.WithGlobalConnectionString(connectionString);
            
                    foreach(var assembly in assemblies)
                    {
                        rb.ScanIn(assembly).For.Migrations();   
                    }
                }, true)
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }
    }
}
