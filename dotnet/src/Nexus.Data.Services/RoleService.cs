using Microsoft.Extensions.Logging;
using Nexus.Api;
using Nexus.Data;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Nexus.Services
{
    public class RoleService
    {
        private ConcurrentDictionary<string, Role> roleCache = 
            new ConcurrentDictionary<string, Role>();  

        private static string systemAdminRoleUriPath = "nexus-administrators";

        public static string SystemAdminRoleUriPath => systemAdminRoleUriPath;

        private NexusDbContext db;
        private ILogger<RoleService> logger;

        private string systemAdminRole = "nexus-admin";

        public RoleService(NexusDbContext dbContext, ILogger<RoleService> logger)
        {

        }

        public static Role Map(RoleRecord record)
        {
            return new Role() {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                ResourceId = record.ResourceId
            };
        }




        public class RolePair 
        {
            public int Id  { get;  set; }

            public string Name { get; set; }
        }
    }

}