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
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Nexus.Services
{
    public class OperationalEnvironmentService
    {
        private NexusDbContext db;

        private ResourceService resourceService;

        public OperationalEnvironmentService(NexusDbContext db)
        {
            this.resourceService = new ResourceService(db);
            this.db = db;
        }

        public async Task<OperationalEnvironment> FindOne(
            string name,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = name.ToLower();
            var record = await this.db.OperationalEnvironments
                .SingleOrDefaultAsync(o => o.Name == name || o.Alias == lowered, cancellationToken)
                .ConfigureAwait(false);

            return Map(record);
        }

        public async Task<OperationalEnvironment> FindOne(
            int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
         
            var record = await this.db.OperationalEnvironments
                .SingleOrDefaultAsync(o => id == o.Id, cancellationToken)
                .ConfigureAwait(false);

            return Map(record);
        }

        public async Task<OperationalEnvironment> SaveAsync(
            OperationalEnvironment environment,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            OperationalEnvironmentRecord record = null;
            var id = 0;
            bool added = false;
            
            if(environment.Id.HasValue && environment.Id.Value > 0)
            {
                id = environment.Id.Value;
                record = await this.db.OperationalEnvironments
                    .SingleOrDefaultAsync(o => o.Id == id)
                    .ConfigureAwait(false);

            }

            if(record == null)
            {
                record = new OperationalEnvironmentRecord() {
                    Id = id,
                };
                var k = await this.resourceService
                            .GetOrAddKindAsync<OperationalEnvironmentRecord>(cancellationToken)
                            .ConfigureAwait(false);

                record.Resource = new ResourceRecord() {
                    KindId = k.Id
                };

                await this.db.OperationalEnvironments.AddAsync(record);
                await this.db.Resources.AddAsync(record.Resource);

                added = true;
            }

            record.Name = environment.Name;
            record.UriPath = environment.UriPath;
            record.Alias = environment.Alias;
            record.Description = environment.Description;
            
            await this.db
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            if(added)
            {
                record.Resource.RowId = record.Id;

                await this.db
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

           

            return Map(record);
        }


        private static OperationalEnvironment Map(OperationalEnvironmentRecord record)
        {
            return new OperationalEnvironment() {
                Id = record.Id,
                Name = record.Name,
                Description = record.Description,
                Alias = record.Alias,
                UriPath = record.UriPath
            };
        }
    }
}