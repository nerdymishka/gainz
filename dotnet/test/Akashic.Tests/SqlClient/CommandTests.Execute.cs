using NerdyMishka.Data;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NerdyMishka.Data.SqlClient.Tests
{
    public partial class CommandTests
    {
        [Fact]
        public static void ExecuteString()
        {
            var read = false;
            using(var connection = Env.CreateConnection())
            {
                // connection must be open to stay open when calling execute.
                // sqlite instance is in memory, so once the connection closes
                // the table is gone.
                connection.Open();

                connection.Execute(@"CREATE TABLE test (
                    id INTEGER PRIMARY KEY ASC,
                    name TEXT
                )         
                ");

                connection.Execute("INSERT INTO test (name) VALUES ('one');");
                connection.Execute("INSERT INTO test(name) VALUES('two');");

                var parameters = new Hashtable();
                parameters.Add("Name", "one");

                

                using (var dr = connection.FetchReader("SELECT * FROM test WHERE name = @Name", parameters))
                {
                    while (dr.Read())
                    {
                        read = true;
                        var id = dr.GetInt32("id");
                        var name = dr.GetString("name");

                        Assert.Equal(1, id);
                        Assert.Equal("one", name);
                    }
                }
                Assert.True(read);
            }
        }



        [Fact]
        public static void ExecuteString_Hashtable()
        {
            var read = false;
            using(var connection = Env.CreateConnection())
            {
                // connection must be open to stay open when calling execute.
                // sqlite instance is in memory, so once the connection closes
                // the table is gone.
                connection.Open();

                var t = connection.Execute(@"CREATE TABLE test (
                    id INTEGER PRIMARY KEY ASC,
                    name TEXT
                )         
                ");
              

                var builder = new QueryBuilder(connection);
                builder.Append("INSERT INTO test (name) VALUES (@Name);")
                    .AddParameter("Name", "One")
                    .Excecute();

                var result = builder.Append("INSERT INTO test(name) VALUES(@Name);")
                    .AddParameter("Name", "Two")
                    .Excecute();

                
                var parameters = new Hashtable();
                parameters.Add("Name", "One");

                

                using (var dr = connection.FetchReader("SELECT * FROM test WHERE name = @Name", parameters))
                {
                    while (dr.Read())
                    {
                        read = true;
                        var id = dr.GetInt32("id");
                        var name = dr.GetString("name");

                        Assert.Equal(1, id);
                        Assert.Equal("One", name);
                    }
                }
                Assert.True(read);
            }
        }
    


    /*
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
    } */

    }
}