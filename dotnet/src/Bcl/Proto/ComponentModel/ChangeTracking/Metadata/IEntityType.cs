

using System.Collections.Generic;

namespace NerdyMishka.ComponentModel.ChangeTracking.Metadata
{
    public interface IEntityType : ITypeInfo
    {
        IEntityType BaseType { get; }

        IKey FindPrimaryKey();

        IEnumerable<IKey> GetKeys();
        
        IEnumerable<IProperty> GetProperties();

        IEnumerable<IRelationship> GetRelationships();
    }
}