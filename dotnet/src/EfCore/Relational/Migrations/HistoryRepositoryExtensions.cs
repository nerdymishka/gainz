

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
namespace NerdyMishka.EfCore.Migrations
{
    public static class HistoryRespositoryExtensions
    {
        public static void Apply(this EntityTypeBuilder<HistoryRow> history, string tableName, string schemeName = null)
        {
            history.Property(o => o.MigrationId).HasColumnName("id");
            history.Property(o => o.ProductVersion).HasColumnName("product_version");
            history.ToTable(tableName, schemeName);
        }

        
    }
}