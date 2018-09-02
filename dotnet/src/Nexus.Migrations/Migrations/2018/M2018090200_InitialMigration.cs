using FluentMigrator;
using NerdyMishka.FluentMigrator;
using System;

namespace NerdyMishka.Nexus.Migrations
{
    [NerdyMishkaMigration("9/2/2018", 0, "core")]
    public class M2018090200_InitialMigration : Migration
    {
        public override void Down()
        {
            this.DropTables(new string[] {
                "configuration_files",
                "operational_environments_roles",
                "operational_environments",
                "user_api_keys_roles",
                "user_api_keys",
                "user_roles",
                "roles",
                "users"
            });

            this.DropRole("nexus_app");
           
        }

        public override void Up()
        {
            // nexus schema will exit because its the default
            // schema used by the default version table
        
            this.CreateRole("nexus_app");


            this.CreateTable("users")
                .Pk()
                .Column<string>("name", limit: 256)
                .Column<string>("display_name", isNullable: true, limit: 256)
                .Column<bool>("is_banned")
                    .WithDefaultValue(false);

            this.CreateTable("roles")
                .Pk()
                .Column<string>("name", limit: 256);

            this.CreateTable("users_roles")
                .Column<int>("user_id", isPrimaryKey: true)
                .Column<int>("role_id", isPrimaryKey: true);
                

             this.CreateTable("user_api_keys")
                .Pk()
                .Column<int>("user_id")
                .Column<string>("api_key", limit: 1024)
                .Column<DateTime?>("expires_at");

            this.CreateTable("user_api_keys_roles")
                .Column<int>("user_api_key_id", isPrimaryKey: true)
                .Column<int>("role_id", isPrimaryKey: true);

            this.CreateTable("operational_environments")
                .Pk()
                .Column<string>("name", limit: 256, uniqueIndexName: "ux_name");

            this.CreateTable("operational_environments_roles")
                .Pk()
                .Column<int>("role_id", isPrimaryKey: true)
                .Column<int>("operational_environment_id", isPrimaryKey: true);

            this.CreateTable("configuration_files")
                .Pk()
                .Column<string>("name", limit: 500)
                .Column<byte[]>("file", isNullable: true)
                .Column<bool>("is_encrypted").WithDefaultValue(true)
                .Column<int?>("operational_environment_id");
            

            this.GrantRolePermissionsToTable("nexus_app", new [] {
                "users",
                "roles",
                "users_roles",
                "user_api_keys",
                "user_api_keys_roles",
                "operational_environments",
                "operational_environments_roles",
                "configuration_files"
            });
        }
    }
}