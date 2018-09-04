using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Data
{
    public class DefaultModelBuilder : 
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<ConfigurationFile>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Group>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<GroupUser>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironment>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<OperationalEnvironmentResource>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Resource>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Role>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleGroup>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleResource>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<RoleUser>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<UserApiKey>,
        Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<UserApiKeyRole>
    {
        public void Configure(EntityTypeBuilder<GroupUser> builder)
        {
            builder
                .Property("UserId")
                .HasColumnName("user_id");


            builder.ToTable("groups_users")
                .HasKey("UserId", "GroupId");
        }

        public void Configure(EntityTypeBuilder<ConfigurationFile> builder)
        {
        }

        public void Configure(EntityTypeBuilder<Group> builder)
        {
          
        }

        public void Configure(EntityTypeBuilder<OperationalEnvironment> builder)
        {
           
        }

        public void Configure(EntityTypeBuilder<OperationalEnvironmentResource> builder)
        {
            builder.HasKey("OperationalEnvironmentId", "ResourceId");
        }

        public void Configure(EntityTypeBuilder<Resource> builder)
        {
           
        }

        public void Configure(EntityTypeBuilder<Role> builder)
        {
            
        }

        public void Configure(EntityTypeBuilder<RoleGroup> builder)
        {
            builder.HasKey("RoleId", "GroupId");
        }

        public void Configure(EntityTypeBuilder<RoleResource> builder)
        {
            builder.HasKey("RoleId", "ResourceId");
        }

        public void Configure(EntityTypeBuilder<RoleUser> builder)
        {
            builder.HasKey("RoleId", "UserId");
        }

        public void Configure(EntityTypeBuilder<UserApiKey> builder)
        {
            
        }

        public void Configure(EntityTypeBuilder<UserApiKeyRole> builder)
        {
            builder.HasKey("UserApiKeyId", "RoleId");
        }
    }
}