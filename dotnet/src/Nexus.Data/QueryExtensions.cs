using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nexus.Data
{
    public static class QueryExtensions
    {
        

        public static Task<int> CountAsync<TModel>(
            this IQueryable<TModel> query, 
            CancellationToken cancellationToken = default(CancellationToken)) {
            return Task.Run<int>(() => query.Count(), cancellationToken);
        } 

        public static Task<int> CountAsync<TModel>(
            this IQueryable<TModel> query, 
            Func<TModel, bool> predicate,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return Task.Run<int>(() => query.Count(predicate), cancellationToken);
        } 

        public static Task<IEnumerable<TProjection>> SelectAsync<TModel, TProjection>(
            this IQueryable<TModel> query,
            Func<TModel, TProjection> projection,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return Task.Run<IEnumerable<TProjection>>(() => {
                return query.Select(projection)
                    .ToList();
            }, cancellationToken);
        }

        public static Task<TModel[]> ToArrayAsync<TModel>(
            this IQueryable<TModel> query, 
            CancellationToken cancellationToken = default(CancellationToken)) {

            return Task.Run<TModel[]>(()=>{
                return query
                    .ToArray();
            }, cancellationToken);
        }

        public static Task<List<TModel>> ToListAsync<TModel>(
            this IQueryable<TModel> query,
            CancellationToken cancellationToken = default(CancellationToken)) {

            return Task.Run<List<TModel>>(() => query.ToList(), 
                cancellationToken);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page = 1, int size = 20)
        {
            if(page < 1)
                page = 1;

            if(size < 10)
                size = 10;

            var skip = (page - 1) * size;
            var take = size;

            if(skip > 0)
                query = query.Skip(skip);

            return query.Take(take);
        }
    }
}