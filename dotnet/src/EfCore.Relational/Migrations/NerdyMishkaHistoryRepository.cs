
using System;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using NerdyMishka.EfCore.Infrastructure;

namespace NerdyMishka.EfCore.Migrations
{

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>   
    /// <para>
    ///     Much of the code in HistoricalRepository was pulled from other
    ///     Ef provider projects
    /// </para>
    /// <list>
    ///    <list type="bulltet">
    ///     <listheader>
    ///         
    ///     </listheader>
    ///     <item>
    ///          [SqliteHistoryRepostory](https://github.com/aspnet/EntityFrameworkCore/blob/master/src/EFCore.Sqlite.Core/Migrations/Internal/SqliteHistoryRepository.cs)
    ///     </item>
    ///     <item>
    ///          [SqlServerHistoryRepostory](https://github.com/aspnet/EntityFrameworkCore/blob/master/src/EFCore.SqlServer/Migrations/Internal/SqlServerHistoryRepository.cs)
    ///     </item>
    /// </list>     
    /// </remarks>
    public class NerdyMishkaHistoryRepository : HistoryRepository
    {
        private RelationalDbTypes dbType;
        private NerdyMishkaOptionsExtension hOpts;

        public enum RelationalDbTypes
        {
            Sqlite,
            SqlServer,
            MySql,
            Postgres
        }

        public NerdyMishkaHistoryRepository(HistoryRepositoryDependencies dependencies) : base(dependencies)
        {
            
            this.hOpts = NerdyMishkaOptionsExtension.Extract(dependencies.Options) ?? 
                new NerdyMishkaOptionsExtension();

            
           
            TableName = hOpts.MigrationTableName;
            TableSchema = hOpts.MigrationSchemaName ?? hOpts.DefaultSchemaName;

            var name = dependencies.Connection.DbConnection.GetType().Name;
            switch(name)
            {
                case "SqlServerConnection":
                    this.dbType = RelationalDbTypes.SqlServer;
                    break;
                case "SqliteConnection":
                    this.dbType = RelationalDbTypes.Sqlite;
                    break;
                default:
                    throw new NotSupportedException(name.Replace("Connection", ""));
            }
        }


        protected override string TableName { get; }
        protected override string TableSchema { get; }
        

        protected override string ExistsSql
        {
            get
            { 
                 var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
                switch(this.dbType)
                {
                    case RelationalDbTypes.SqlServer:
                        return "SELECT OBJECT_ID(" +
                            stringTypeMapping.GenerateSqlLiteral(
                                SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema)) +
                            ")" + SqlGenerationHelper.StatementTerminator;

                    case RelationalDbTypes.Sqlite:
                        return "SELECT COUNT(*) FROM \"sqlite_master\" WHERE \"name\" = " +
                            stringTypeMapping.GenerateSqlLiteral(TableName) +
                            " AND \"type\" = 'table';";
                }

                throw new NotSupportedException(this.dbType.ToString());
            }
        }

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            var conventions = this.hOpts.NamingConventions;
            var tableName = TableName;
            var schemaName = TableSchema;
            var migrationId = "MigrationId";
            var productVersion = "ProductVersion";
            if(conventions != null)
            {
                tableName = conventions.FormatTableName(tableName);
                schemaName = conventions.FormatSchemaName(schemaName);
                migrationId = conventions.FormatColumnName(migrationId);
                productVersion = conventions.FormatColumnName(productVersion);
            }
         
            history.ToTable(this.TableName, this.TableSchema);
            history.HasKey(h => h.MigrationId);
            
          
            history.Property(h => h.MigrationId)
                .HasMaxLength(150)
                .HasColumnName(migrationId);

          
            var pv = history.Property(h => h.ProductVersion)
                .HasMaxLength(32)
                .HasColumnName(productVersion)
                .IsRequired();
        }

        public override string GetBeginIfExistsScript(string migrationId)
        {
            if(string.IsNullOrWhiteSpace(migrationId))
                throw new ArgumentNullException(nameof(migrationId));

            switch(this.dbType)
            {
                case RelationalDbTypes.SqlServer:
                    var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
                    return new StringBuilder()
                        .Append("IF EXISTS(SELECT * FROM ")
                        .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
                        .Append(" WHERE ")
                        .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
                        .Append(" = ")
                        .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
                        .AppendLine(")")
                        .Append("BEGIN")
                        .ToString();

              
            }

            throw new NotSupportedException(this.dbType.ToString());
        }

        public override string GetBeginIfNotExistsScript(string migrationId)
        {
            switch(this.dbType)
            {
                case RelationalDbTypes.SqlServer:
                    var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
                    return new StringBuilder()
                        .Append("IF NOT EXISTS(SELECT * FROM ")
                        .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
                        .Append(" WHERE ")
                        .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
                        .Append(" = ")
                        .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
                        .AppendLine(")")
                        .Append("BEGIN")
                        .ToString();
            }

            throw new NotSupportedException(this.dbType.ToString());
        }

        public override string GetCreateIfNotExistsScript()
        {
            switch(this.dbType)
            {
                case RelationalDbTypes.SqlServer:
                
                    var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

                    var builder = new StringBuilder()
                        .Append("IF OBJECT_ID(")
                        .Append(
                            stringTypeMapping.GenerateSqlLiteral(
                                SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema)))
                        .AppendLine(") IS NULL")
                        .AppendLine("BEGIN");

                    using (var reader = new StringReader(GetCreateScript()))
                    {
                        var first = true;
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                builder.AppendLine();
                            }

                            if (line.Length != 0)
                            {
                                builder
                                    .Append("    ")
                                    .Append(line);
                            }
                        }
                    }

                    builder
                        .AppendLine()
                        .Append("END")
                        .AppendLine(SqlGenerationHelper.StatementTerminator);

                    return builder.ToString();
                
                case RelationalDbTypes.Sqlite:
                    var script = GetCreateScript();
                    return script.Insert(script.IndexOf("CREATE TABLE", StringComparison.Ordinal) + 12, " IF NOT EXISTS");
            }

            throw new NotSupportedException(this.dbType.ToString());
        }

        public override string GetEndIfScript()
        {
           switch(this.dbType)
            {
                case RelationalDbTypes.SqlServer:
                    return new StringBuilder()
                        .Append("END")
                        .AppendLine(SqlGenerationHelper.StatementTerminator)
                        .ToString();
            }

            throw new NotSupportedException(this.dbType.ToString());
        }

        protected override bool InterpretExistsResult( object value)
        {
            switch(this.dbType)
            {
                case RelationalDbTypes.SqlServer:
                    return value != DBNull.Value;
            }

            throw new NotSupportedException(this.dbType.ToString());
        }
    }
}