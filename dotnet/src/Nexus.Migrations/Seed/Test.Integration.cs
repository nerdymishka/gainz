
using FluentMigrator;
using NerdyMishka.FluentMigrator;

namespace NerdyMishka.Nexus.Migrations
{
    [Profile("Test:Integration")]
    public class Integration : Migration
    {
        public override void Down()
        {
            throw new System.NotImplementedException();
        }

        public override void Up()
        {
            
            var schema = "nexus";

            this.DisableAutoIncement(schema, new [] {
                "resources"
            });

            this.InsertInto("resources", schema).Row(new {
                id = 1L,
                uri = "/users",
                type = "users",
                key = (long?)null,
                is_deleted = false
            });

            this.EnableAutoIncement(schema, new [] {
                "resources"
            });
        }
    }
}