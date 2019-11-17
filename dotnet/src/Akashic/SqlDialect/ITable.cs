
using System.Collections.Generic;

namespace NerdyMishka.Data
{
    public interface ITable : ITableReference, IEnumerable<IColumn>
    {

        bool AutoIncrement { get; }

        bool IsTemporary { get; set; }

        void Add(IColumn column);

        void Remove(IColumn column);
    }
}