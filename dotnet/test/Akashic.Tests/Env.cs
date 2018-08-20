using Microsoft.Extensions.Configuration;
using NerdyMishka.Data;

public class Env
{
    private static IConfigurationRoot root;

    public static IConfigurationRoot Config
    {
        get
        {
            if(root ==null) {
                var builder = new ConfigurationBuilder();
                builder
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile("appsettings.test.json");

                root = builder.Build();
            }

            return root;
        }
    }


    public static DataConnection CreateConnection()
    {
        var connection = new DataConnection(
            Config.GetSection("akashic:default:provider").Value, 
            Config.GetSection("akashic:default:connectionString").Value    
        );

        return connection;
    }
}