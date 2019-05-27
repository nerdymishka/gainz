using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka.EfCore;
using NerdyMishka.EfCore.Metadata;

namespace NerdyMishka.EfCore.Identity
{

    public class IdentityEfCoreConfiguration : 
        IEntityTypeConfiguration<User>, 
        IEntityTypeConfiguration<PasswordLogin>
    {
        private string schemaName;


        public IdentityEfCoreConfiguration(string schemaName = "identity")
        {
            this.schemaName = schemaName;
        }

        public virtual void ApplyConfiguration(ModelBuilder builder)
        {
            builder.ApplyConfiguration<User>(this);
            builder.ApplyConfiguration<PasswordLogin>(this);
        }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {
            builder.SetSchema(this.schemaName);
            builder.HasKey(o => o.Id);
            
            builder.HasDependant(
                o => o.PasswordLogin, 
                o => o.User, 
                o => o.UserId);
        }

        public virtual void Configure(EntityTypeBuilder<PasswordLogin> builder)
        {
            builder.SetSchema(this.schemaName);
            builder.HasKey(o => o.UserId);

            builder.Property(o => o.UserId)
                .ValueGeneratedNever();
        }

       
    }
}