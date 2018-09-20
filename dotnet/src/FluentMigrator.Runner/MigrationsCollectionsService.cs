using System;

using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Generators;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Initialization.AssemblyLoader;
using FluentMigrator.Runner.Initialization.NetFramework;
using FluentMigrator.Runner.Logging;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner.VersionTableInfo;
using FluentMigrator.Validation;


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NerdyMishka.FluentMigrator;
using NerdyMishka.FluentMigrator.Runner;
using NerdyMishka.FluentMigrator.Runner.Versioning;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up the migration runner services in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class NerdyMishkaFluentMigratorServiceCollectionExtensions
    {
        /// <summary>
        /// Adds migration runner (except the DB processor specific) services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection AddNerdyMishkaFluentMigratorCore(
            this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services
                // Add support for options
                .AddOptions()

                // Add loggins support
                .AddLogging()

                // The default assembly loader factory
                .AddSingleton<AssemblyLoaderFactory>()

                // Assembly loader engines
                .AddSingleton<IAssemblyLoadEngine, AssemblyNameLoadEngine>()
                .AddSingleton<IAssemblyLoadEngine, AssemblyFileLoadEngine>()

                // Defines the assemblies that are used to find migrations, profiles, maintenance code, etc...
                .AddSingleton<IAssemblySource, AssemblySource>()

                // Configure the loader for migrations that should be executed during maintenance steps
                .AddSingleton<IMaintenanceLoader, MaintenanceLoader>()

                // Add the default embedded resource provider
                .AddSingleton<IEmbeddedResourceProvider>(sp => new DefaultEmbeddedResourceProvider(sp.GetRequiredService<IAssemblySource>().Assemblies))

                // The default set of conventions to be applied to migration expressions
                .AddSingleton<IConventionSet, DefaultConventionSet>()

                // Configure the runner conventions
                .AddSingleton<IMigrationRunnerConventionsAccessor, NerdyMishkaAssemblySourceMigrationRunnerConventionsAccessor>()
                .AddSingleton(sp => sp.GetRequiredService<IMigrationRunnerConventionsAccessor>().MigrationRunnerConventions)

                // The IStopWatch implementation used to show query timing
                .AddSingleton<IStopWatch, StopWatch>()

                // Source for migrations
#pragma warning disable 618
                .AddScoped<IMigrationSource, MigrationSource>()
                .AddScoped(
                    sp => sp.GetRequiredService<IMigrationSource>() as IFilteringMigrationSource
                     ?? ActivatorUtilities.CreateInstance<MigrationSource>(sp))
#pragma warning restore 618

                // Source for profiles
                .AddScoped<IProfileSource, ProfileSource>()

                // Configure the accessor for the version table metadata
                .AddScoped<IVersionTableMetaDataAccessor, AssemblySourceVersionTableMetaDataAccessor>()
                .AddScoped<INerdyMishkaVersionTableMetaData, NerdyMishkaMigrationsVersionTable>() 
                // Configure the default version table metadata
                .AddScoped(sp => sp.GetRequiredService<IVersionTableMetaDataAccessor>().VersionTableMetaData ?? ActivatorUtilities.CreateInstance<NerdyMishkaMigrationsVersionTable>(sp))

                // Configure the migration information loader
                .AddScoped<IMigrationInformationLoader, NerdyMishkaMigrationInformationLoader>()

                // Provide a way to get the migration generator selected by its options
                .AddScoped<IGeneratorAccessor, SelectingGeneratorAccessor>()

                // Provide a way to get the migration accessor selected by its options
                .AddScoped<IProcessorAccessor, SelectingProcessorAccessor>()

                // IQuerySchema is the base interface for the IMigrationProcessor
                .AddScoped<IQuerySchema>(sp => sp.GetRequiredService<IProcessorAccessor>().Processor)

                // The profile loader needed by the migration runner
                .AddScoped<IProfileLoader, ProfileLoader>()

                // Some services especially for the migration runner implementation
                .AddScoped<IMigrationExpressionValidator, DefaultMigrationExpressionValidator>()
                .AddScoped<MigrationValidator>()
                .AddScoped<MigrationScopeHandler>()

                // The connection string readers


                .AddScoped<IConnectionStringReader, ConfigurationConnectionStringReader>()

                // The connection string accessor that evaluates the readers
                .AddScoped<IConnectionStringAccessor, ConnectionStringAccessor>()

            
                .AddScoped<IVersionLoader>(
                    sp =>
                    {
                        var options = sp.GetRequiredService<IOptions<RunnerOptions>>();
                        var connAccessor = sp.GetRequiredService<IConnectionStringAccessor>();
                        var hasConnection = !string.IsNullOrEmpty(connAccessor.ConnectionString);
                        if (options.Value.NoConnection || !hasConnection)
                        {
                            return ActivatorUtilities.CreateInstance<ConnectionlessVersionLoader>(sp);
                        }

                        return ActivatorUtilities.CreateInstance<NerdyMishkaVersionLoader>(sp);
                    })

                // Configure the runner
                .AddScoped<IMigrationRunner, NerdyMishkaMigrationRunner>()

                // Configure the task executor
                .AddScoped<TaskExecutor>()

                // Migration context
                .AddTransient<IMigrationContext>(
                    sp =>
                    {
                        var querySchema = sp.GetRequiredService<IQuerySchema>();
                        var options = sp.GetRequiredService<IOptions<RunnerOptions>>();
                        var connectionStringAccessor = sp.GetRequiredService<IConnectionStringAccessor>();
                        var connectionString = connectionStringAccessor.ConnectionString;
#pragma warning disable 612
                        var appContext = options.Value.ApplicationContext;
#pragma warning restore 612
                        return new MigrationContext(querySchema, sp, appContext, connectionString);
                    });

            return services;
        }


         /// <summary>
        /// Configures the migration runner with NerdyMishkaMigrationRunner.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="configure">The <see cref="IMigrationRunnerBuilder"/> configuration delegate.</param>
        /// <returns>The updated service collection</returns>
        public static IServiceCollection ConfigureNerdyMishkaMigrationRunner(
            this IServiceCollection services,
            Action<IMigrationRunnerBuilder> configure, bool consoleLogger = false)
        {
            // 
            var builder = new MigrationRunnerBuilder(services);
            configure.Invoke(builder);
            if(consoleLogger)
                builder.Services
                    .AddSingleton<ILoggerProvider, FluentMigratorConsoleLoggerProvider>();

            if (builder.DanglingAssemblySourceItem != null)
            {
                builder.Services
                    .AddSingleton(builder.DanglingAssemblySourceItem);
            }

            return services;
        }


    
        private class MigrationRunnerBuilder : IMigrationRunnerBuilder
        {
            public MigrationRunnerBuilder(IServiceCollection services)
            {
                Services = services;
                DanglingAssemblySourceItem = null;
                services.AddNerdyMishkaFluentMigratorCore();
            }

            /// <inheritdoc />
            public IServiceCollection Services { get; }

            /// <inheritdoc />
            public IAssemblySourceItem DanglingAssemblySourceItem { get; set; }
        }

        private class ConnectionlessProcessorAccessor : IProcessorAccessor
        {
            public ConnectionlessProcessorAccessor(IServiceProvider serviceProvider)
            {
                Processor = ActivatorUtilities.CreateInstance<ConnectionlessProcessor>(serviceProvider);
            }

            /// <inheritdoc />
            public IMigrationProcessor Processor { get; }
        }
    }
}