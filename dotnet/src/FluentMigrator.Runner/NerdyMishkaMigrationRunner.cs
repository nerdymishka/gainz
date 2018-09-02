using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NerdyMishka.FluentMigrator.Runner.Versioning;
using MigrationRunner = FluentMigrator.Runner.MigrationRunner;

namespace NerdyMishka.FluentMigrator.Runner
{
    public class NerdyMishkaMigrationRunner : MigrationRunner
    {
        private readonly MigrationScopeHandler migrationScopeHandler;
        private readonly MigrationValidator migrationValidator;
        private ILogger logger;
        private IStopWatch stopWatch;
        private readonly ProcessorOptions processorOptions;

        private readonly RunnerOptions options;

        private IServiceProvider serviceProvider;

        private List<Exception> caughtExceptions;

        private Lazy<IVersionLoader> lazyVersionLoader;

        private NerdyMishkaVersionLoader versionLoader;

  

        [Obsolete]
#pragma warning disable 612
        private readonly IAssemblyCollection migrationAssemblies;
#pragma warning restore 612

        private bool AllowBreakingChanges =>
#pragma warning disable 612
            this.options?.AllowBreakingChange ?? RunnerContext?.AllowBreakingChange ?? false;
#pragma warning restore 612

        public NerdyMishkaMigrationRunner(
            IOptions<RunnerOptions> options, 
            IOptionsSnapshot<ProcessorOptions> processorOptions, 
            IProfileLoader profileLoader, 
            IProcessorAccessor processorAccessor, 
            IMaintenanceLoader maintenanceLoader, 
            IMigrationInformationLoader migrationLoader, 
            ILogger<MigrationRunner> logger, 
            IStopWatch stopWatch, 
            IMigrationRunnerConventionsAccessor migrationRunnerConventionsAccessor, 
            IAssemblySource assemblySource, 
            MigrationValidator migrationValidator, 
            IServiceProvider serviceProvider) : 
                base(
                    options,
                    processorOptions,
                    profileLoader,
                    processorAccessor,
                    maintenanceLoader,
                    migrationLoader,
                    logger,
                    stopWatch,
                    migrationRunnerConventionsAccessor,
                    assemblySource,
                    migrationValidator,
                    serviceProvider)
        {

            this.options = options.Value;
            this.logger = logger;
            this.stopWatch = stopWatch;

            this.processorOptions = processorOptions.Value;

            this.migrationScopeHandler = new MigrationScopeHandler(Processor);
            this.migrationValidator = migrationValidator;  
            this.serviceProvider = serviceProvider;
            #pragma warning disable 612
#pragma warning disable 618
            this.migrationAssemblies = new AssemblyCollectionService(assemblySource);
#pragma warning restore 618
#pragma warning restore 612

            this.lazyVersionLoader =  new Lazy<IVersionLoader>(serviceProvider.GetRequiredService<IVersionLoader>);
        }

        public new IReadOnlyList<Exception> CaughtExceptions => this.caughtExceptions;

        public NerdyMishkaVersionLoader NerdyMishkaVersionLoader
        {
            get{
                if(this.versionLoader != null)
                    return this.versionLoader;

                this.versionLoader = (NerdyMishkaVersionLoader)this.lazyVersionLoader.Value;
                return this.versionLoader;
            }
            set{ this.versionLoader = value; }
        }

        public new IVersionLoader VersionLoader 
        {
            get => this.NerdyMishkaVersionLoader;
            set => this.NerdyMishkaVersionLoader = (NerdyMishkaVersionLoader)value;
        }


        public override void ApplyMigrationUp(IMigrationInfo migrationInfo, bool useTransaction)
        {
            var name = migrationInfo.GetName();

            
            this.logger.LogHeader($"{name} migrating");

            this.stopWatch.Start();

            using (var scope = this.migrationScopeHandler.CreateOrWrapMigrationScope(useTransaction))
            {
                try
                {
                    if (migrationInfo.IsAttributed() && migrationInfo.IsBreakingChange &&
                        !this.processorOptions.PreviewOnly && !this.AllowBreakingChanges)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                "The migration {0} is identified as a breaking change, and will not be executed unless the necessary flag (allow-breaking-changes|abc) is passed to the runner.",
                                migrationInfo.GetName()));
                    }

                    ExecuteMigration(migrationInfo.Migration, (m, c) => m.GetUpExpressions(c));

