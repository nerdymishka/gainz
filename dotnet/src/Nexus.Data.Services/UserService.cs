using Microsoft.Extensions.Logging;
using Nexus.Api;
using Nexus.Data;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nexus.Services
{
    public class UserResourceService 
    {
        private NexusDbContext db;
        private ILogger<AdminResourceService> logger;


        public async Task<ApiActionResponse<SimpleUserRegistrationResponse>> SimpleRegisterAsync(
            SimpleUserRegistrationRequest user)
        {
            var count = await this.db.Users
                .Where(o => o.Username == user.Name)
                .CountAsync();

            if(count > 0)
            {
                Response.Fail<SimpleUserRegistrationResponse>("User name already exists.",
                    new SimpleUserRegistrationResponse() {
                    Name = user.Name,
                    DisplayName = user.DisplayName,
                });
            }

            return null;
                
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
        public bool IsBanned { get; set; }

        public bool GenerateApiKey { get; set; }

        public bool IsAdmin { get; set; }
    }

    public class SimpleUserRegistrationResponse : SimpleUserRegistrationBase
    {
        public string ApiKey { get; set; }
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