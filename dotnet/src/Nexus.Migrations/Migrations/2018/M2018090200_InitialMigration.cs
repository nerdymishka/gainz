using FluentMigrator;
using NerdyMishka.FluentMigrator;
using System;

namespace NerdyMishka.Nexus.Migrations
{   

    [NerdyMishkaMigration(2018090200L, "core")]
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

            // resources can be a group of records or single record.
            // a single record could have multiple resources. 
            this.CreateTable("resources")
                .LongPk()
                .Column<string>("uri", limit: 2048)
                .Column<string>("type", limit: 128)
                .Column<int?>("key")
                .Column<bool>("is_deleted");

            this.CreateTable("users")
                .Pk()
                .Column<string>("name", limit: 256, uniqueIndexName: "ux_users_name")
                .Column<string>("display_name", isNullable: true, limit: 256)
                .Column<bool>("is_banned")
                    .WithDefaultValue(false)
                .Column<long?>("resource_id");

            // production
            this.CreateTable("operational_environments")
                .Pk()
                .Column<string>("uri_fragment", limit: 256, uniqueIndexName: "ux_openvs_uri")
                .Column<string>("display_name", limit: 256)
                .Column<string>("alias", limit: 32)
                .Column<long?>("resource_id");

            this.CreateTable("groups")
                .Pk()
                .Column<string>("uri_fragment", limit: 256, uniqueIndexName: "ux_groups_uri")
                .Column<string>("display_name", limit: 256)
                .Column<long?>("resource_id");

            this.CreateTable("roles")
                .Pk()
                .Column<string>("uri_fragment", limit: 256, isNullable: false, uniqueIndexName: "ux_roles_uri")
                .Column<string>("display_name", limit: 256)
                .Column<long?>("resource_id");

            this.CreateTable("groups_users")
                .Column<int>("group_id", isPrimaryKey: true)
                .Column<int>("user_id", isPrimaryKey: true)
                .Column<byte>("membership_type");

            // resources that an operational env has like a configuration files
            // role "Dev Readers
            // resource - dev environment
            // actions - list / read
            this.CreateTable("operational_environments_resources")
                .Column<int>("operational_environment_id", isPrimaryKey: true)
                .Column<long>("resource_id", isPrimaryKey: true);

            // what can this role do for a given resource
            this.CreateTable("roles_resources")
                .Column<int>("role_id", isPrimaryKey: true)
                .Column<long>("resource_id", isPrimaryKey: true)
                .Column<byte>("actions");

            this.CreateTable("roles_users")
                .Column<int>("user_id", isPrimaryKey: true)
                .Column<int>("role_id", isPrimaryKey: true);

            this.CreateTable("roles_groups")
                .Column<int>("role_id", isPrimaryKey: true)
                .Column<int>("group_id", isPrimaryKey: true);

            // api key for a specific user
            this.CreateTable("user_api_keys")
                .Pk()
                .Column<int>("user_id")
                .Column<string>("api_key", limit: 1024)
                .Column<DateTime?>("expires_at");

            // roles the api can use
            this.CreateTable("user_api_keys_roles")
                .Column<int>("user_api_key_id", isPrimaryKey: true)
                .Column<int>("role_id", isPrimaryKey: true);

            // configuration files can be owned by groups, operational environments
            // users, etc.
            this.CreateTable("configuration_files")
                .Pk()
                .Column<string>("uri_fragment", limit: 256)
                .Column<byte[]>("content", isNullable: true)
                .Column<string>("description", limit: 512)
                .Column<string>("encoding", isNullable: true, limit: 64)
                .Column<string>("mime_type", isNullable: true, limit: 124)
                .Column<bool>("is_encrypted").WithDefaultValue(true)
                .Column<long?>("resource_id")
                .Column<long?>("owner_resource_id");

            this.GrantRolePermissionsToTable("nexus_app", new [] {
                "resources",
                "users",
                "operational_environments",
                "groups",
                "roles",
                "roles_groups",
                "roles_users",
                "roles_resources",
                "user_api_keys",
                "user_api_keys_roles",
                "operational_environments_resources",
                "configuration_files"
            });
        }
    }
}