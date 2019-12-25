using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionCache : IReadOnlyCollection<IType>
    {
        int Capacity { get; }

        IType GetOrAdd(Type type);

        void Clear();

        void Remove(Type type);

    }

}