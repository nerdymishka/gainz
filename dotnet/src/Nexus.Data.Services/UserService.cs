using Microsoft.Extensions.Logging;
using Nexus.Api;
using Nexus.Data;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security;

using NerdyMishka.Security.Cryptography;

namespace Nexus.Services
{
    public class UserService 
    {
        private NexusDbContext db;
        private ILogger<UserService> logger;

        private IPasswordAuthenticator authenticator;


     


        public async Task<ApiActionResponse<SimpleUserRegistrationResponse>> SimpleRegisterAsync(
            SimpleUserRegistrationRequest request)
        {
            var req  = request;
            var count = await this.db.Users
                .Where(o => o.Name == req.Name)
                .CountAsync();

            if(count > 0)
            { 
                Response.Fail<SimpleUserRegistrationResponse>("User name already exists.",
                    new SimpleUserRegistrationResponse() {
                    Name = req.Name,
                    DisplayName = req.DisplayName,
                });
            }

            var response = new SimpleUserRegistrationResponse();

            var user = new UserRecord() {
                Name = req.Name,
                DisplayName = req.DisplayName,
            };

       

            if(req.GeneratePassword) {
               response.Password = this.GenerateTempPassword(user, req.IsAdmin);
            }

            bool addedToAdmin = false;
            if(req.IsAdmin) {
               
                if(!req.UseAdminRole) {
                    var groupUsers = this.AddGroups(user, GroupService.SystemAdminGroupUriPath);
                    if(groupUsers != null)
                    {
                        this.db.AddRange(groupUsers.ToArray());
                        addedToAdmin = true;
                        
                    }
                }

                if(!addedToAdmin) {
                    var roleUsers = this.AddRoles(user, RoleService.SystemAdminRoleUriPath);
                    if(roleUsers == null)
                        throw new InvalidOperationException($"Admin role {RoleService.SystemAdminRoleUriPath} is missing");

                    this.db.AddRange(roleUsers.ToArray());
                    addedToAdmin = true;
                }
            }

            if(req.GenerateApiKey) {
                var apiKey = new UserApiKeyRecord() {
                    User = user
                };

                response.ApiKey = this.GenerateApiKey(apiKey);
                
                if(addedToAdmin) {
                    var roleIds = (from r in this.db.Roles
                                    join rg in this.db.RoleGroups
                                    on r.Id equals rg.RoleId
                                    join g in this.db.Groups
                                    on rg.GroupId equals g.Id
                                where g.UriPath == GroupService.SystemAdminGroupUriPath
                                select r.Id)
                                .ToArray();
                                    
                    var userApiKeyRoles = this.AddApiKeyRoles(apiKey, roleIds);
                    if(userApiKeyRoles == null)
                        throw new InvalidOperationException($"Admin role {RoleService.SystemAdminRoleUriPath} is missing");

                    this.db.AddRange(userApiKeyRoles);
                }
            }
            
            await this.db.SaveChangesAsync();

            response.Id = user.Id;
            

            return null;
                
        }

        private IEnumerable<GroupUserRecord> AddGroups(
            UserRecord user, params GroupRecord[] groups)
        {
            if(groups == null || groups.Length == 0)
                return null;

            return this.AddGroups(user, 
                groups.Select(o => o.Id).ToArray());
        }

        private IEnumerable<GroupUserRecord> AddGroups(
            UserRecord user, params string[] groupsUriPaths)
        {
            var groupIds = this.db.Groups.Where(o => groupsUriPaths.Contains(o.UriPath))
                            .Select(o => o.Id)
                            .ToArray();

            return this.AddGroups(user, groupIds);
        }

        private IEnumerable<GroupUserRecord> AddGroups(
            UserRecord user, params int[] groupIds)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var list = new List<GroupUserRecord>();

            if(groupIds == null || groupIds.Length == 0)
                return null;

            if(user.Id > 0) {
                foreach(var groupId in groupIds) {
                    list.Add(new GroupUserRecord(){ UserId = user.Id, GroupId = groupId  });
                }
                return list;
            }

            foreach(var groupId in groupIds) {
                list.Add(new GroupUserRecord() { User = user, GroupId = groupId });
            }

            return list;
        }

