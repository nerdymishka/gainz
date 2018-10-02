using ProfileAttribute = FluentMigrator.ProfileAttribute;
using NerdyMishka.FluentMigrator;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Nexus.Migrations
{
    [Profile("Nexus:Core:Production")]
    public class Production : Migration
    {
        public override void Down()
        {
            throw new System.NotImplementedException();
        }

        public override void Up()
        {
            /*
            var schema = "nexus";

            var tableNames = new [] {
                "resource_kinds",
                "resources",
                "users",
                "roles",
                "groups",
                "operational_environments",
                "user_api_keys",
                "configuration_files"
            };

            const int users = 1;
            const int roles = 2;
            const int groups = 3;
            const int opEnvs = 4;
            //const int configFiles = 5;

            this.DisableAutoIncement(schema, tableNames[0]);

            this.InsertInto("resource_kinds", schema).Rows(
                new {
                    id = 1,
                    name = "users",
                    uri_path = "/users"
                },
                new {
                    id = 2,
                    name = "roles",
                    uri_path = "/roles"
                },
                 new {
                    id = 3,
                    name = "groups",
                    uri_path = "/groups"
                },
                new {
                    id = 4,
                    name = "operational_environments",
                    uri_path = "/operational-environments"
                }, 
                new {
                    id = 5,
                    name = "configuration_files",
                    uri_path = "/configuration-files"
                }
            );

            this.EnableAutoIncement(schema, tableNames[0]);
            this.DisableAutoIncement(schema, tableNames[2]);
            var resourceId = 1L;
            this.InsertInto("users", schema).Rows(
                new {
                    id = 1,
                    name = "",
                    display_name = "system",
                    resource_id = resourceId
                },
                 new {
                    id = 2,
                    name = "admin@ip.com",
                    display_name = "admin",
                    resource_id = ++resourceId
                }
            );
            this.EnableAutoIncement(schema, tableNames[2]);
            this.DisableAutoIncement(schema, tableNames[3]);
            this.InsertInto("roles", schema).Rows(new {
                id = 1,
                uri_path = "nexus-owner",
                name = "Nexus Owner",
                resource_id = ++resourceId,
            },
            new {
                id = 2,
                uri_path = "nexus-manager",
                name = "Nexus Manager",
                resource_id = ++resourceId,
            },
            new {
                id = 3,
                uri_path = "nexus-operator",
                name = "Nexus Operator",
                resource_id = ++resourceId,
            },
            new {
                id = 4,
                uri_path = "nexus-contributor",
                name = "Nexus Contributor",
                resource_id = ++resourceId,
            },new {
                id = 5,
                uri_path = "nexus-reader",
                name = "Nexus Reader",
                resource_id = ++resourceId,
            },
            new {
                id = 6,
                uri_path = "nexus-auditor",
                name = "Nexus Auditor",
                resource_id = ++resourceId,
            },
            new {
                id = 7,
                uri_path = "nexus-billing",
                name = "Nexus Billing",
                resource_id = ++resourceId,
            });
            this.EnableAutoIncement(schema, tableNames[3]);
            this.DisableAutoIncement(schema, tableNames[4]);
            this.InsertInto("groups", schema).Rows(new {
                id = 1,
                uri_path = "nexus-admins",
                name = "Nexus Admins",
                resource_id = ++resourceId
            });
            this.EnableAutoIncement(schema, tableNames[4]);
            this.InsertInto("groups_users", schema).Rows(new {
                // 
                group_id = 1,
                user_id = 2,
                membership_type = (byte)8
            });

            this.InsertInto("roles_users", schema).Rows(new {
                // System User / Nexus Manager
                role_id = 2,
                user_id = 1
            });

            this.InsertInto("roles_groups", schema).Rows(new {
                // Nexu Admin / Nexus Owners
                role_id = 1,
                group_id = 1
            });
            this.DisableAutoIncement(schema, tableNames[5]);
            this.InsertInto("operational_environments", schema).Rows(new {
                id = 1,
                uri_path = "production",
                name = "Production",
                alias = "prod",
                resource_id = ++resourceId
            },
            new {
                id = 2,
                uri_path = "staging",
                name = "Staging",
                alias = "staging",
                resource_id = ++resourceId
            }, new {
                id = 3,
                uri_path = "test",
                name = "Test",
                alias = "test",
                resource_id = ++resourceId
            }, new {
                id = 4,
                uri_path = "qa",
                name = "Quality Assurance",
                alias = "qa",
                resource_id = ++resourceId
            },new {
                id = 5,
                uri_path = "development",
                name = "Development",
                alias = "dev",
                resource_id = ++resourceId
            });
            this.EnableAutoIncement(schema, tableNames[5]);

            // api key 
            // 9hxMxFNlxTVaYkcI|BD

            var auth = new PasswordAuthenticator();

            this.DisableAutoIncement(schema, tableNames[6]);
            this.InsertInto("user_api_keys", schema).Rows(new {
                id = 1,
                user_id = 2,
                api_key = auth.ComputeHash("9hxMxFNlxTVaYkcI|BD")
            });

            this.EnableAutoIncement(schema, tableNames[6]);

            this.InsertInto("user_api_keys_roles", schema).Rows(new {
                user_api_key_id = 1,
                role_id = 1
            });


            resourceId = 1L;
            this.DisableAutoIncement(schema, tableNames[1]);
            this.InsertInto("resources", schema).Rows(
                new{
                    id = resourceId,
                    kind_id = users,
                    key = 1
                },
                new{
                    id = ++resourceId,
                    kind_id = users,
                    key = 2
                },
                new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 1
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 2
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 3
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 4
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 5
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 6
                },
                 new{
                    id = ++resourceId,
                    kind_id = roles,
                    key = 7
                },

                new {
                    id = ++resourceId,
                    kind_id = groups,
                    key = 1
                },

                new {
                    id = ++resourceId,
                    kind_id = opEnvs,
                    key = 1
                },

                 new {
                    id = ++resourceId,
                    kind_id = opEnvs,
                    key = 2
                },

                 new {
                    id = ++resourceId,
                    kind_id = opEnvs,
                    key = 3
                },

                 new {
                    id = ++resourceId,
                    kind_id = opEnvs,
                    key = 4
                },

                 new {
                    id = ++resourceId,
                    kind_id = opEnvs,
                    key = 5
                }
            );
            this.EnableAutoIncement(schema, tableNames[1]); */
        }
    }
}