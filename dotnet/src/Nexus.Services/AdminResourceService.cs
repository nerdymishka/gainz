
using Nexus.Api;
using Nexus.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Nexus.Services
{
    public class AdminResourceService : IAdminResourceService
    {
        private NexusDbContext db;
        private ILogger logger;

        public AdminResourceService(NexusDbContext dbContext, ILogger logger)
        {
            if(dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

            if(logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.db = dbContext;
            this.logger = logger;
        }

        public async Task<ApiActionResponse<bool>> DeleteAsync(long[] ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryTask = new Task<List<Resource>>(() => {
                 return this.db.Resources
                    .Where(o => ids.Contains(o.Id))
                    .ToList();

            }, cancellationToken)
            .ConfigureAwait(false);
           
            var records = await queryTask;
          
            if(cancellationToken.IsCancellationRequested)
                return Response.Cancel<bool>("Resource Deletion");

            if(records.Count != ids.Length)
            {
                var missingIds = new List<long>();
                foreach(var id in ids)
                {
                    bool found = false;
                    foreach(var record in records)
                    {
                        if(record.Id == id)
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                        missingIds.Add(id);
                }
                var msg = $"The following resource records were not found {string.Join(",", missingIds)}";
                return Response.Fail<bool>(msg);
            }

            try {

                if(cancellationToken.IsCancellationRequested)
                    return Response.Cancel<bool>("Resource Deletion");
                
                db.RemoveRange(records);

                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            
            } catch(Exception ex) {
                var msg = $"Resource deletion failed for {string.Join(",", ids)}";

                if(this.logger.IsEnabled(LogLevel.Error))
                    this.logger.LogError(ex, msg);

                return Response.Fail<bool>(msg);
            }

            return Response.Ok(true);
        }

        public async Task<ApiActionResponse<ResourceModel>> FindAsync(long id, CancellationToken cancellationToken = default(CancellationToken))
        {
            Resource resource = null;
            try {
                resource = await this.db.Resources.FindAsync(new [] { id }, cancellationToken)
                    .ConfigureAwait(false);
            } catch (Exception ex) {
                if(logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, $"Retrieval Error for Resource({id}).");
            }

            if(resource == null)
                return Response.Fail<ResourceModel>($"Resource was not found for ${id}");

            return Response.Ok(
                Project(resource));
        }

        public async Task<ApiActionResponse<ResourceModel>> FindAsync(
            string uri, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Resource resource = null;
            try {
                var t = new Task<Resource>(() => {
                    return this.db.Resources.SingleOrDefault(o => o.Uri == uri);
                }, cancellationToken).ConfigureAwait(false);

                resource = await t;
            } catch (Exception ex) {
                if(logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, $"Retrieval Error for Resource({uri}).");
            }

            if(resource == null)
                return Response.Fail<ResourceModel>($"Resource was not found for ${uri}");

            return Response.Ok(Project(resource));
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

           
        public static ResourceModel Map(Resource source, ResourceModel dest)
        {
            dest.Id = source.Id;
            dest.Uri = source.Uri;
            dest.Type = source.Type;
            dest.Key = source.Key;
            dest.IsDeleted = source.IsDeleted;

            return dest;
        }

        

        public async Task<PagedApiActionResponse<ResourceModel>> ListAsync(
            int page = 1, 
            int size = 20, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = this.db.Resources.Count();
            if(page < 1)
                page = 1;

            if(size < 10)
                size = 10;

            var query = this.db.Resources.AsQueryable();
            var skip = (page - 1) * size;
            var take = size;

            if(skip > 0)
                query = query.Skip(skip);

            try {
                var queryAsync = new Task<ResourceModel[]>(() => {
                    
                    return query
                        .Take(take)
                        .Select(Project)
                        .ToArray();
                }, cancellationToken)
                .ConfigureAwait(false);

                var set = await queryAsync;

                return Response.PagedOk(set, page, size, count);

            } catch (Exception ex) {

                var msg = $"Resources Retrieval failed. Page {page} Size {size}";
                if(logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, msg);

                return Response.PagedFail<ResourceModel>(msg);
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
                    var queryTask = new Task<List<Resource>>(() => {
                            return this.db.Resources
                                .Where(o => ids.Contains(o.Id))
                                .ToList();
                        }, 
                        cancellationToken)
                        .ConfigureAwait(false);

                    updateSet = await queryTask;

                    foreach(var record in updateSet) {
                        var model = resources.SingleOrDefault(
                            o => o.Id.HasValue && o.Id.Value == record.Id);

                        if(model == null)
                            continue;

                        Map(model, record);
                    }
                }

                foreach(var model in newSet) {
                    insertSet.Add(Map(model, new Resource()));
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
                var msg = "Save Resource action failed";
                
                if(logger.IsEnabled(LogLevel.Error))
                    logger.LogError(ex, msg);

                return Response.Fail<ResourceModel[]>(msg);
            }
        }
    }
}