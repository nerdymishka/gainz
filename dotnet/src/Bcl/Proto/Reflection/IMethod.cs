
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IMethod : IReflectionMember
    {
        MethodInfo MethodInfo { get; }

        IReadOnlyCollection<IParameter> Parameters { get; }

        IReadOnlyCollection<Type> GenericArguments { get; }

        IReadOnlyCollection<Type> ParameterTypes { get; }
        bool IsStatic { get; }
        bool IsPublic { get; }
        bool IsPrivate{ get; }
        bool IsInstance { get; }
        bool IsVirtual { get; }
        bool IsFamily { get; }
        bool IsAssembly { get; }

        object Invoke(object instance, params object[] parameters);

        T Invoke<T>(object instance, params object[] parameters);
    }
}