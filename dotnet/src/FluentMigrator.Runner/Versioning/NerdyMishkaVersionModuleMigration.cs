using FluentMigrator;
using FluentMigrator.Runner.VersionTableInfo;

namespace NerdyMishka.FluentMigrator.Runner.Versioning
{
   public class NerdyMishkaVersionModuleMigration : ForwardOnlyMigration
    {
        private readonly INerdyMishkaVersionTableMetaData versionTableMetaData;

        public NerdyMishkaVersionModuleMigration(INerdyMishkaVersionTableMetaData versionTableMetaData)
        {
            this.versionTableMetaData = versionTableMetaData;
        }

        public override void Up()
        {
            this.Delete.Index(versionTableMetaData.UniqueIndexName);

            this.Create.Column(this.versionTableMetaData.ModuleColumnName)
                .OnTable(this.versionTableMetaData.TableName)
                .InSchema(this.versionTableMetaData.SchemaName)
                .AsString(500)
                .NotNullable()
                .WithDefaultValue("app");

            this.Create.UniqueConstraint(this.versionTableMetaData.UniqueIndexName)
                .OnTable(this.versionTableMetaData.TableName)
                .WithSchema(this.versionTableMetaData.SchemaName)
                .Columns(
                    this.versionTableMetaData.ModuleColumnName,
                    this.versionTableMetaData.ColumnName);
        }
        
    }
}