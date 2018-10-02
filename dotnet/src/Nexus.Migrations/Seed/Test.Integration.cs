using ProfileAttribute = FluentMigrator.ProfileAttribute;
using NerdyMishka.FluentMigrator;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Nexus.Migrations
{
    [Profile("Nexus:Core:Integration")]
    public class Integration : Migration
    {
        public override void Down()
        {
            throw new System.NotImplementedException();
        }

        public override void Up()
        {
            
            var schema = "nexus";

          

            
        }
    }
}