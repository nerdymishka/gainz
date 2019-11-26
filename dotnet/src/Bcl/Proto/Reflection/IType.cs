

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IType : IReflectionMember
    {
        IType BaseType { get; }

        IType UnderlyingType { get;  }

        string Namespace { get; }

        string FullName { get; }

        bool IsDataType { get; }

        IReadOnlyCollection<IProperty> Properties { get; }

        IReadOnlyCollection<IInterface> Interfaces { get; }

        IReadOnlyCollection<IMethod> Methods { get; }

        IProperty GetProperty(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance);

        IEnumerable<IProperty> GetProperties(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance);

        IMethod GetMethod(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance,
            Type[] genericArgTypes = null,
            Type[] parameterTypes = null);

        IEnumerable<IMethod> GetMethods(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance,
            Type[] genericArgTypes = null,
            Type[] parameterTypes = null);
       
    }
}