
using NerdyMishka.FluentMigrator;
using System;
using Microsoft.Extensions.DependencyInjection;
using NerdyMishka.FluentMigrator.Runner;

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


            this.CreateTable("resource_kinds")
                .Pk()
                .Column<string>("name", limit: 256)
                .Column<string>("uri_path", limit: 256, isNullable: false)
                .Column<string>("table_name", limit: 512, isNullable: false)
                .Column<string>("clr_type_name", limit: 512);

            // resources can be a group of records or single record.
            // a single record could have multiple resources. 
            this.CreateTable("resources")
                .LongPk()
                .Column<int>("kind_id")
                .Column<long?>("row_id")
                .Column<bool>("is_deleted")
                    .WithDefaultValue(0);

            this.CreateTable("users")
                .Pk()
                .Column<string>("name", limit: 256, uniqueIndexName: "ux_users_name")
                .Column<string>("display_name", isNullable: true, limit: 256)
                .Column<string>("password", limit: 1024, isNullable: true)
                .Column<string>("icon_uri", limit: 1024, isNullable: true)
                .Column<string>("role_cache", limit: 2048, isNullable: true)
                .Column<bool>("is_admin")
                    .WithDefaultValue(false)
                .Column<bool>("is_banned")
                    .WithDefaultValue(false)
                .Column<long?>("resource_id");

            // production
            this.CreateTable("operational_environments")
                .Pk()
                .Column<string>("name", limit: 256)
                .Column<string>("alias", limit: 32)
                .Column<string>("uri_path", limit: 256, uniqueIndexName: "ux_openvs_uri")
                .Column<string>("description", limit: 512, isNullable: true)
                .Column<long?>("resource_id");

            // api key for a specific user
            this.CreateTable("user_api_keys")
                .Pk()
                .Column<int>("user_id")
                .Column<string>("api_key", limit: 1024)
                .Column<DateTime?>("expires_at");

            this.CreateTable("configuration_sets")
                .Pk()
                .Column<string>("name", limit: 256)
                .Column<int?>("operational_environment_id")
                .Column<long?>("resource_id");

            // configuration files can be owned by groups, operational environments
            // users, etc.
            this.CreateTable("configuration_files")
                .Pk()
                .Column<string>("uri_path", limit: 1024)
                .Column<byte[]>("blob", isNullable: true)
                .Column<string>("description", limit: 512)
                .Column<string>("encoding", isNullable: true, limit: 64)
                .Column<string>("mime_type", isNullable: true, limit: 124)
                .Column<bool>("is_encrypted").WithDefaultValue(true)
                .Column<bool>("is_key_external").WithDefaultValue(false)
                .Column<bool>("is_template")
                .Column<long?>("resource_id")
                .Column<int?>("configuration_set_id")
                .Column<int?>("user_id");

            this.CreateTable("public_keys")
                .Pk()
                .Column<string>("uri_path", limit: 2048, isNullable: false)
                .Column<byte[]>("blob")
                .Column<int?>("user_id");

            this.CreateTable("protected_blob_vaults")
                .Pk()
                .Column<string>("name", limit: 1024)
                .Column<byte[]>("salt", limit: 200) // used to hash key
                .Column<short>("key_type")
                .Column<int?>("user_id")
                .Column<int?>("operational_environment_id")
                .Column<int?>("public_key_id");

            this.CreateTable("protected_blobs")
                .Pk()
                .Column<string>("uri_path", limit: 2048, isNullable: false)
                .Column<byte[]>("blob")
                .Column<DateTime?>("expires_at")
                .Column<int>("protected_blob_vault_id")
                .Column<short>("blob_type")
                .Column<string>("tags", limit: 2048, isNullable: true);



            this.GrantRolePermissionsToTable("nexus_app", tables: new [] {
                "resource_kinds",
                "resources",
                "users",
                "user_api_keys",
                "operational_environments",
                "configuration_files",
                "configuration_sets",
                "public_keys",
                "protected_blob_vaults",
                "protected_blobs"
            });
        }
    }
}