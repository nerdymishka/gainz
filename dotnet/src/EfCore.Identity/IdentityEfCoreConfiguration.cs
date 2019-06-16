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
        IEntityTypeConfiguration<Permission>,
        IEntityTypeConfiguration<RolePermission>,
        IEntityTypeConfiguration<User>, 
        IEntityTypeConfiguration<UserRole>,
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
        }

        public virtual void Configure(EntityTypeBuilder<User> builder)
        {
           
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Pseudonym)
                .HasMaxLength(35);

            builder.Property(o => o.EmailHash)
                .HasMaxLength(1024);

            builder.HasMany(o => o.Phones)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.EmailAddresses)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

        
            builder.HasMany(o => o.UserRoles)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.ApiKeys)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.PasswordLogin)
                .WithOne(o => o.User)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public virtual void Configure(EntityTypeBuilder<PasswordLogin> builder)
        {
            builder.HasKey(o => o.UserId);
            builder.Property(o => o.UserId)
                .ValueGeneratedNever();
                
            builder.Property(o => o.Password)
                .HasMaxLength(1024);
        }

        public void Configure(EntityTypeBuilder<ApiKey> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(o => o.Value)
                .HasMaxLength(1024)
                .IsRequired();

            builder.HasMany(o => o.ApiKeyRoles)
                .WithOne(o => o.ApiKey)
                .HasForeignKey(o => o.ApiKeyId)
                .OnDelete(DeleteBehavior.Cascade);

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
            throw new System.NotImplementedException();
        }

        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasMany(o => o.Users)
                .WithOne(o => o.Organization)
                .HasForeignKey(o => o.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            
        }

        public void Configure(EntityTypeBuilder<MultiFactorPolicy> builder)
        {
            builder.HasKey(o => o.Id);

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
                .WithOne(o => o.MultiFactorPolicy)
                .HasForeignKey(o => o.MultiFactorPolicyId)
                .OnDelete(DeleteBehavior.SetNull);
        }

        public void Configure(EntityTypeBuilder<PasswordPolicy> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Name)
                .HasMaxLength(100);

            builder.Property(o => o.Composition)
                .HasConversion(new EnumToNumberConverter<PasswordComposition, int>());

            builder.HasMany(o => o.Users)
                .WithOne(o => o.PasswordPolicy)
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

            builder.Property(o => o.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.Description)
                .HasMaxLength(256);

        }

        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey("RoleId", "PermissionId");
        }

        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey("UserId", "RoleId");
        }

        public void Configure(EntityTypeBuilder<ApiKeyRole> builder)
        {
            builder.HasKey("ApiKeyId", "RoleId");
        }
    }
}