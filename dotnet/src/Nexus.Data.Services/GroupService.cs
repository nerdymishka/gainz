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
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace Nexus.Services
{


    public class GroupService
    {
        private NexusDbContext db;
        private ResourceService resourceService;

        private static readonly ConcurrentBag<Group> groupCache = 
            new ConcurrentBag<Group>();
        private static string systemAdminGroupUriPath = "nexus-administrators";

        public static string SystemAdminGroupUriPath => systemAdminGroupUriPath;

        public GroupService(
            NexusDbContext dbContext, 
            ResourceService resourceService) {

            this.db = dbContext;
        }

        private IQueryable<GroupRecord> Query(bool includeUser, bool includeRoles, bool includeResource = false)
        {
            

            if(includeUser && includeRoles)
            {
                return (from g in this.db.Groups
                    join gu in this.db.GroupUsers.Include(o1 => o1.User)
                    on g.Id equals gu.GroupId
                    join gr in this.db.RoleGroups.Include(o2 => o2.Role)
                    on g.Id equals gr.GroupId
                    select g);
            }

            if(includeRoles)
            {
                return (from g in this.db.Groups
                    join gr in this.db.RoleGroups.Include(o2 => o2.Role)
                    on g.Id equals gr.GroupId
                    select g);
            }


            return (from g in this.db.Groups
                    join gu in this.db.GroupUsers.Include(o1 => o1.User)
                    on g.Id equals gu.GroupId
                    select g);
        } 

        public async Task<Group> FindOneAsync(int id, 
            bool includeUsers = false,
            bool includeRoles =  false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this.Query(includeUsers, includeRoles);
            var record = await query.Where(o => o.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);
           
            var group = GroupService.Map(record);
            if(record.GroupUsers != null && record.GroupUsers.Count > 0)
                group.Users = record.GroupUsers.Select(o => UserService.Map(o.User)).ToArray();

            if(record.GroupRoles != null && record.GroupRoles.Count > 0)
                group.Roles = record.GroupRoles.Select(o => RoleService.Map(o.Role)).ToArray();
                
            return group;
        }

        public async Task<Group> FindOneAsync(
            long resourceId,
            bool includeUsers = false,
            bool includeRoles =  false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this.Query(includeUsers, includeRoles);
            var record = await query.Where(o => o.ResourceId == resourceId)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);
           
            var group = GroupService.Map(record);
            if(record.GroupUsers != null && record.GroupUsers.Count > 0)
                group.Users = record.GroupUsers.Select(o => UserService.Map(o.User)).ToArray();

            if(record.GroupRoles != null && record.GroupRoles.Count > 0)
                group.Roles = record.GroupRoles.Select(o => RoleService.Map(o.Role)).ToArray();
                
            return group;
        }

        public async Task<Group> FindOneAsync(
            string uriPath,
            bool includeUsers = false,
            bool includeRoles =  false,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(!includeUsers && !includeRoles) {
                return groupCache.SingleOrDefault(o => o.UriPath == uriPath);
            }

            var query = this.Query(includeUsers, includeRoles);
            var record = await query.Where(o => o.UriPath == uriPath)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);
           
            var group = GroupService.Map(record);
            if(record.GroupUsers != null && record.GroupUsers.Count > 0)
                group.Users = record.GroupUsers.Select(o => UserService.Map(o.User)).ToArray();

            if(record.GroupRoles != null && record.GroupRoles.Count > 0)
                group.Roles = record.GroupRoles.Select(o => RoleService.Map(o.Role)).ToArray();
                
            return group;
        }


        public async Task<bool> DeleteGroupAsync(
            int id,
            bool force, 
            CancellationToken cancellationToken = default(CancellationToken)) 
        {
            var record =await this.Query(true, true)
                            .Where(o => o.Id == id)
                            .SingleOrDefaultAsync();

            bool hasRoles =  record.GroupRoles != null && record.GroupRoles.Count > 0;
            bool hasUsers = record.GroupUsers != null && record.GroupUsers.Count > 0;
            if(!force && hasRoles || hasUsers)
            {
                throw new InvalidOperationException($"force must be true if the group has child objects");
            }

            if(force && hasUsers)
                this.db.GroupUsers.RemoveRange(record.GroupUsers);
            
            if(force && hasRoles)
                this.db.RoleGroups.RemoveRange(record.GroupRoles);

            if(force && (hasRoles || hasUsers))
                await this.db.SaveChangesAsync(cancellationToken);


            var resource = await this.db.Resources.SingleOrDefaultAsync(o => o.Id ==record.ResourceId);
          
           
            this.db.Groups.Remove(record);
            await this.db.SaveChangesAsync(cancellationToken);

            var group = groupCache.SingleOrDefault(o => o.Id == record.Id);
            if(group != null)
                groupCache.TryTake(out group);

            

            return true;
        }

        public async Task<Group> SaveGroupAsync(Group group,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var record = new GroupRecord();
            var add = true;
            if(group.Id.HasValue || group.Id.Value > 0) {
                var id = group.Id.Value;
                record = await (from g in this.db.Groups
                            join r in this.db.Resources
                            on g.ResourceId equals r.Id
                            where g.Id == id 
                            select g)
                            .SingleOrDefaultAsync();

                if(record == null)
                    throw new RecordNotFoundException("Group", group.Id.Value);

                add = false;
            } 
            
            if(record.Name != group.Name) {
                record.Name = group.Name;
                record.UriPath = group.Name.Hyphenate().ToLower();
            }
            record.Description = group.Description;
            
            if(add) {
                await this.db.AddAsync(record, cancellationToken);
            }
                
            await this.db.SaveChangesAsync();

            if(add) {
                var resource = await this.resourceService.AddResourceAsync<GroupRecord>(record.Id);
                record.Resource = resource;

                await this.db.SaveChangesAsync();
            }

         

            group = Map(record);

            var old = groupCache.SingleOrDefault(o => o.Id == record.Id);
            if(old != null)
                groupCache.TryTake(out old);

            groupCache.Add(group);
            
            return group;
        }

        
        internal protected static Group Map(GroupRecord record)
        {
            return new Group() {
                Id = record.Id,
                Name = record.Name,
                UriPath = record.UriPath,
                Description = record.Description,
                ResourceId = record.Resource.Id
            };
        }

    }

}