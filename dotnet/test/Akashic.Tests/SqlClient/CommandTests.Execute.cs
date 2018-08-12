using NerdyMishka.Data;
using System.Data;
using Xunit;

namespace NerdyMishka.Data.SqlClient.Tests
{
    public partial class CommandTests
    {
        private static string cs = "Server=(LocalDb)\\MSSQLLocalDB;Integrated Security=SSPI;Database=master";
        
        [Fact]
        public static void ExecuteString()
        {
            var home = System.Environment.GetEnvironmentVariable("UserProfile");
            var db  = System.IO.Path.Combine(home, "ExecuteTest.mdf");
            var name = "ExecuteTest";
            using(var connection = CreateConnection())
            {
                
                CreateDatabase(connection, name); 
                System.Threading.Thread.Sleep(500);
                
                Assert.True(System.IO.File.Exists(db));
               
                DropDatabase(connection,  name);
                System.Threading.Thread.Sleep(500);
                Assert.False(System.IO.File.Exists(db));
            }
        }

        private static void CreateDatabase(
            DataConnection connection,
            string databaseName
        ) {
            var home = System.Environment.GetEnvironmentVariable("USERPROFILE");
            if(string.IsNullOrWhiteSpace(home))

            System.Console.WriteLine(home);
            var builder = connection.CreateSqlBuilder();
            builder.AppendLine($"CREATE DATABASE {databaseName}");
            builder.AppendLine($@"            
ON PRIMARY(
    NAME = {databaseName},
    FILENAME = '{home}\{databaseName}.mdf'
            
)
LOG ON (
    NAME = {databaseName}_log,
    FILENAME = '{home}\{databaseName}_log.ldf'
)
");         connection.Execute(builder);
        }

        private static void DropDatabase(
            DataConnection connection,
            string databaseName)
        {
            connection.Execute($"ALTER DATABASE {databaseName} SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            connection.Execute($"DROP DATABASE {databaseName}");
        }

        private static DataConnection CreateConnection()
        {
            return new DataConnection(KnownProviders.SqlServer, cs);
        }
    }
}