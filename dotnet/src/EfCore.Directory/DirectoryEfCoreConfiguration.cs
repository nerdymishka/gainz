using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka.EfCore;
using NerdyMishka.EfCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace NerdyMishka.EfCore.Directory
{

    public class IdentityEfCoreConfiguration :
        IEntityTypeConfiguration<OperatingSystemDistributionVersion>,
        IEntityTypeConfiguration<DeviceMake>,
        IEntityTypeConfiguration<DeviceModel>


    {

        private string schema;




        public virtual void Configure(EntityTypeBuilder<OperatingSystemDistributionVersion> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrWhiteSpace(this.schema))
                builder.Metadata.Relational().Schema = this.schema;

            builder.Property(o => o.Version).HasMaxLength(50);
            builder.Property(o => o.OsName).HasMaxLength(50);
            builder.Property(o => o.Edition).HasMaxLength(50);
            builder.Property(o => o.DistroName).HasMaxLength(50);
            builder.Property(o => o.Code).HasMaxLength(20);
            builder.HasIndex(o => o.Code);
        }

        public virtual void Configure(EntityTypeBuilder<DeviceMake> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrWhiteSpace(this.schema))
                builder.Metadata.Relational().Schema = this.schema;

            builder.Property(o => o.Id);
            builder.Property(o => o.Name).HasMaxLength(50);
        }

        public virtual void Configure(EntityTypeBuilder<DeviceModel> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrWhiteSpace(this.schema))
                builder.Metadata.Relational().Schema = this.schema;

            builder.Property(o => o.Id);
            builder.Property(o => o.Name).HasMaxLength(100);
            builder.Property(o => o.SupportUri).HasMaxLength(250);
        }
    }
}