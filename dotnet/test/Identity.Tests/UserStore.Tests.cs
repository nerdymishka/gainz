using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Xunit;
using NerdyMishka.EfCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace NerdyMishka.Identity.Tests
{
    public partial class UserStore_Tests
    {
        public class Context : IDisposable
        {
            public IdentityDbContext Db { get; set; }

            public UserStore Store { get; set; }

            public void Dispose()
            {
                this.Store.Dispose();
                this.Db.Dispose();
            }
        }

        protected virtual Context GenerateContext(string dbName, Action<ServiceCollection> assemble = null)
        {
            var services = Env.GenerateProvider("UserStore_" + dbName, assemble);
            return new Context() {
                Db = (IdentityDbContext)services.GetService(typeof(DbContext)),
                Store = (UserStore)services.GetService(typeof(IUserStore<EfCore.Identity.User>))
            };
        }
        
        [Fact]
        public void Constructor()
        {
            var c = this.GenerateContext(nameof(Constructor));
           

            Assert.NotNull(c.Store);
        }

        [Fact]
        public async void Create_user()
        {
            using(var c = this.GenerateContext("CreateUser"))
            {
                var db = c.Db;
                var store = c.Store;

                var count = await db.Users.CountAsync();
                Assert.Equal(0, count);

                var result = await store.CreateAsync(new User() { Pseudonym = "newUser" });
                count = await db.Users.CountAsync();
                Assert.True(result.Succeeded);
                Assert.Equal(1, count);
            }
        }

        [Fact]
        public async void Delete_user()
        {
            using(var c = this.GenerateContext("DeleteUser"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="d3leteMe" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var count = await db.Users.CountAsync();
                Assert.Equal(1, count);

                var result = await store.DeleteAsync(user);
                count = await db.Users.CountAsync();
                Assert.True(result.Succeeded);
                Assert.Equal(0, count);
            }
        }

        [Fact]
        public async void Update_user()
        {
            using(var c = this.GenerateContext("UpdateUser"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="updateMe" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var user2 = await db.Users.FirstOrDefaultAsync();
                Assert.NotNull(user2);
                user2.Pseudonym = "updateMe2";

                var result = await store.UpdateAsync(user);
                Assert.True(result.Succeeded);
                var user3 = await db.Users.FirstOrDefaultAsync();
                Assert.Equal("updateme2", user3.Pseudonym);
            }
        }


        [Fact]
        public async void Get_UserName()
        {
            using(var c = this.GenerateContext("UserName"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var name = await store.GetUserNameAsync(user);
                Assert.Equal(user.DisplayName, name);
            }
        }

        [Fact]
        public async void Get_NormalizedUserName()
        {
            using(var c = this.GenerateContext("NormalizedUserName"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var name = await store.GetNormalizedUserNameAsync(user);
                Assert.Equal(user.Pseudonym, name);
            }
        }


           [Fact]
        public async void Set_UserName()
        {
            using(var c = this.GenerateContext("SetUserName"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                var name = "IHazCoolerName";
                db.Users.Add(user);
                await db.SaveChangesAsync();

                // does not save changes.
                await store.SetUserNameAsync(user,  name);
                Assert.Equal(name, user.DisplayName);
            }
        }

        [Fact]
        public async void Set_NormalizedUserName()
        {
            using(var c = this.GenerateContext("SetNormalizedUserName"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                var name = "IHazCoolerName";
                await db.SaveChangesAsync();

                await store.SetUserNameAsync(user, name);
                Assert.Equal(name.ToLowerInvariant(), user.Pseudonym);
            }
        }

        [Fact]
        public async void Get_UserId()
        {
            using(var c = this.GenerateContext("Get_UserId"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var id =  await store.GetUserIdAsync(user);
                Assert.Equal("1", id);
            }
        }

        [Fact]
        public async void Find_UserById()
        {
            using(var c = this.GenerateContext("Find_UserById"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var result =  await store.FindByIdAsync("1");
                Assert.Equal(result.Id, user.Id);
                Assert.Equal(result.Pseudonym, user.Pseudonym);
            }
        }

        [Fact]
        public async void Find_UserByName()
        {
            using(var c = this.GenerateContext("Find_UserById"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var result =  await store.FindByNameAsync("ihazname");
                Assert.Equal(result.Id, user.Id);
                Assert.Equal(result.Pseudonym, user.Pseudonym);
            }
        }
    }
}