                    if (migrationInfo.IsAttributed())
                    {
                        var info = (INerdyMishkaMigrationInfo)migrationInfo;
                        this.NerdyMishkaVersionLoader.UpdateVersionInfo(
                            info.Version,
                            info.Description ?? info.Migration.GetType().Name,
                            info.Module
                        );
                    }

                    scope.Complete();
                }
                catch
                {
                    if (useTransaction && scope.IsActive)
                    {
                        scope.Cancel();  // SQLAnywhere needs explicit call to rollback transaction
                    }

                    throw;
                }

                this.stopWatch.Stop();

                this.logger.LogSay($"{name} migrated");
                this.logger.LogElapsedTime(this.stopWatch.ElapsedTime());
            }
        }


        private MigrationStatus GetStatus(KeyValuePair<long, IMigrationInfo> migration, long currentVersion)
        {
            MigrationStatus status;
            var info = (INerdyMishkaMigrationInfo)migration.Value;
            var versionInfo = this.NerdyMishkaVersionLoader.NerdyMishkaVersionInfo;

            if (migration.Key == currentVersion)
            {
                status = MigrationStatus.Current;
            }
            else if (versionInfo.HasAppliedMigration(info.Module, info.Version))
            {
                status = MigrationStatus.Applied;
            }
            else
            {
                status = MigrationStatus.NotApplied;
            }

            if (migration.Value.IsBreakingChange)
            {
                status |= MigrationStatus.Breaking;
            }

            return status;
        }

        public void MigrateTo(
            long version = long.MaxValue, 
            string module = null, 
            bool useAutomaticTransactionManagement = true) {

            this.NerdyMishkaVersionLoader.LoadVersionInfo();
            var versionInfo = (INerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;

            var currentVersion = versionInfo.Latest(module);
            if(currentVersion == version)
                return;

            if(currentVersion < version)
                this.MigrateUp(version, module, useAutomaticTransactionManagement);
            else 
                this.MigrateDown(version, module, useAutomaticTransactionManagement);
        }

                /// <inheritdoc />
        public new void MigrateUp()
        {
            MigrateUp(true);
        }

        /// <summary>
        /// Apply migrations
        /// </summary>
        /// <param name="useAutomaticTransactionManagement"><c>true</c> if automatic transaction management should be used</param>
        public new void MigrateUp(bool useAutomaticTransactionManagement)
        {
            MigrateUp(long.MaxValue, null, useAutomaticTransactionManagement);
        }

        public void MigrateUp(string module)
        {
            this.MigrateUp(long.MaxValue, module, true);
        }

        /// <inheritdoc />
        public void MigrateUp(long targetVersion, string module = null)
        {
            MigrateUp(targetVersion, module, true);
        }

        /// <summary>
        /// Apply migrations up to the given <paramref name="targetVersion"/>
        /// </summary>
        /// <param name="targetVersion">The target migration version</param>
        /// <param name="useAutomaticTransactionManagement"><c>true</c> if automatic transaction management should be used</param>
        public void MigrateUp(long targetVersion, string module = null, bool useAutomaticTransactionManagement = true)
        {
            var migrationInfos = GetUpMigrationsToApply(targetVersion, module);

            using (IMigrationScope scope = this.migrationScopeHandler.CreateOrWrapMigrationScope(useAutomaticTransactionManagement && TransactionPerSession))
            {
                try
                {
                    ApplyMaintenance(MigrationStage.BeforeAll, useAutomaticTransactionManagement);

                    foreach (var migrationInfo in migrationInfos)
                    {
                        ApplyMaintenance(MigrationStage.BeforeEach, useAutomaticTransactionManagement);
                        ApplyMigrationUp(migrationInfo, useAutomaticTransactionManagement && migrationInfo.TransactionBehavior == TransactionBehavior.Default);
                        ApplyMaintenance(MigrationStage.AfterEach, useAutomaticTransactionManagement);
                    }

                    ApplyMaintenance(MigrationStage.BeforeProfiles, useAutomaticTransactionManagement);

                    ApplyProfiles();

                    ApplyMaintenance(MigrationStage.AfterAll, useAutomaticTransactionManagement);

                    scope.Complete();
                }
                catch
                {
                    if (scope.IsActive)
                    {
                        scope.Cancel();  // SQLAnywhere needs explicit call to rollback transaction
                    }

                    throw;
                }
            }

            VersionLoader.LoadVersionInfo();
        }

        private IEnumerable<IMigrationInfo> GetUpMigrationsToApply(long version, string module = null)
        {
            var NerdyMishkaLoader = (NerdyMishkaMigrationInformationLoader)this.MigrationLoader;
            var migrations = NerdyMishkaLoader.LoadMigrations(module);

            return from pair in migrations
                   where IsMigrationStepNeededForUpMigration(pair.Key, pair.Value)
                   select pair.Value;
        }

        private bool IsMigrationStepNeededForUpMigration(long targetVersion, IMigrationInfo migration)
        {
            var info = (INerdyMishkaMigrationInfo)migration;
            var versionInfo = (INerdyMishkaVersionInfo)NerdyMishkaVersionLoader.VersionInfo;

            if (info.Version <= targetVersion && !versionInfo.HasAppliedMigration(info.Module, info.Version))
            {
                return true;
            }
            return false;
        }


        public new void MigrateDown(long targetVersion)
        {
            MigrateDown(targetVersion, null, true);
        }

        public void MigrateDown(long targetVersion, string module)
        {
            this.MigrateDown(targetVersion, module);
        }

        /// <summary>
        /// Revert migrations down to the given <paramref name="targetVersion"/>
        /// </summary>
        /// <param name="targetVersion">The target version that should become the last applied migration version</param>
        /// <param name="useAutomaticTransactionManagement"><c>true</c> if automatic transaction management should be used</param>
        public void MigrateDown(long targetVersion, string module, bool useAutomaticTransactionManagement)
        {
            var migrationInfos = GetDownMigrationsToApply(targetVersion, module);

            using (IMigrationScope scope = this.migrationScopeHandler.CreateOrWrapMigrationScope(useAutomaticTransactionManagement && TransactionPerSession))
            {
                try
                {
                    foreach (var migrationInfo in migrationInfos)
                    {
                        ApplyMigrationDown(migrationInfo, useAutomaticTransactionManagement && migrationInfo.TransactionBehavior == TransactionBehavior.Default);
                    }

                    ApplyProfiles();

                    scope.Complete();
                }
                catch
                {
                    if (scope.IsActive)
                    {
                        scope.Cancel();  // SQLAnywhere needs explicit call to rollback transaction
                    }

                    throw;
                }
            }

            this.NerdyMishkaVersionLoader.LoadVersionInfo();
        }

        private IEnumerable<IMigrationInfo> GetDownMigrationsToApply(long targetVersion, string module = null)
        {
            var NerdyMishkaLoader = (NerdyMishkaMigrationInformationLoader)this.MaintenanceLoader;
            var migrations = NerdyMishkaLoader.LoadMigrations(module);

            var migrationsToApply = (from pair in migrations
                                     where IsMigrationStepNeededForDownMigration(pair.Key, pair.Value)
                                     select pair.Value);

            return migrationsToApply.OrderByDescending(x => x.Version);
        }


        private bool IsMigrationStepNeededForDownMigration(long targetVersion, IMigrationInfo migration)
        {
            var info = (INerdyMishkaMigrationInfo)migration;
            var versionInfo = (INerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;

            if (info.Version > targetVersion && versionInfo.HasAppliedMigration(info.Module, info.Version))
            {
                return true;
            }
            return false;

        }

        public new bool HasMigrationsToApplyUp(long? version = null)
        {
            return this.HasMigrationsToApplyUp(version, null);
        }

        /// <inheritdoc />
        public bool HasMigrationsToApplyUp(long? version = null, string module = null)
        {
            if (version.HasValue)
            {
                return GetUpMigrationsToApply(version.Value, module).Any();
            }

            var NerdyMishkaLoader = (NerdyMishkaMigrationInformationLoader)this.MigrationLoader;
            var versionInfo = (INerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;
            return NerdyMishkaLoader.LoadMigrations(module).Any(mi => 
                versionInfo.HasAppliedMigration(module, mi.Key));
        }


        /// <inheritdoc />
        public bool HasMigrationsToApplyDown(long version, string module = null)
        {
            return GetDownMigrationsToApply(version, module).Any();
        }

        public new bool HasMigrationsToApplyDown(long version)
        {
            return GetDownMigrationsToApply(version, null).Any();
        }

        /// <inheritdoc />
        public  new bool HasMigrationsToApplyRollback()
        {
            var versionInfo = (NerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;
            return versionInfo.AppliedModuleMigrations.Any();;
        }

        public bool HasMigrationsToApplyRollback(string module)
        {
            var versionInfo = (NerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;
            return versionInfo.AppliedModuleMigrations.Any(o => o.Item1 == module);
        }

        public new void ListMigrations()
        {
            this.ListMigrations(null);
        }

        public void ListMigrations(string module)
        {
            var currentVersionInfo = (INerdyMishkaVersionInfo)this.NerdyMishkaVersionLoader.VersionInfo;
            var currentVersion = currentVersionInfo.Latest();

            this.logger.LogHeader("Migrations");
            var NerdyMishkaMigrationLoader = (NerdyMishkaMigrationInformationLoader)MigrationLoader;

            foreach (var migration in NerdyMishkaMigrationLoader.LoadMigrations(module))
            {
                var info = (INerdyMishkaMigrationInfo)migration.Value;
                var migrationName = migration.Value.GetName();
                var moduleName = info.Module;  
                var status = GetStatus(migration, currentVersion);
                var statusString = string.Join(", ", GetStatusStrings(status));
                var message = $"{moduleName}-{migrationName}{(string.IsNullOrEmpty(statusString) ? string.Empty : $" ({statusString})")}";

                var isCurrent = (status & MigrationStatus.AppliedMask) == MigrationStatus.Current;
                var isBreaking = (status & MigrationStatus.Breaking) == MigrationStatus.Breaking;
                if (isCurrent || isBreaking)
                {
                    this.logger.LogEmphasized(message);
                }
                else
                {
                    this.logger.LogSay(message);
                }
            }
        }

        private IEnumerable<string> GetStatusStrings(MigrationStatus status)
        {
            switch (status & MigrationStatus.AppliedMask)
            {
                case MigrationStatus.Applied:
                    break;
                case MigrationStatus.Current:
                    yield return "current";
                    break;
                default:
                    yield return "not applied";
                    break;
            }

            if ((status & MigrationStatus.Breaking) == MigrationStatus.Breaking)
            {
                yield return "BREAKING";
            }
        }

        public override void ApplyMigrationDown(IMigrationInfo migrationInfo, bool useTransaction)
        {
            if (migrationInfo == null)
            {
                throw new ArgumentNullException(nameof(migrationInfo));
            }

            var name = migrationInfo.GetName();
            this.logger.LogHeader($"{name} reverting");

            this.stopWatch.Start();

            using (var scope = this.migrationScopeHandler.CreateOrWrapMigrationScope(useTransaction))
            {
                try
                {
                    ExecuteMigration(migrationInfo.Migration, (m, c) => m.GetDownExpressions(c));
                    if (migrationInfo.IsAttributed())
                    {
                        var info = (INerdyMishkaMigrationInfo)migrationInfo;

                        this.NerdyMishkaVersionLoader.DeleteVersion(
                            info.Version,
                            info.Module
                        );
                    }

                    scope.Complete();
                }
                catch
                {
                    if (useTransaction && scope.IsActive)
                    {
                        scope.Cancel();  // SQLAnywhere needs explicit call to rollback transaction
                    }

                    throw;
                }

                this.stopWatch.Stop();

                this.logger.LogSay($"{name} reverted");
                this.logger.LogElapsedTime(this.stopWatch.ElapsedTime());
            }
        }

        private void ExecuteMigration(IMigration migration, Action<IMigration, IMigrationContext> getExpressions)
        {
            this.caughtExceptions = new List<Exception>();

            MigrationContext context;

            if (this.serviceProvider == null)
            {
#pragma warning disable 612
                context = new MigrationContext(
                    this.Processor, 
                    this.migrationAssemblies, 
                    this.RunnerContext?.ApplicationContext, 
                    this.Processor.ConnectionString);
#pragma warning restore 612
            }
            else
            {
                var connectionStringAccessor = this.serviceProvider.GetRequiredService<IConnectionStringAccessor>();
                context = new MigrationContext(
                    Processor,
                    this.serviceProvider,
#pragma warning disable 612
                    this.options?.ApplicationContext ?? RunnerContext?.ApplicationContext,
#pragma warning restore 612
                    connectionStringAccessor.ConnectionString);
            }

            getExpressions(migration, context);

            this.migrationValidator.ApplyConventionsToAndValidateExpressions(migration, context.Expressions);
            this.ExecuteExpressions(context.Expressions);
        }

         [Flags]
        private enum MigrationStatus
        {
            Applied = 0,
            Current = 1,
            NotApplied = 2,
            AppliedMask = 3,
            Breaking = 4,
        }
    }

    
}