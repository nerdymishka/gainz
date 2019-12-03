using NerdyMishka.Data;
using System.Data;
using Xunit;

namespace Tests
{
    //[Unit]
    [Trait("tag", "unit")]
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
                        Assert.Equal(1, dr.GetInt32(0));
                    }

                    Assert.True(read);
                }
            }
        }
    }
}