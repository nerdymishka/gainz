

using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public interface IReflectionMember
    {
        string Name { get; }

        Type ClrType { get; }

        bool HasFlag(string flag);

        IReflectionFactory Factory  { get; }

        IReflectionMember SetFlag(string flag, bool value);

        T GetMetadata<T>(string name);

        IReflectionMember SetMetadata<T>(string name, T value);

        IReflectionMember LoadAttributes(bool inherit = true);

        IReadOnlyCollection<Attribute> Attributes { get; }

        T FindAttribute<T>() where T: Attribute; 

        IEnumerable<T> FindAttributes<T>() where T: Attribute;
    }
}