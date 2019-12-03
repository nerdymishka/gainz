
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NerdyMishka.EfCore.Identity;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Identity.Tests
{
    internal class Env 
    {

        public static ServiceCollection GenerateServices(string dbName)
        {
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

            string defaultName = "Db" + new Random().Next(0, 10000);

            serviceCollection.AddDbContext<IdentityDbContext, InMemoryDbContext>((builder) => {
                builder.UseInMemoryDatabase(databaseName: dbName ?? defaultName);
            });

            serviceCollection.AddDbContext<DbContext, InMemoryDbContext>((builder) => {
                builder.UseInMemoryDatabase(databaseName: dbName ?? defaultName);
            });

         

            serviceCollection.AddSingleton<IPasswordAuthenticator>(new PasswordAuthenticator());

            serviceCollection.AddTransient<IUserStore<EfCore.Identity.User>, UserStore>();


            return serviceCollection;
        }

        public static ServiceProvider GenerateProvider(string dbName = null, Action<ServiceCollection> assemble = null)
        {
            var services = GenerateServices(dbName);
            if(assemble != null)
                assemble(services);

            return services.BuildServiceProvider();
        }



       
    }
}