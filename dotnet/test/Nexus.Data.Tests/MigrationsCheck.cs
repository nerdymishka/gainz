using System;
using System.IO;
using NerdyMishka.Data;
using NerdyMishka.FluentMigrator;
using Xunit;

namespace NerdyMishka.Nexus.Data.Tests
{
    public class MigrationsCheck
    {
        private string localDbString = "Data Source=(LocalDB)\\MSSQLLocalDB;Integrated Security=True";

        [Fact]
        public void SqlServer()
        {
            if(Constants.IsWindows)
            {
                 bool dbCreated  = false;
                try {
                   
                    using(var connection = new DataConnection(KnownProviders.SqlServer, localDbString))
                    {
                        var dir = Env.ResolvePath("~/Data");
                        if(!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);

                      
                        //connection.Execute("ALTER DATABASE nexus_migration_check SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" );
                        //connection.Execute("DROP DATABASE nexus_migration_check");
                          

                        connection.Execute(
                            $@" CREATE DATABASE
                [nexus_migration_check]
            ON PRIMARY (
            NAME=Fmg,
            FILENAME = '{dir}\nexus_migration_check.mdf'
            )
            LOG ON (
                NAME=Fmg_log,
                FILENAME = '{dir}\nexus_migration_check.ldf'
            )"
                        );
                        
                        dbCreated = true;
                    }

                
                    var assembly = typeof(NerdyMishka.Nexus.Migrations.M2018090200_InitialMigration).Assembly;
                    var cs = $"{localDbString};Database=nexus_migration_check";
                    ConsoleRunner.DefaultAssembly = assembly;
                    ConsoleRunner.MigrateTo(connectionString: cs, provider: "sqlserver");
                    ConsoleRunner.ListMigrations(connectionString: cs, provider: "sqlserver");

                    Assert.True(true);
                } finally {
                    if(dbCreated) {
                         
                         using(var connection = new DataConnection(KnownProviders.SqlServer, localDbString))
                         {
                             connection.Execute("ALTER DATABASE nexus_migration_check SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" );
                             connection.Execute("DROP DATABASE nexus_migration_check");
                         }  
                         
                    }
                }
            }
        }

        [Fact]
        public void Sqlite()
        {
            var dir = Env.ResolvePath("~/Data");
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var db = $"{dir}\\nexus_sqlite_migrations.db";

            var cs  = $"Data Source={db}";

            

            try {
                var dataConnection = new DataConnection(KnownProviders.SqliteCore, cs);
                dataConnection.Open();
                dataConnection.Close();
                var assembly = typeof(NerdyMishka.Nexus.Migrations.M2018090200_InitialMigration).Assembly;
                ConsoleRunner.DefaultAssembly = assembly;
                ConsoleRunner.MigrateTo(connectionString: cs, provider: "sqlite");
                ConsoleRunner.ListMigrations(connectionString: cs, provider: "sqlite");
                Assert.True(File.Exists(db));
            } finally {
                if(File.Exists(db)) 
                    File.Delete(db);
            }
        }
    }
}
