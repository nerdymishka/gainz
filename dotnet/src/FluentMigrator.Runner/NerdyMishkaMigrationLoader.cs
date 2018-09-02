using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentMigrator;
using FluentMigrator.Exceptions;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;


using Microsoft.Extensions.Options;

namespace NerdyMishka.FluentMigrator.Runner 
{

public class NerdyMishkaMigrationInformationLoader : IMigrationInformationLoader
{
    private readonly IReadOnlyCollection<string> tagsToMatch;

#pragma warning disable 618
    private readonly IMigrationSource source;
#pragma warning restore 618

    private SortedList<long, IMigrationInfo> migrationInfos;

      


        public NerdyMishkaMigrationInformationLoader(
#pragma warning disable 618
            IMigrationSource source,
#pragma warning restore 618
            IOptionsSnapshot<TypeFilterOptions> filterOptions,
            IMigrationRunnerConventions conventions,
            IOptions<RunnerOptions> runnerOptions)
        {
            this.source = source;
            Namespace = filterOptions.Value.Namespace;
            LoadNestedNamespaces = filterOptions.Value.NestedNamespaces;
            Conventions = conventions;

            #if NET45 || NET451 || NET452 
                this.tagsToMatch = runnerOptions.Value.Tags ?? new string[0];
            #else  
                this.tagsToMatch = runnerOptions.Value.Tags ?? Array.Empty<string>();
            #endif 
        }

        public IMigrationRunnerConventions Conventions { get; }

        [Obsolete]
        public IAssemblyCollection Assemblies { get; }

        public string Namespace { get; }

        public bool LoadNestedNamespaces { get; }

        [Obsolete]
        public IEnumerable<string> TagsToMatch => this.tagsToMatch;

        public SortedList<long, IMigrationInfo> LoadMigrations()
        {
            return this.LoadMigrations(null);
        }

        public SortedList<long, IMigrationInfo> LoadMigrations(string module)
        {
            if (this.migrationInfos != null)
            {
                if (this.migrationInfos.Count == 0)
                    throw new MissingMigrationsException();
                return this.migrationInfos;
            }

            this.migrationInfos = new SortedList<long, IMigrationInfo>();
            var migrationInfos = FindMigrations(
                this.source, 
                this.Conventions, 
                this.Namespace,
                this.LoadNestedNamespaces, 
                this.tagsToMatch,
                module);

            foreach (var migrationInfo in migrationInfos)
            {
                if (this.migrationInfos.ContainsKey(migrationInfo.Version))
                {
                    throw new DuplicateMigrationException($"Duplicate migration version {migrationInfo.Version}.");
                }

                this.migrationInfos.Add(migrationInfo.Version, migrationInfo);
            }

            if (this.migrationInfos.Count == 0)
                throw new MissingMigrationsException();

            return this.migrationInfos;
        }


        private static IEnumerable<IMigrationInfo> FindMigrations(
#pragma warning disable 618
            IMigrationSource source,
#pragma warning restore 618
            IMigrationRunnerConventions conventions,
            string @namespace = null,
            bool loadNestedNamespaces = true,
            IReadOnlyCollection<string> tagsToMatch = null,
            string module = null)
        {
            if(module != null)
                module = module.ToLowerInvariant();

            bool IsMatchingMigration(Type type)
            {
                if (!type.IsInNamespace(@namespace, loadNestedNamespaces))
                    return false;
                if (!conventions.TypeIsMigration(type))
                    return false;

                // if empty load all
                if(!string.IsNullOrWhiteSpace(module)) {
                    var attr = (NerdyMishkaMigrationAttribute)type.GetCustomAttribute(typeof(NerdyMishkaMigrationAttribute));
                    if(attr.Module.ToLowerInvariant() != module) {
                        return false;
                    }
                }
                
                return conventions.TypeHasMatchingTags(type, tagsToMatch)
                 || (tagsToMatch.Count == 0 && !conventions.TypeHasTags(type))
                 || !conventions.TypeHasTags(type);
            }

            IReadOnlyCollection<IMigration> migrations;

            if (source is IFilteringMigrationSource filteringSource)
            {
                migrations = filteringSource.GetMigrations(IsMatchingMigration).ToList();
            }
            else
            {
                migrations =
                    (from migration in source.GetMigrations()
                     where IsMatchingMigration(migration.GetType())
                     select migration).ToList();
            }

            if (migrations.Count == 0)
            {
                throw new MissingMigrationsException("No migrations found");
            }

            var migrationInfos = migrations
                .Select(conventions.GetMigrationInfoForMigration)
                .ToList();

            return migrationInfos;
        }
    }
}