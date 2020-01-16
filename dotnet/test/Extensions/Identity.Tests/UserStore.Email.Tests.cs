using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Xunit;
using NerdyMishka.EfCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mettle;
using Xunit.Abstractions;
using Serilog;
using Serilog.Events;
using NerdyMishka.Identity;
using System.Linq;
using NerdyMishka.EfCore;

namespace Tests
{
    
    public partial class UserStore_Tests 
    {
        private IAssert assert;

        public UserStore_Tests(IAssert assert)
        {
            this.assert = assert;
        }

        [UnitTest]
        public async void Find_UserByEmail(IdentityDbContext db, UserStore store)
        {
              
            string email = "nerdy@nerdymishka.com";
            var user =  await store.FindByEmailAsync("nerdy@nerdymishka.com");
            assert = assert ?? AssertImpl.Current;
            assert.NotNull(user);
            assert.Equal(2, user.Id);
            assert.Equal(email, user.Email);     
        }

        [UnitTest]
        public async void Set_Email(IdentityDbContext db, UserStore store)
        {
            
            var name = Guid.NewGuid().ToString();
            var email = name + "@nerdymishka.com";
            var user = new User() { 
                Pseudonym = name,
                DisplayName = name 
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            assert.Null(user.Email);

            await store.SetEmailAsync(user, email);
            await store.SaveChanges();

            user = await db.Users.SingleAsync(o => o.Email == email);
            assert.NotNull(user);
            assert.NotNull(user.Email);
            assert.Equal(email, user.Email);

            var email1 = db.EmailAddresses
                    .SingleOrDefault(
                        o => o.UserId == user.Id && 
                        o.Purpose == 0);

            assert.NotNull(email1);
            assert.Equal(email.ToLowerInvariant(), user.Email);
            assert.Equal(email, email1.Value);
        }

    /*

        [Fact]
        [Trait("tag", "unit")]
        //[Unit]

        public async void Set_NormalizedEmail()
        {
            using(var c = this.GenerateContext("Set_NormalizedEmail"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { 
                    Pseudonym ="ihazname", 
                    DisplayName = "IHazName"
                };
                db.Users.Add(user);
                var email = "User@nerdymishka.com";
               
                await store.SetEmailAsync(user, email);
                await db.SaveChangesAsync();
                var user1 = await db.Users.SingleOrDefaultAsync(o => o.Email == email.ToLowerInvariant());
                Assert.NotNull(user1);

                Assert.Equal(email.ToLowerInvariant(), user.Email); 
            }
        }


        [Fact]
        [Trait("tag", "unit")]
        //[Unit]

        public async void Get_NormalizedEmail()
        {
            using(var c = this.GenerateContext("Get_NormalizedEmail"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { 
                    Pseudonym ="ihazname", 
                    DisplayName = "IHazName",
                    Email = "user@nerdymishka.com"
                };
                db.Users.Add(user);
                 await db.SaveChangesAsync();
                var email = "user@nerdymishka.com";
               
                var result  =await store.GetNormalizedEmailAsync(user);
                Assert.Equal(email, result);
            }
        }


        [Fact]
        [Trait("tag", "unit")]
        //[Unit]

        public async void Get_Email()
        {
            using(var c = this.GenerateContext("Get_Email"))
            {
                var db = c.Db;
                var store = c.Store;
                var email = "user@NerdyMishka.com";
                var user = new User() { 
                    Pseudonym ="ihazname", 
                    DisplayName = "IHazName",
                    Email = email.ToLowerInvariant()
                };
                
                db.Users.Add(user);
                db.EmailAddresses.Add(new EmailAddress() {
                    UserId = user.Id,
                    Purpose = (int)EmailPurpose.Primary,
                    Value = email
                }); 
                await db.SaveChangesAsync();
               
                var result  =await store.GetEmailAsync(user);
                Assert.Equal(email, result);
            }
        }


        [Fact]
        [Trait("tag", "unit")]
        //[Unit]

        public async void Set_EmailConfirmed()
        {
            using(var c = this.GenerateContext("Set_EmailConfirmed"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { 
                    Pseudonym ="ihazname", 
                    DisplayName = "IHazName", 
                    Email = "user@nerdymishka.com",
                    IsEmailConfirmed = true
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                await store.SetEmailConfirmedAsync(user, false);
                Assert.False( user.IsEmailConfirmed);
            }
        }


        [Fact]
        [Trait("tag", "unit")]
        //[Unit]

        public async void Get_EmailConfirmed()
        {
            using(var c = this.GenerateContext("Get_EmailConfirmed"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { 
                    Pseudonym ="ihazname", 
                    DisplayName = "IHazName", 
                    Email = "user@nerdymishka.com",
                    IsEmailConfirmed = true
                };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var result =  await store.GetEmailConfirmedAsync(user);
                Assert.True(result);
            }
        } */
    }
   
}