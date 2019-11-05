using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NerdyMishka.EfCore.Metadata
{
    public interface IConstraintConventions
    {
        string FormatTableName(string tableName);

        string FormatSchemaName(string schemaName);

        string FormatColumnName(string columnName);

        string GetDefaultTableName(IEntityType entityType);

        string GetDefaultSchemaName(IEntityType entityType);

        string GetDefaultName(IForeignKey foreignKey);

        string GetDefaultName(IIndex index);

        string GetDefaultName(IKey key);

        string GetDefaultName(IProperty property);

        IEnumerable<IEfCoreConvention> Conventions { get; }
    }
}