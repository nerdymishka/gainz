using System;
using Microsoft.EntityFrameworkCore;
using NerdyMishka.EfCore.Identity;

namespace Tests 
{
    public partial class SeedData 
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            //ApplyPolicies(modelBuilder);
            //ApplyOrganizations(modelBuilder);
            ApplyRoles(modelBuilder);
            ApplyUsers(modelBuilder);
        }

        

        public static void ApplyUsers(ModelBuilder mb)
        {
            mb.Entity<User>().HasData(
                new User() {
                    Id = 1,
                    DisplayName = "Serguei",
                    Pseudonym = "crash_override",
                    Email = "sg@nerdymishka.com",
                    IsActive = true,
                    IsEmailConfirmed = true,
                    IsPhoneConfirmed = true,
                    OrganizationId = null,
                    MultiFactorPolicyId = null,
                    PasswordPolicyId = null, 
                },
                new User() {
                    Id = 2,
                    DisplayName = "Nerdy",
                    Pseudonym = "nerdy",
                    Email = "nerdy@nerdymishka.com",
                    IsActive = true,
                    IsEmailConfirmed = true,
                    IsPhoneConfirmed = true,
                    //OrganizationId = 2,
                }
            );

           
        }

        public static void ApplyUserRoles(ModelBuilder mb)
        {
             mb.Entity<UserRole>().HasData(
                new UserRole() {
                    RoleId = 3,
                    UserId =2 
                },
                new UserRole() {
                    RoleId = 2,
                    UserId = 1
                }
            );
        }

        public static void ApplyRoles(ModelBuilder mb)
        {
            mb.Entity<Permission>().HasData(
                new Permission() {
                    Id = 1,
                    Code = Guid.NewGuid().ToString(),
                    Name = "read"
                }, 
                new Permission() {
                    Id = 2,
                    Code = Guid.NewGuid().ToString(),
                    Name = "write"
                },
                new Permission() {
                    Id = 3,
                    Code = Guid.NewGuid().ToString(),
                    Name = "execute",
                },
                new Permission() {
                    Id = 4, 
                    Code = Guid.NewGuid().ToString(),
                    Name = "audit"
                }
            );

            mb.Entity<Role>().HasData(
                new Role() {
                    Id = 1,
                    Name = "Viewer",
                    Code = Guid.NewGuid().ToString(),
                },
                new Role() {
                    Id = 2,
                    Name = "Contributor",
                    Code = Guid.NewGuid().ToString()
                },
                new Role() {
                    Id = 3,
                    Name = "Operator",
                    Code = Guid.NewGuid().ToString()
                }
            );

            mb.Entity<RolePermission>().HasData(
                new RolePermission() {
                    RoleId = 1,
                    PermissionId = 1
                },
                 new RolePermission() {
                    RoleId = 2,
                    PermissionId = 1
                },
                 new RolePermission() {
                    RoleId = 1,
                    PermissionId = 2
                },
                new RolePermission() {
                    RoleId = 3,
                    PermissionId = 1
                },
                new RolePermission() {
                    RoleId = 3,
                    PermissionId = 2
                },
                new RolePermission() {
                    RoleId = 3,
                    PermissionId = 3
                }
            );
        }

        public static void ApplyPolicies(ModelBuilder mb)
        {
            

            mb.Entity<PasswordPolicy>().HasData(
                new PasswordPolicy() {
                    Id = 1,
                    LifetimeInDays = null,
                    Minimum = 12,
                    Name = "Default",
                    Composition = PasswordComposition.All,
                }
            );

            mb.Entity<MultiFactorPolicy>().HasData(
                new MultiFactorPolicy() {
                    Id = 1,
                    Name = "Default",
                    IsEmailRequired = true,
                    IsEnabled = true,
                }
            );
        }

        public static void ApplyOrganizations(ModelBuilder mb)
        {
            mb.Entity<Organization>().HasData(
                new Organization() {
                    Id = 1,
                    Name = "Public",
                    Code = Guid.NewGuid().ToString(),
                    MultiFactorPolicyId = 1,
                    PasswordPolicyId = 1, 
                }
            );

            mb.Entity<Organization>().HasData(
                new Organization() {
                    Id = 2,
                    Name = "NerdyMishka",
                    Code = Guid.NewGuid().ToString(),
                    MultiFactorPolicyId =  1          
                }
            );

            mb.Entity<Domain>().HasData(
             
                new Domain() {
                    Id = 1,
                    Name = "gmail.com",
                    OrganizationId = 1
                },
                new Domain() {
                    Id = 2,
                    Name = "hotmail.com",
                    OrganizationId = 1
                },
                new Domain() {
                    Id = 3,
                    Name = "yahoo.com",
                    OrganizationId = 1,
                },
                new Domain() {
                    Id = 4,
                    Name = "nerdymishka.com",
                    OrganizationId = 2,
                }
            );
        }
    }
}