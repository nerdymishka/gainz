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
        [Fact]
        public async void Find_UserByEmail()
        {
            using(var c = this.GenerateContext("Find_UserByEmail"))
            {
                var db = c.Db;
                var store = c.Store;
                
                var user = new User() { Pseudonym ="ihazname", DisplayName = "IHazName", Email = "user@nerdymishka.com" };
                db.Users.Add(user);
                await db.SaveChangesAsync();

                var result =  await store.FindByEmailAsync("user@nerdymishka.com");
                Assert.Equal(result.Id, user.Id);
                Assert.Equal(result.Pseudonym, user.Pseudonym);
            }
        }

        [Fact]
        public async void Set_Email()
        {
            using(var c = this.GenerateContext("Set_Email"))
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

                var email1 = await db.EmailAddresses
                    .SingleOrDefaultAsync(
                        o => o.UserId == user1.Id && 
                        o.Purpose == EmailPurpose.Primary);

                Assert.NotNull(email1);

                Assert.Equal(email.ToLowerInvariant(), user.Email);
                Assert.Equal(email, email1.Value);
            }
        }


        [Fact]
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
                    Purpose = EmailPurpose.Primary,
                    Value = email
                }); 
                await db.SaveChangesAsync();
               
                var result  =await store.GetEmailAsync(user);
                Assert.Equal(email, result);
            }
        }


        [Fact]
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
        }
    }
}