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
    public class ConfigurationService
    {
        private NexusDbContext db;

        private ResourceService resourceService;

        private CompositeKey compositeKey;

        public ConfigurationService(NexusDbContext dbContext, CompositeKey key)
        {
            this.db = dbContext;
            this.compositeKey = key;
            this.resourceService = new ResourceService(dbContext);
        }

        public async Task<string[]> AllConfigurationFileNamesAsync(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = await this.db.ConfigurationFiles
                .SelectAsync(o => o.UriPath, cancellationToken)
                .ConfigureAwait(false);

            return query.ToArray();
        }

        public async Task<ConfigurationFile> FindOne(string uriPath)
        {
            var file = await this.db.ConfigurationFiles
                .Include(cf => cf.ConfigurationSet)
                .Include(cf => cf.User)
                .SingleOrDefaultAsync(o => o.UriPath == uriPath)
                .ConfigureAwait(false);

            return this.Map(file);
        }

        public async Task<ConfigurationFile> FindOne(int id)
        {
            var file = await this.db.ConfigurationFiles
                .Include(cf => cf.ConfigurationSet)
                .Include(cf => cf.User)
                .SingleOrDefaultAsync(o => o.Id == id)
                .ConfigureAwait(false);

            return this.Map(file);
        }

        public async Task<ConfigurationSet> FindSetByIdAsync(
            int id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var set = await this.db.ConfigurationSets
                .Include(o => o.OperationalEnvironment)
                .SingleOrDefaultAsync(o => o.Id == id, cancellationToken)
                .ConfigureAwait(false);

       
            if(set == null)
                return null;

            return this.Map(set);
        }

         public async Task<ConfigurationSet> FindSetByNameAsync(
             string name,
             CancellationToken cancellationToken = default(CancellationToken))
        {
            var set = await this.db.ConfigurationSets
                .Include(o => o.OperationalEnvironment)
                .SingleOrDefaultAsync(o => o.Name == name, cancellationToken)
                .ConfigureAwait(false);

       
            if(set == null)
                return null;

            return this.Map(set);
        }

        

        public async Task<ConfigurationFile[]> FindAllBySet(
            string setName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var files = await this.db.ConfigurationFiles
                        .Include(o => o.ConfigurationSet)
                        .Include(o => o.User)
                        .Where(o => o.ConfigurationSet.Name == setName)
                        .SelectAsync(o => o, cancellationToken);

            var list = new List<ConfigurationFile>();

            foreach(var file in files)
                list.Add(
                    this.Map(file));
       
            return list.ToArray();
        }

        public async Task<ValueTuple<string, int>[]> FindLabelsByUser(
            string username,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = username.ToLower();
            var files = await this.db.ConfigurationFiles
                        .Include(o => o.ConfigurationSet)
                        .Include(o => o.User)
                        .Where(o => o.User.Name == username)
                        .SelectAsync(o => new ValueTuple<string, int>(o.UriPath, o.Id) , cancellationToken);
        
            return files.ToArray();
        }


        public async Task<ValueTuple<string, int>[]> FindLabelsBySet(
            string setName,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var files = await this.db.ConfigurationFiles
                        .Include(o => o.ConfigurationSet)
                        .Include(o => o.User)
                        .Where(o => o.ConfigurationSet.Name == setName)
                        .SelectAsync(o => new ValueTuple<string, int>(o.UriPath, o.Id) , cancellationToken);
        
            return files.ToArray();
        }

        


         public async Task<ConfigurationFile[]> FindAllByUser(
            string username,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var lowered = username.ToLower();
            var files = await this.db.ConfigurationFiles
                        .Include(o => o.ConfigurationSet)
                        .Include(o => o.User)
                        .Where(o => o.User.Name == username)
                        .SelectAsync(o => o, cancellationToken);

            var list = new List<ConfigurationFile>();

            foreach(var file in files)
                list.Add(
                    this.Map(file));
       
            return list.ToArray();
        }
        
        public async Task<ConfigurationFile> SaveAsync(
            ConfigurationFile file,
            CancellationToken cancellationToken = default(CancellationToken)) {
            
            ConfigurationFileRecord record = null;
            int id = 0;
            if(file.Id.HasValue && file.Id.Value > 0)
            {
                id = file.Id.Value;
                record = await this.db.ConfigurationFiles
                    .Include(cf => cf.ConfigurationSet)
                    .Include(cf => cf.User)
                    .SingleOrDefaultAsync(o => o.Id == id, cancellationToken)
                    .ConfigureAwait(false);

            }

            bool added = false;

            if(record == null)
            {
                record = new ConfigurationFileRecord() { Id  =  id} ;
                record.IsEncrypted = file.IsEncrypted.HasValue && file.IsEncrypted.Value;
                record.IsKeyExternal = file.IsKeyExternal.HasValue && file.IsKeyExternal.Value;
                var k = this.resourceService.GetOrAddKindAsync<ConfigurationFile>();
                record.Resource = new ResourceRecord() {
                    KindId = k.Id
                };
                await this.db.ConfigurationFiles.AddAsync(record, cancellationToken);
                await this.db.Resources.AddAsync(record.Resource, cancellationToken);
                added = true;
            }

            ConfigurationSetRecord set = null;
            if(!string.IsNullOrWhiteSpace(file.ConfigurationSetName ) && !file.ConfigurationSetId.HasValue)
            {
                set = await this.db.ConfigurationSets
                    .SingleOrDefaultAsync(o => o.Name == file.ConfigurationSetName)
                    .ConfigureAwait(false);

                if(set != null)
                {
                    file.ConfigurationSetId = set.Id;
                }
            }

            if(file.ConfigurationSetId != record.ConfigurationSetId)
            {
                record.ConfigurationSet = null;
                record.ConfigurationSetId = file.ConfigurationSetId;
                if(set != null)
                    record.ConfigurationSet = set;
            }
                
            
            UserRecord user = null;
            if(!string.IsNullOrWhiteSpace(file.Username) && !file.UserId.HasValue)
            {
                var lowered = file.Username.ToLowerInvariant();
                user = await this.db.Users
                    .SingleOrDefaultAsync(o => o.Name == lowered, cancellationToken)
                    .ConfigureAwait(false);

                if(user != null)
                {
                    file.Username = user.Name;
                    file.UserId = user.Id;
                }
            }

            if(file.UserId != record.UserId)
            {
                record.User = null;
                record.UserId = file.UserId;
                if(user != null)
                    record.User = user;
            }

          

            if(file.Base64Content != null && file.Base64Content.Length > 0)
            {
                var bytes = Convert.FromBase64String(file.Base64Content);
                if(record.IsEncrypted && !record.IsKeyExternal)
                    bytes = DataProtection.EncryptBlob(bytes, this.compositeKey);

                record.Blob = bytes;
            } else {
                record.Blob = null;
            }

            // do not update IsEncrypted || IsKeyExternal on the fly.
            record.UriPath = file.UriPath;
            record.MimeType = file.MimeType;
            record.Encoding = file.Encoding;
            record.IsTemplate = file.IsTemplate.HasValue && file.IsTemplate.Value;
            record.Description = file.Description;

            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            if(added)
            {
                record.Resource.RowId = record.Id;
                await this.db.SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            return this.Map(record);
        }

        private ConfigurationSet Map(ConfigurationSetRecord record)
        {
            return new ConfigurationSet() {
                Id = record.Id,
                Name = record.Name,
                OperationalEnvironmentId = record.OperationalEnvironmentId,
                OperationalEnvironmentName = record.OperationalEnvironment == null ?
                    null : record.OperationalEnvironment.Name
            };
        }
        
        private ConfigurationFile Map(ConfigurationFileRecord record)
        {
            byte[] content = null;
            if(record.IsEncrypted && !record.IsKeyExternal && record.Blob != null)
                content = DataProtection.DecryptBlob(record.Blob, this.compositeKey);
            else 
                content = record.Blob;

            string base64 = null;
            if(content != null && content.Length > 0)
                base64 = Convert.ToBase64String(content);
        

            var file = new ConfigurationFile() {
                Id = record.Id,
                IsEncrypted = record.IsEncrypted,
                IsKeyExternal = record.IsKeyExternal,
                IsTemplate = record.IsTemplate,
                UriPath = record.UriPath,
                Base64Content = base64,
                MimeType = record.MimeType,
                Encoding = record.Encoding,
            };

            if(record.ConfigurationSet != null)
            {
                file.ConfigurationSetId = record.ConfigurationSet.Id;
                file.ConfigurationSetName = record.ConfigurationSet.Name;
             
            } else {
                 file.ConfigurationSetId = record.ConfigurationSetId;
            } 

            if(record.User != null)
            {
                file.UserId = record.User.Id;
                file.Username = record.User.Name;
            } else {
                file.UserId = record.UserId;
            }
            

           
            return file;
        }

        public async Task<ConfigurationSet> SaveAsync(
            ConfigurationSet set, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            ConfigurationSetRecord record = null;
            int id = 0;
            if(set.Id.HasValue && set.Id.Value > 0)
            {
                id = set.Id.Value;
                record = await this.db.ConfigurationSets
                    .Include(o => o.OperationalEnvironment)
                    .SingleOrDefaultAsync(o => o.Id == id, cancellationToken)
                    .ConfigureAwait(false);

            }


            if(record == null)
            {
                record = new ConfigurationSetRecord() { Id  =  id} ;
             
                await this.db.ConfigurationSets.AddAsync(record, cancellationToken);
            }

            if(!string.IsNullOrWhiteSpace(set.OperationalEnvironmentName) && set.OperationalEnvironmentId < 1)
            {
                var name = set.OperationalEnvironmentName;
                var env = await this.db.OperationalEnvironments
                            .FirstOrDefaultAsync(o => o.Name == name || o.Alias == name)
                            .ConfigureAwait(false);

                if(env != null)
                {
                    set.OperationalEnvironmentId = env.Id;
                } else {
                    set.OperationalEnvironmentName = null;
                }
            }

            record.Name = set.Name;
            record.OperationalEnvironmentId = set.OperationalEnvironmentId;

            await this.db.SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return this.Map(record);
        }


        private static Encoding GetEncoding(string encoding)
        {
            string lowered = null;
            if(!string.IsNullOrWhiteSpace(encoding))
                lowered = encoding.ToLowerInvariant();
            
            switch(encoding)
            {
                case "ascii":
                    return System.Text.Encoding.ASCII;
                case "unicode":
                    return System.Text.Encoding.Unicode;
                case "utf-32":
                case "utf32":
                     return System.Text.Encoding.UTF32;
                case "utf-7":
                case "utf7":
                     return System.Text.Encoding.UTF7;
                case "utf-8":
                case "utf8":
                     return System.Text.Encoding.UTF8;
                default:
                    return System.Text.Encoding.UTF8;
            }
        } 
    }
}