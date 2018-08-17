using NerdyMishka.Data;
using System.Data;
using Xunit;

namespace NerdyMishka.Data.SqlClient.Tests
{
    public partial class CommandTests
    {
        [Fact]
        public static void Fetch()
        {
            using(var connection = Env.CreateConnection())
            {
                using(var dr = connection.FetchReader("SELECT 1"))
                {
                    Assert.Equal(DataConnectionState.Open, connection.State);
                    
                    Assert.NotNull(dr);
                    var read = false;
                    while(dr.Read())
                    {
                        read = true;
                        Assert.Equal(dr.GetInt32(0), 1);
                    }

                    Assert.True(read);
                }
            }
        }
    }
}