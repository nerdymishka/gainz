using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NerdyMishka.EfCore.Metadata
{
    public interface IEfCoreConvention
    {

    }

    public interface IEntityTypeConvention : IEfCoreConvention
    {
        void Apply(IMutableEntityType mutableEntityType);
    }

    public interface IRelationalEntityTypeConvention : IEfCoreConvention
    {
        void Apply(RelationalEntityTypeAnnotations annotations);
    }

    public interface IPropertyConvention : IEfCoreConvention
    {
        void Apply(IMutableProperty mutableProperty);
    }

    public interface IRelationalPropertyConvention : IEfCoreConvention
    {
        void Apply(RelationalPropertyAnnotations annotations);
    }

    public interface IForeignKeyConvention : IEfCoreConvention
    {
        void Apply(IMutableForeignKey mutableForeignKey);
    }

    public interface IRelationalForeignKeyConvention : IEfCoreConvention
    {
        void Apply(RelationalForeignKeyAnnotations annotations);
    }

    public interface IKeyConvention : IEfCoreConvention
    {
        void Apply(IMutableKey mutableKey);
    }

    public interface IRelationalKeyConvention : IEfCoreConvention
    {
        void Apply(RelationalKeyAnnotations annotations);
    }

    public interface IIndexConvention : IEfCoreConvention
    {
        void Apply(IMutableIndex mutableIndex);
    }

    public interface IRelationalIndexConvention: IEfCoreConvention
    {
        void Apply(RelationalIndexAnnotations annotations);
    }

    public interface INavigationConvention: IEfCoreConvention
    {
        void Apply(IMutableNavigation mutableNavigation);
    }


    public interface ISequenceConvention : IEfCoreConvention
    {
        void Apply(IMutableSequence mutableSequence);
    }

}