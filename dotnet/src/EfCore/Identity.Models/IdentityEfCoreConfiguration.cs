using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using NerdyMishka.EfCore;
using NerdyMishka.EfCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NerdyMishka.EfCore.Identity
{

    public class IdentityEfCoreConfiguration : 
        IEntityTypeConfiguration<ApiKey>,
        IEntityTypeConfiguration<ApiKeyRole>,
        IEntityTypeConfiguration<Domain>,
        IEntityTypeConfiguration<Organization>,
        IEntityTypeConfiguration<PasswordPolicy>,
        IEntityTypeConfiguration<MultiFactorPolicy>,
        IEntityTypeConfiguration<EmailAddress>,
        IEntityTypeConfiguration<Phone>,
        IEntityTypeConfiguration<Role>,
        IEntityTypeConfiguration<RoleClaim>,
        IEntityTypeConfiguration<Permission>,
        IEntityTypeConfiguration<RolePermission>,
        IEntityTypeConfiguration<User>, 
        IEntityTypeConfiguration<UserClaim>,
        IEntityTypeConfiguration<UserRole>,
        IEntityTypeConfiguration<UserLogin>,
        IEntityTypeConfiguration<UserToken>,
        IEntityTypeConfiguration<PasswordLogin>
    {
        private string schemaName;


        public IdentityEfCoreConfiguration(string schemaName = "identity")
        {
            this.schemaName = schemaName;
        }

        public virtual void Apply(ModelBuilder builder)
        {
            builder.ApplyConfiguration<User>(this);
            builder.ApplyConfiguration<Permission>(this);
            builder.ApplyConfiguration<Role>(this);
            builder.ApplyConfiguration<Organization>(this);
            builder.ApplyConfiguration<PasswordLogin>(this);
            builder.ApplyConfiguration<PasswordPolicy>(this);
            builder.ApplyConfiguration<ApiKey>(this);
            builder.ApplyConfiguration<Domain>(this);
            builder.ApplyConfiguration<EmailAddress>(this);
            builder.ApplyConfiguration<Phone>(this);
            builder.ApplyConfiguration<ApiKeyRole>(this);
            builder.ApplyConfiguration<RolePermission>(this);
            builder.ApplyConfiguration<UserRole>(this);
            builder.ApplyConfiguration<UserClaim>(this);
            builder.ApplyConfiguration<UserLogin>(this);
            builder.ApplyConfiguration<UserToken>(this);
            builder.ApplyConfiguration<RoleClaim>(this);
        }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {
           
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Pseudonym)
                .HasMaxLength(35);

            builder.Property(o => o.Email)
                .HasMaxLength(1024);
        }

        public virtual void Configure(EntityTypeBuilder<PasswordLogin> builder)
        {
            builder.HasKey(o => o.UserId);
           if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.UserId)
                .ValueGeneratedNever();
                
            builder.Property(o => o.Password)
                .HasMaxLength(1024);
        }

        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
            builder.Property(o => o.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Value)
                .HasMaxLength(1024)
                .IsRequired();
        }

        public class EmailPurposeConverter : EnumToNumberConverter<EmailPurpose, int>
        {
            
        }

        public class PhonePurposeConverter : EnumToNumberConverter<PhonePurpose, int>
        {
            
        }

        public void Configure(EntityTypeBuilder<EmailAddress> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Value)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(o => o.Name)
                .HasMaxLength(50);

            builder.Property(o => o.Purpose)
                .HasConversion(new EmailPurposeConverter());

            builder.HasIndex(o => o.Purpose);
        }

        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
            builder.Property(o => o.Value)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(o => o.Name)
                .HasMaxLength(50);

            builder.Property(o => o.Purpose)
                .HasConversion(new PhonePurposeConverter());

            builder.HasIndex(o => o.Purpose);
        }

        public void Configure(EntityTypeBuilder<Domain> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
        }

        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(100);
        }

        public void Configure(EntityTypeBuilder<MultiFactorPolicy> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Settings)
                .HasMaxLength(1024);
            
            builder.HasMany(o => o.Organizations)
                .WithOne(o => o.MultiFactorPolicy)
                .HasForeignKey(o => o.MultiFactorPolicyId)
                .OnDelete(DeleteBehavior.SetNull);

             builder.HasMany(o => o.Users)
                .WithOne()
                .HasForeignKey(o => o.MultiFactorPolicyId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public void Configure(EntityTypeBuilder<PasswordPolicy> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
            builder.Property(o => o.Name)
                .HasMaxLength(100);

            builder.Property(o => o.Composition)
                .HasConversion(new EnumToNumberConverter<PasswordComposition, int>());

            builder.HasMany(o => o.Users)
                .WithOne()
                .HasForeignKey(o => o.PasswordPolicyId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(o => o.Organizations)
                .WithOne(o => o.PasswordPolicy)
                .HasForeignKey(o => o.PasswordPolicyId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Description)
                .HasMaxLength(256);

            builder.HasMany(o => o.RolePermissions)
                .WithOne(o => o.Role)
                .HasForeignKey(o => o.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.UserRoles)
                .WithOne(o => o.Role)
                .HasForeignKey(o => o.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Description)
                .HasMaxLength(256);
        }

        public virtual void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey("RoleId", "PermissionId");
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
        }

        public virtual void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey("UserId", "RoleId");
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
        }

        public virtual void Configure(EntityTypeBuilder<ApiKeyRole> builder)
        {
            builder.HasKey("ApiKeyId", "RoleId");
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
        }

        public virtual void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.HasKey(o => new { o.ProviderName, o.Key });;
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.UserId)
                .ValueGeneratedNever();

            builder.Property(o => o.DisplayName)
                .HasMaxLength(128);

            builder.Property(o => o.ProviderName)
                .HasMaxLength(128);

            builder.Property(o => o.Key)
                .HasMaxLength(1024);
        }

        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Type)
                .IsRequired(true)
                .HasMaxLength(512);
            
            builder.Property(o => o.Value);
        }

        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.HasKey(o => o.Id);
            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);

            builder.Property(o => o.Type)
                .IsRequired(true)
                .HasMaxLength(512);
            
            builder.Property(o => o.Value);
        }

        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.HasKey(o => new { o.UserId, o.ProviderName, o.Name });

            if(!string.IsNullOrEmpty(this.schemaName))
                builder.Metadata.SetSchema(this.schemaName);
        }
    }
}