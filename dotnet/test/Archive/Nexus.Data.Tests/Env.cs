using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using NerdyMishka;
using Nexus.Data;
using System.IO;
using NerdyMishka.Data;
using NerdyMishka.FluentMigrator;
using Nexus.Api;
using Nexus.Services;
using Serilog;

namespace NerdyMishka
{
    public static class Env
    {
        public static readonly bool IsWindows = System.Environment.GetEnvironmentVariable("OS") == "Windows_NT";

        public static readonly bool IsLocalDb = true;

        private static IApplicationEnvironment appEnv;
        
        static Env()
        {
            appEnv = new ApplicationEnvironment(typeof(Env), 3);
            appEnv.EnvironmentName = "Test";
            appEnv.ApplicationName = "NerdyMishka.Nexus.Data.Tests";
            ConsoleRunner.DefaultSchema = "nexus";

             Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(Env.ResolvePath("~/Data/logs.txt"))
                .CreateLogger();
        }

        public static string ResolvePath(string path) => appEnv.ResolvePath(path);


        public static ServiceCollection CreateServices()
        {
            var services = new ServiceCollection();

            return services;
        }

        public static ServiceProvider CreateSqliteEnv(string testName)
        {
            var dir = Env.ResolvePath("~/Data");
            var db = Env.ResolvePath($"~/Data/{testName}.db");
            var cs = $"Data Source={db}";

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if(File.Exists(db))
                File.Delete(db);

            using(var connection = new DataConnection(KnownProviders.SqliteCore, cs))
            {
                connection.Open();
            } 

            var assembly = typeof(NerdyMishka.Nexus.Migrations.M2018090200_InitialMigration).Assembly;
            ConsoleRunner.DefaultAssembly = assembly;
            ConsoleRunner.MigrateTo(connectionString: cs, provider: "sqlite");
            ConsoleRunner.SeedData("Test:Integration", cs, "sqlite");

            var services = new ServiceCollection();
            services.AddLogging(o => o.AddSerilog(dispose: true));
            services.AddEntityFrameworkSqlite();
            
            services.AddDbContext<NexusDbContext>((provider, o) => {
                o.UseSqlite(cs)
                    .UseInternalServiceProvider(provider);
            });

          

            AddServices(services);

            return services.BuildServiceProvider();
        }

        public static void CleanupSqliteEnv(string testName)
        {
            var dir = Env.ResolvePath("~/Data");
            var db = Env.ResolvePath($"~/Data/{testName}.db");

            if(File.Exists(db))
                File.Delete(db);
        }

        public static void CleanupSqlServerEnv(string testName)
        {
            var masterCs = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True";
            var cs = $"Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;Database={testName}";
            var dir = Env.ResolvePath("~/Data");
            var db = Env.ResolvePath($"~/Data/{testName}.mdf");
             if(File.Exists(db))
            {
                using(var connection = new DataConnection(KnownProviders.SqlServer, masterCs))
                {
                    connection.Execute($"ALTER DATABASE {testName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" );
                    connection.Execute($"DROP DATABASE {testName}");
                }
            }
        }

        public static ServiceProvider CreateSqlServerEnv(string testName)
        {
            var masterCs = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True";
            var cs = $"Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True;Database={testName}";
            var dir = Env.ResolvePath("~/Data");
            var db = Env.ResolvePath($"~/Data/{testName}.mdf");

            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if(File.Exists(db))
            {
                using(var connection = new DataConnection(KnownProviders.SqlServer, masterCs))
                {
                    connection.Execute($"ALTER DATABASE {testName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" );
                    connection.Execute($"DROP DATABASE {testName}");
                }
            }

            using(var connection  = new DataConnection(KnownProviders.SqlServer, masterCs))
            {
                connection.Execute($@" CREATE DATABASE
                [{testName}]
            ON PRIMARY (
            NAME={testName},
            FILENAME = '{dir}\{testName}.mdf'
            )
            LOG ON (
                NAME={testName}_log,
                FILENAME = '{dir}\{testName}.ldf'
            )");    

            }

            var assembly = typeof(NerdyMishka.Nexus.Migrations.M2018090200_InitialMigration).Assembly;
            ConsoleRunner.DefaultAssembly = assembly;
            ConsoleRunner.MigrateTo(connectionString: cs, provider: "sqlserver");
            ConsoleRunner.SeedData("Test:Integration", cs, "sqlServer");

            var services = new ServiceCollection();
            services.AddLogging(c => c.AddConsole());
            services.AddEntityFrameworkSqlServer();
            
            services.AddDbContext<NexusDbContext>((provider, o) => {
                o.UseSqlServer(cs)
                    .UseInternalServiceProvider(provider);
            });

            AddServices(services);

            return services.BuildServiceProvider();
        }

        private static void AddServices(IServiceCollection services)
        {
            //services.AddTransient<IAdminResourceService, AdminResourceService>();
        }
    }
}