using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using NerdyMishka.EfCore.Identity.SqlServer;

namespace NerdyMishka.Migrator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }

    public class MigrationsFactory : IDesignTimeDbContextFactory<SqlMigrationIdentityDbContext>
    {
        public SqlMigrationIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SqlMigrationIdentityDbContext>();
            

            return new SqlMigrationIdentityDbContext(optionsBuilder.Options);
        }
    }
}