        public IEnumerable<RoleUserRecord> AddRoles(
            UserRecord user, params string[] roleUriPaths)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if(roleUriPaths == null || roleUriPaths.Length == 0)
                return null;
            
            var roleIds = this.db.Roles.Where(o => roleUriPaths.Contains(o.UriPath))
                            .Select(o => o.Id)
                            .ToArray();

            return this.AddRoles(user, roleIds);
        }

        public IEnumerable<RoleUserRecord> AddRoles(
            UserRecord user, params RoleRecord[] roles)
        {
            if(roles == null || roles.Length == 0)
                return null;
            
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            return this.AddRoles(user, 
                roles.Select(o => o.Id).ToArray());
        }

        public IEnumerable<UserApiKeyRoleRecord> AddApiKeyRoles(
            UserApiKeyRecord userApiKey, params int[] roleIds)
        {
            if(userApiKey == null)
                throw new ArgumentNullException(nameof(userApiKey));

            var list = new List<UserApiKeyRoleRecord>();
            if(userApiKey.Id > 0)
            {
                foreach(var roleId in roleIds)
                {
                    list.Add(new UserApiKeyRoleRecord() { UserApiKeyId= userApiKey.Id, RoleId = roleId });
                }
                return list;
            }

            foreach(var roleId in roleIds)
            {
                list.Add(new UserApiKeyRoleRecord() { UserApiKey = userApiKey, RoleId = roleId });
            }

            return list;
        }

        public IEnumerable<RoleUserRecord> AddRoles(UserRecord user, params int[] roleIds)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var list = new List<RoleUserRecord>();
            if(user.Id > 0)
            {
                foreach(var roleId in roleIds)
                {
                    list.Add(new RoleUserRecord() { UserId = user.Id, RoleId = roleId });
                }
                return list;
            }

            foreach(var roleId in roleIds)
            {
                list.Add(new RoleUserRecord() { User = user, RoleId = roleId });
            }

            return list;
        }

        private SecureString GenerateTempPassword(UserRecord user, bool isAdmin = false)
        {
            int length = 12;
            if(isAdmin)
                length = 20;
                
            var pw = PasswordGenerator.Generate(length: length);

            var ss = new SecureString();
            foreach(char c in pw)
                ss.AppendChar(c);

        
            var bytes = System.Text.Encoding.UTF8.GetBytes(pw);

            user.Password = Convert.ToBase64String(
                this.authenticator.ComputeHash(bytes));

            Array.Clear(pw, 0, pw.Length);
            Array.Clear(bytes, 0, bytes.Length);

            return ss;
        }

        private SecureString GenerateApiKey(UserApiKeyRecord apiKey)
        {
            var pw = PasswordGenerator.Generate(length: 20);

            var ss = new SecureString();
            foreach(char c in pw)
                ss.AppendChar(c);

            
            var bytes = System.Text.Encoding.UTF8.GetBytes(pw);
            apiKey.ApiKey = Convert.ToBase64String(this.authenticator.ComputeHash(bytes));
            


            Array.Clear(pw, 0, pw.Length);
            Array.Clear(bytes, 0, bytes.Length);
            return ss;
        }

        internal protected static User Map(UserRecord record)
        {
            return new User() {
                Id = record.Id,
                Name = record.Name,
                IconUri = record.IconUri,
                DisplayName = record.DisplayName,
                ResourceId = record.Resource.Id
            };
        }
    }


    

    public class SimpleUserRegistrationBase
    {
        public int? Id { get; set; }

        public long? ResourceId { get; set; }

        public string DisplayName { get; set; }

        public string Name { get; set; }
    }

    public class SimpleUserRegistrationRequest : SimpleUserRegistrationBase
    {
        public bool GeneratePassword { get; set; }

        public bool IsBanned { get; set; }

        public bool GenerateApiKey { get; set; }

        public bool IsAdmin { get; set; }

        public bool UseAdminRole { get; set; }
    }

    public class SimpleUserRegistrationResponse : SimpleUserRegistrationBase
    {
        public SecureString Password { get; set; }

        public SecureString ApiKey { get; set; }
    }

    

    public class UserModel 
    {
        
        public int? Id { get; set; }

        public string DisplayName { get; set; }


        public string Name { get; set; }

        public bool IsBanned { get; set; }

        public long? ResourceId { get; set; }


    }
}