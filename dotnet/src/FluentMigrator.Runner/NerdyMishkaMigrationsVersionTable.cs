using System;
using FluentMigrator.Expressions;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace NerdyMishka.FluentMigrator.Runner
{

    public interface INerdyMishkaVersionTableMetaData : IVersionTableMetaData
    {
        string ModuleColumnName { get; }
    }

    [VersionTableMetaData]
    public class NerdyMishkaMigrationsVersionTable : 
        INerdyMishkaVersionTableMetaData,
        ISchemaExpression
    {

        public NerdyMishkaMigrationsVersionTable(IConventionSet conventionSet, IOptions<RunnerOptions> runnerOptions)
        {

#pragma warning disable 618
#pragma warning disable 612
            ApplicationContext = runnerOptions.Value.ApplicationContext;
#pragma warning restore 612
#pragma warning restore 618
            conventionSet.SchemaConvention?.Apply(this);
        }


        public object ApplicationContext { get; set; }

        public bool OwnsSchema { get; set; }

        public virtual string SchemaName  { get ; set; } = "NerdyMishka";

        public virtual string TableName => "schema_migrations";

        public virtual string ColumnName => "version";

        public virtual string DescriptionColumnName => "description";

        public virtual string UniqueIndexName => "ux_version";

        public virtual string ModuleColumnName => "module";

        public string AppliedOnColumnName => "applied_at";
    }
}
