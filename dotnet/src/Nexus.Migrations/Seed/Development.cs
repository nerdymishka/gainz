

using FluentMigrator;

namespace NerdyMishka.Nexus.Migrations
{
    [Profile("Development")]
    public class Development : Migration
    {
        public override void Down()
        {
            throw new System.NotImplementedException();
        }

        public override void Up()
        {
            this.Insert.IntoTable("users").InSchema("nexus").Row(new {
                name = "admin@nerdymishka.com",
                display_name = "admin",
                is_banned = false 
            });
        }
    }
}