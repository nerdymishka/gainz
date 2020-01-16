using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NerdyMishka.EfCore.Identity;
using NerdyMishka.EfCore;
using NerdyMishka.Security.Cryptography;
using NerdyMishka.Identity;
using Mettle;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;



namespace Tests 
{
    public class InMemoryServices : IServiceProviderFactory
    {
        public IServiceProvider CreateProvider()
        {
            string dbName = Guid.NewGuid().ToString();
            var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            serviceCollection.AddSingleton<IAssert>((s) => AssertImpl.Current);
            serviceCollection.AddTransient<ITestOutputHelper, TestOutputHelper>();

            serviceCollection.AddDbContext<IdentityDbContext, InMemoryDbContext>((builder) => {
               
                builder
                    .UseInMemoryDatabase(dbName)
                    
                    .UseModelConfiguration(
                        Microsoft.EntityFrameworkCore.InMemory.Metadata.Conventions.InMemoryConventionSetBuilder.Build(),
                        (mb) => {
                            var module = new IdentityEfCoreConfiguration(seedData: SeedData.Apply);
                            module.Apply(mb);
                    });
            });

            serviceCollection.AddDbContext<DbContext, InMemoryDbContext>((builder) => {
                builder
                    .UseInMemoryDatabase(dbName)
                    .UseModelConfiguration(
                        Microsoft.EntityFrameworkCore.InMemory.Metadata.Conventions.InMemoryConventionSetBuilder.Build(),
                        (mb) => {
                        var module = new IdentityEfCoreConfiguration(seedData: SeedData.Apply);
                        module.Apply(mb);
                    });
            });

            serviceCollection.AddSingleton<IPasswordAuthenticator>(new PasswordAuthenticator());
            serviceCollection.AddTransient<IUserStore<User>, UserStore>();
            serviceCollection.AddTransient<UserStore, UserStore>();

            var sp = serviceCollection.BuildServiceProvider();
            var db = sp.GetService<IdentityDbContext>();
            db.Database.EnsureCreated();
            var assert = sp.GetService(typeof(IAssert));
            return sp;
        }
    }
}