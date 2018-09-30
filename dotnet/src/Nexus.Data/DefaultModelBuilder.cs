using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class DefaultModelBuilder : 
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ConfigurationFileRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<GroupRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<GroupUserRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironmentRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironmentResourceRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ResourceRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleGroupRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleResourceRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleUserRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<UserApiKeyRecord>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<UserApiKeyRoleRecord>
    {
        public void Configure(EntityTypeBuilder<GroupUserRecord> builder)
        {
            builder
                .Property("UserId")
                .HasColumnName("user_id");


            builder.ToTable("groups_users")
                .HasKey("UserId", "GroupId");
        }

        public void Configure(EntityTypeBuilder<ConfigurationFileRecord> builder)
        {
        }

        public void Configure(EntityTypeBuilder<GroupRecord> builder)
        {
          
        }

        public void Configure(EntityTypeBuilder<OperationalEnvironmentRecord> builder)
        {
           
        }

        public void Configure(EntityTypeBuilder<OperationalEnvironmentResourceRecord> builder)
        {
            builder.HasKey("OperationalEnvironmentId", "ResourceId");
        }

        public void Configure(EntityTypeBuilder<ResourceRecord> builder)
        {
           
        }

        public void Configure(EntityTypeBuilder<RoleRecord> builder)
        {
            
        }

        public void Configure(EntityTypeBuilder<RoleGroupRecord> builder)
        {
            builder.HasKey("RoleId", "GroupId");
        }

        public void Configure(EntityTypeBuilder<RoleResourceRecord> builder)
        {
            builder.HasKey("RoleId", "ResourceId");
        }

        public void Configure(EntityTypeBuilder<RoleUserRecord> builder)
        {
            builder.HasKey("RoleId", "UserId");
        }

        public void Configure(EntityTypeBuilder<UserApiKeyRecord> builder)
        {
            
        }

        public void Configure(EntityTypeBuilder<UserApiKeyRoleRecord> builder)
        {
            builder.HasKey("UserApiKeyId", "RoleId");
        }
    }
}