using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace DankInventory.Data
{
    public class SqlDbContext : DankDbContext
    {

     

        public SqlDbContext([NotNull] DbContextOptions options) : base(options)
        {
           
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            var cs = @"data source=(localdb)\MSSQLLocalDB;Database=DankInventory_SqlServer;"+
                "integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
            optionsBuilder.UseSqlServer(cs);
        }
    }
}