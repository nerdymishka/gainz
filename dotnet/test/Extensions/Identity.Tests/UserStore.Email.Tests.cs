using Microsoft.EntityFrameworkCore;
using Mettle;
using NerdyMishka.Identity;
using NerdyMishka.EfCore.Identity;
using System;
using System.Linq;

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
        
        [UnitTest]
        public async void Set_NormalizedEmail(IdentityDbContext db, UserStore store)
        {
            var name = Guid.NewGuid().ToString().ToUpperInvariant();
            var email = name + "@nerdymishka.com";
            var user = new User() { 
                Pseudonym = name,
                DisplayName = name 
            };
            db.Users.Add(user);
        
            await store.SetEmailAsync(user, email);
            await db.SaveChangesAsync();
            var user1 = await db.Users
                .SingleOrDefaultAsync(o => o.Email == email.ToLowerInvariant());
            
            assert.NotNull(user1);
            assert.Equal(email.ToLowerInvariant(), user.Email); 
        }


        
        [UnitTest]
        public async void Get_NormalizedEmail(IdentityDbContext db, UserStore store)
        {
            var name = Guid.NewGuid().ToString().ToLowerInvariant();
            var email = name + "@nerdymishka.com";
            var user = new User() { 
                Pseudonym = name, 
                DisplayName = name,
                Email = email 
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var result  = await store.GetNormalizedEmailAsync(user);
            assert.Equal(email.ToLowerInvariant(), result);
        }


        [UnitTest]
        public async void Get_Email(UserStore store)
        {
            var user = await store.FindByIdAsync(1);
            assert.NotNull(user);

            var result  = await store.GetEmailAsync(user);
            assert.Equal("sg@nerdymishka.com", result);
        }


        [UnitTest]
        public async void Set_EmailConfirmed(IdentityDbContext db, UserStore store)
        {
            var name = Guid.NewGuid().ToString().ToLowerInvariant();
            var email = name + "@nerdymishka.com";
            var user = new User() { 
                Pseudonym = name, 
                DisplayName = name,
                Email = email,
                IsEmailConfirmed = false
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            assert.NotNull(user);
            await store.SetEmailConfirmedAsync(user, true);
            await store.SaveChanges();
            
            user = await store.FindByEmailAsync(email);
            assert.NotNull(user);
            assert.Ok(user.IsEmailConfirmed);
        }


        
        [UnitTest]
        public async void Get_EmailConfirmed(IdentityDbContext db, UserStore store)
        {
            var name = Guid.NewGuid().ToString().ToLowerInvariant();
            var email = name + "@nerdymishka.com";
             var user = new User() { 
                Pseudonym = name, 
                DisplayName = name,
                Email = email,
                IsEmailConfirmed = true 
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            user = await store.FindByEmailAsync(email);
            var result =  await store.GetEmailConfirmedAsync(user);
            assert.Ok(result);
        }
    }
   
}