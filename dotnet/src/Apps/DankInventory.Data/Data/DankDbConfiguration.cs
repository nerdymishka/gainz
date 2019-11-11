using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DankInventory.Data
{

    public class DankDbConfiguration : IEntityTypeConfiguration<License>
    {
        private string schema;

        public DankDbConfiguration(string schema = null)
        {
            this.schema = schema;
        }


        public virtual void Apply(ModelBuilder builder)
        {
            this.Configure(builder.Entity<License>());
        }

        public void Configure(EntityTypeBuilder<License> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Uri)
                .HasMaxLength(256);
        }
    }
}