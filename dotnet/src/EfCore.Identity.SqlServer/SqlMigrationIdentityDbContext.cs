using System;
using Microsoft.EntityFrameworkCore;

namespace NerdyMishka.EfCore.Identity.SqlServer
{
    public class SqlMigrationIdentityDbContext : NerdyMishka.EfCore.Identity.IdentityDbContext
    {

        public SqlMigrationIdentityDbContext(DbContextOptions options) : base(options) {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"data source=(localdb)\MSSQLLocalDB;Database=NerdyMishka_SqlServer;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
