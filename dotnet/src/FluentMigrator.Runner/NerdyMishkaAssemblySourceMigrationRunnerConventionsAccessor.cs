

using System;
using System.Collections.Generic;
using System.Linq;
using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Infrastructure;
using FluentMigrator.Runner.Initialization;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using NerdyMishka.FluentMigrator;
using FluentMigrator.Runner.VersionTableInfo;

namespace NerdyMishka.FluentMigrator.Runner
{

    public class NerdyMishkaAssemblySourceMigrationRunnerConventionsAccessor: IMigrationRunnerConventionsAccessor
    {
        private readonly Lazy<IMigrationRunnerConventions> _lazyConventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblySourceMigrationRunnerConventionsAccessor"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to instantiate the found <see cref="IMigrationRunnerConventions"/> implementation</param>
        /// <param name="assemblySource">The assemblies used to search for the <see cref="IMigrationRunnerConventions"/> implementation</param>
        public NerdyMishkaAssemblySourceMigrationRunnerConventionsAccessor(
            IServiceProvider serviceProvider,
            IAssemblySource assemblySource = null)
        {
            _lazyConventions = new Lazy<IMigrationRunnerConventions>(
                () =>
                {
                    if (assemblySource == null) {
                        return Conventions.Instance;
                    }
                        

                    var matchedType = assemblySource.Assemblies.SelectMany(a => a.GetExportedTypes())
                        .FirstOrDefault(t => typeof(IMigrationRunnerConventions).IsAssignableFrom(t));

                    if (matchedType != null)
                    {
                        if (serviceProvider == null)
                            return (IMigrationRunnerConventions)Activator.CreateInstance(matchedType);
                        return (IMigrationRunnerConventions)ActivatorUtilities.CreateInstance(serviceProvider, matchedType);
                    }

                    return Conventions.Instance;
                });
        }

        /// <inheritdoc />
        public IMigrationRunnerConventions MigrationRunnerConventions => _lazyConventions.Value;


        public class Conventions : IMigrationRunnerConventions
        {
            private Conventions()
            {
            }

            public static Conventions Instance { get; } = new Conventions();

            public Func<Type, bool> TypeIsMigration => TypeIsMigrationImpl;
            public Func<Type, bool> TypeIsProfile => TypeIsProfileImpl;
            public Func<Type, MigrationStage?> GetMaintenanceStage => GetMaintenanceStageImpl;
            public Func<Type, bool> TypeIsVersionTableMetaData => TypeIsVersionTableMetaDataImpl;

            [Obsolete]
            public Func<Type, IMigrationInfo> GetMigrationInfo => GetMigrationInfoForImpl;

            /// <inheritdoc />
            public Func<IMigration, IMigrationInfo> GetMigrationInfoForMigration => GetMigrationInfoForMigrationImpl;

            public Func<Type, bool> TypeHasTags => TypeHasTagsImpl;
            public Func<Type, IEnumerable<string>, bool> TypeHasMatchingTags => TypeHasMatchingTagsImpl;

            private static bool TypeIsMigrationImpl(Type type)
            {
                return typeof(IMigration).IsAssignableFrom(type) && type.GetCustomAttributes<NerdyMishkaMigrationAttribute>().Any();
            }

            private static MigrationStage? GetMaintenanceStageImpl(Type type)
            {
                if (!typeof(IMigration).IsAssignableFrom(type))
                    return null;

                var attribute = type.GetCustomAttribute<MaintenanceAttribute>();
                return attribute?.Stage;
            }

            private static bool TypeIsProfileImpl(Type type)
            {
                return typeof(IMigration).IsAssignableFrom(type) && type.GetCustomAttributes<ProfileAttribute>().Any();
            }

            private static bool TypeIsVersionTableMetaDataImpl(Type type)
            {
                return typeof(INerdyMishkaVersionTableMetaData).IsAssignableFrom(type) && type.GetCustomAttributes<VersionTableMetaDataAttribute>().Any();
            }

            private static IMigrationInfo GetMigrationInfoForMigrationImpl(IMigration migration)
            {
                var migrationType = migration.GetType();
                var migrationAttribute = migrationType.GetCustomAttribute<NerdyMishkaMigrationAttribute>();
                var migrationInfo = new NerdyMishkaMigrationInfo(
                    migrationAttribute.Version,
                    migrationAttribute.Module, 
                    migrationAttribute.Description, 
                    migrationAttribute.TransactionBehavior, 
                    migrationAttribute.BreakingChange, () => migration);

                foreach (var traitAttribute in migrationType.GetCustomAttributes<MigrationTraitAttribute>(true))
                    migrationInfo.AddTrait(traitAttribute.Name, traitAttribute.Value);

                return migrationInfo;
            }

            private IMigrationInfo GetMigrationInfoForImpl(Type migrationType)
            {
                var migration = (IMigration) Activator.CreateInstance(migrationType);
                return GetMigrationInfoForMigration(migration);
            }

            private static bool TypeHasTagsImpl(Type type)
            {
                return type.GetCustomAttributes<TagsAttribute>(true).Any();
            }

            private static bool TypeHasMatchingTagsImpl(Type type, IEnumerable<string> tagsToMatch)
            {
                var tags = type.GetCustomAttributes<TagsAttribute>(true).ToList();
                var matchTagsList = tagsToMatch.ToList();

                if (tags.Count != 0 && matchTagsList.Count == 0)
                    return false;

                var tagNamesForAllBehavior = tags.Where(t => t.Behavior == TagBehavior.RequireAll).SelectMany(t => t.TagNames).ToArray();
                if (tagNamesForAllBehavior.Any() && matchTagsList.All(t => tagNamesForAllBehavior.Any(t.Equals)))
                {
                    return true;
                }

                var tagNamesForAnyBehavior = tags.Where(t => t.Behavior == TagBehavior.RequireAny).SelectMany(t => t.TagNames).ToArray();
                if (tagNamesForAnyBehavior.Any() && matchTagsList.Any(t => tagNamesForAnyBehavior.Any(t.Equals)))
                {
                    return true;
                }

                return false;
            }
        }
    }
}