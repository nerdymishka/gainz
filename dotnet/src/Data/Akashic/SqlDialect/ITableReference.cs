
using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public interface ITableReference : INamedDbObject
    {
        string SchemaName { get; set; }


        IEnumerable <IColumnReference> Keys { get; }

        IEnumerable<IColumnReference> Columns { get; }

        IEnumerable<IConstraintReference> Constraints { get; }
    }
}