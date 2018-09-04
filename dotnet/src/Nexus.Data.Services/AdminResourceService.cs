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
    public class AdminResourceService : IAdminResourceService
    {
        private NexusDbContext db;
        private ILogger<AdminResourceService> logger;
        private string name;

        public AdminResourceService(NexusDbContext dbContext, ILogger<AdminResourceService> logger)
        {
            if(dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.db = dbContext;
            this.logger = logger;
            this.name = "Resource";
        }

        public async Task<ApiActionResponse<bool>> DeleteAsync(long[] ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(ids == null || ids.Length == 0)
                return Response.Fail<bool>("1 or more ids are required");

            var records = await this.db.Resources
                .Where(o => ids.Contains(o.Id))
                .ToListAsync()
                .ConfigureAwait(false);
          
            if(cancellationToken.IsCancellationRequested)
                return Response.Cancel<bool>("Resource Deletion");

            if(records.Count != ids.Length)
            {
                var existingIds = records.Select(o => o.Id);
                var missingIds = ids.Except(existingIds).ToList();
                var msg = $"The following {this.name} records were not found {string.Join(",", missingIds)}";
                return Response.Fail<bool>(msg);
            }

            try {

                if(cancellationToken.IsCancellationRequested)
                    return Response.Cancel<bool>($"{this.name} Deletion");
                
                db.RemoveRange(records);

                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return Response.Ok(true);
            } catch(Exception ex) {

                var msg = $"Delete {this.name} records failed for {string.Join(",", ids)}";
                return Response.Fail<bool>(msg, logger, ex);
            }
        }

        public async Task<ApiActionResponse<ResourceModel>> FindAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            try {
              
                var resource = await this.db.Resources
                    .FindAsync(new object[] { id }, cancellationToken)
                    .ConfigureAwait(false);  

                if(resource == null)
                    return Response.Ok<ResourceModel>(null);
               
                return Response.Ok(Project(resource));
            } catch (Exception ex) {

                var msg = $"Find {this.name} failed for {id}. " + ex.Message  + " " + ex.StackTrace;
                return Response.Fail<ResourceModel>(msg, logger, ex);
            }
        }

        public async Task<ApiActionResponse<ResourceModel>> FindAsync(
            string uri, 
            CancellationToken cancellationToken = default(CancellationToken))
        { 
            try {
                var resource = await this.db.Resources
                    .SingleOrDefaultAsync(o => o.Uri == uri)
                    .ConfigureAwait(false); 

                if(resource == null)
                    return Response.Ok<ResourceModel>(null);

                return Response.Ok(Project(resource));
            } catch (Exception ex) {

                var msg = $"Find {this.name} records failed for {uri}.";
                return Response.Fail<ResourceModel>(msg, logger, ex);
            }
        }

        public static Resource Map(ResourceModel source, Resource dest)
        {
            dest.Id = source.Id.HasValue ? source.Id.Value : 0L;
            dest.Uri = source.Uri;
            dest.Key = source.Key;
            dest.Type = source.Type;
            dest.IsDeleted = source.IsDeleted;

            return dest;
        }

        public static Func<Resource, ResourceModel> Project =>  (o) => new ResourceModel() {
            Id = o.Id,
            Uri = o.Uri,
            Key = o.Key,
            Type = o.Type,
            IsDeleted = o.IsDeleted
        };


        public async Task<PagedApiActionResponse<ResourceModel>> ListAsync(
            int page = 1, 
            int size = 20, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = await this.db.Resources
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
            
            if(count == 0)
                return Response.PagedEmpty<ResourceModel>();

            try {
                var set = await this.db.Resources
                    .Page(page, size)
                    .SelectAsync(Project, cancellationToken)
                    .ConfigureAwait(false);
            
                return Response.PagedOk(set, page, size, count);

            } catch (Exception ex) {

                var msg = $"List {this.name} records failed. Page:{page} Size: {size}";
                return Response.PagedFail<ResourceModel>(msg, logger, ex);
            }
        }

        public async Task<ApiActionResponse<ResourceModel[]>> SaveAsync(
            ResourceModel[] resources, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var ids = resources
                .Where(o => o.Id.HasValue)
                .Select(o => o.Id.Value)
                .ToList();

            var newSet = resources
                .Where(o => !o.Id.HasValue)
                .ToList();

            var insertSet = new List<Resource>();
            var updateSet = new List<Resource>();

            try {

                if(ids.Count > 0)
                {
                    updateSet = await this.db.Resources
                        .Where(o => ids.Contains(o.Id))
                        .ToListAsync(cancellationToken)
                        .ConfigureAwait(false);
                  
                    foreach(var record in updateSet) {
                        var model = resources.SingleOrDefault(
                            o => o.Id.HasValue && o.Id.Value == record.Id);

                        if(model == null)
                            continue;

                        Map(model, record);
                    }
                }

                
                foreach(var model in newSet) {
                    var resource = new Resource();
                    Map(model, resource);
                    insertSet.Add(resource);
                }

                db.AddRange(insertSet);
                
                await db.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
                
                updateSet.AddRange(insertSet);

                var results = updateSet
                    .Select(Project)
                    .ToArray();

                return Response.Ok(results);
            } catch(Exception ex) {
                var msg = $"{this.name} Save action failed";            
                return Response.Fail<ResourceModel[]>(msg, logger, ex);
            }
        }
    }
}