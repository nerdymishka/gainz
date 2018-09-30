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
using Humanizer.Inflections;
using Humanizer;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.Services
{
    public class ResourceService
    {
        private NexusDbContext db;
        private static readonly ConcurrentDictionary<Type, ResourceKindRecord> resourceKindCache = 
            new ConcurrentDictionary<Type, ResourceKindRecord>();


        public async Task<ResourceKindRecord> AddKindAsync(string name, string tableName, Type type, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(resourceKindCache.TryGetValue(type, out ResourceKindRecord kind))
                return kind;

            kind = new ResourceKindRecord() {
                Name = name,
                TableName = tableName,
                UriPath = name.Hyphenate().ToLower(),
                ClrTypeName = type.FullName
            };

            this.db.Add(kind);
            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

             if(cancellationToken.IsCancellationRequested)
                return null;

            resourceKindCache.AddOrUpdate(type, kind, (t, k) => {
                return k;
            });

            return kind;
        }

        public Task<ResourceKindRecord> GetOrAddKindAsync<T>(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.GetOrAddKindAsync(typeof(T), cancellationToken);
        }

        public async Task<ResourceKindRecord> GetOrAddKindAsync(Type type,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if(resourceKindCache.TryGetValue(type, out ResourceKindRecord kind))
                return kind;

            var t = type;
            var table = t.GetCustomAttributes(typeof(TableAttribute), true)
                            .FirstOrDefault() as TableAttribute;
            if(table == null) {
                throw new Exception("type T must have a TableAttribute");
            }

            var name = t.Name;
            if(name.EndsWith("Record"))
                name = name.Substring(0, name.Length - 7);

            name = name.Pluralize();

            var tableName = table.Name;
            if(!string.IsNullOrWhiteSpace(table.Schema))
                tableName = table.Schema + "." + tableName;

            kind = await this.AddKindAsync(name, tableName, t, cancellationToken);
            
            return kind;
        }

        public Task AddResources<T>(int[] rowIds,
            bool save = false, 
            CancellationToken cancellationToken = default(CancellationToken))
        {   
            var longRowIds = rowIds.Cast<long>().ToArray();
            return this.AddResources<T>(longRowIds, save, cancellationToken);
        }

        internal async protected Task<IList<ResourceRecord>> AddResources<T>(long[] rowIds, 
            bool save,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var k = await this.GetOrAddKindAsync<T>();

            var list = new List<ResourceRecord>();
            
            foreach(var rowId in rowIds) {
                list.Add(new ResourceRecord() {
                    Kind = k,
                    RowId = rowId
                });
            }

            await db.AddRangeAsync(list, cancellationToken);

            if(save)
            {
                await this.db.SaveChangesAsync(cancellationToken);
            }

            return list;
        }

        
        internal protected Task<ResourceRecord> AddResourceAsync<T>(int rowKey,
            bool save = false, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.AddResourceAsync<T>((long)rowKey,save, cancellationToken);
        }

        internal async protected Task<ResourceRecord> AddResourceAsync<T>(
            long rowKey,
            bool save = false, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var k = await this.GetOrAddKindAsync<T>();
        

            var resource = new ResourceRecord() {
                RowId = rowKey,
                Kind = k 
            }; 

            await db.AddAsync(resource);
            if(save)
                await this.db.SaveChangesAsync(cancellationToken);

            return resource;
        }
    }

}