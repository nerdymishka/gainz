
using System;

namespace NerdyMishka.ComponentModel.ChangeTracking.Metadata
{
    public interface ITypeInfo
    {
        string Name { get; }

        string FullName { get; }

        Type ClrType { get; set; }
    }
}