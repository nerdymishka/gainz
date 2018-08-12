using NerdyMishka.Data;
using System.Data;
using Xunit;

namespace NerdyMishka.Data.Tests
{
    public class DataConnectionTests
    {
        private static string cs = "Server=(LocalDb)\\MSSQLLocalDB;Integrated Security=SSPI;";
        
        [Fact]
        public static void Constructor_KnownProvider() 
        {
            var connection = CreateConnection();
            Assert.NotNull(connection);
            
            Assert.Equal(DataConnectionState.Closed, connection.State);  
            Assert.NotNull(connection.SqlDialect);
        }

        [Fact]
        public static void Open()
        {
            using(var connection = CreateConnection())
            {
                connection.Open();
                
                Assert.Equal(DataConnectionState.Open, connection.State);
            }
        }

        [Fact]
        public static void Close()
        {
            using(var connection = CreateConnection())
            {
                connection.Open();
                
                Assert.Equal(DataConnectionState.Open, connection.State);

                connection.Close();
                Assert.Equal(DataConnectionState.Closed, connection.State);
            }
        }

        [Fact]
        public static void CreateCommand()
        {
            using(var connection = CreateConnection())
            {
                var cmd = connection.CreateCommand();
                Assert.NotNull(cmd);
                Assert.Equal(System.Data.CommandType.Text, cmd.Type);
            }
        }

        [Fact]
        public static void CreateCommand_WithBehavior()
        {
            using(var connection = CreateConnection())
            {
                var cmd = connection.CreateCommand(CommandBehavior.CloseConnection);
                Assert.NotNull(cmd);
                Assert.Equal(System.Data.CommandType.Text, cmd.Type);
                Assert.Equal(CommandBehavior.CloseConnection, cmd.Behavior);
            }
        }

        [Fact]
        public static void BeginTransaction()
        {
            using(var connection = CreateConnection())
            {
                var transaction = connection.BeginTransaction();
                Assert.NotNull(transaction);
            }
        }

        private static DataConnection CreateConnection()
        {
            return new DataConnection(KnownProviders.SqlServer, cs);
        }
    }
}