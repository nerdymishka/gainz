

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit.Abstractions;

namespace Mettle
{
    internal static class Extensions
    {
        internal static object GetDefaultValue(this TypeInfo typeInfo)
        {
            if (typeInfo.IsValueType)
                return Activator.CreateInstance(typeInfo.AsType());

            return null;
        }

        
        public static void Add<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value)
    {
        dictionary.GetOrAdd(key).Add(value);
    }

    public static bool Contains<TKey, TValue>(this IDictionary<TKey, List<TValue>> dictionary, TKey key, TValue value, IEqualityComparer<TValue> valueComparer)
    {
        List<TValue> values;

        if (!dictionary.TryGetValue(key, out values))
            return false;

        return values.Contains(value, valueComparer);
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : new()
    {
        return dictionary.GetOrAdd<TKey, TValue>(key, () => new TValue());
    }

    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> newValue)
    {
        TValue result;

        if (!dictionary.TryGetValue(key, out result))
        {
            result = newValue();
            dictionary[key] = result;
        }

        return result;
    }


        public static IEnumerable<IAttributeInfo> GetCustomAttributes(this ITypeInfo typeInfo, Type attributeType)
        {
            return typeInfo.GetCustomAttributes(attributeType.AssemblyQualifiedName);
        }

        
    /// <summary>
    /// Gets all the custom attributes for the given assembly.
    /// </summary>
    /// <param name="assemblyInfo">The assembly</param>
    /// <param name="attributeType">The type of the attribute</param>
    /// <returns>The matching attributes that decorate the assembly</returns>
    public static IEnumerable<IAttributeInfo> GetCustomAttributes(this IAssemblyInfo assemblyInfo, Type attributeType)
    {
        return assemblyInfo.GetCustomAttributes(attributeType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Gets all the custom attributes for the given attribute.
    /// </summary>
    /// <param name="attributeInfo">The attribute</param>
    /// <param name="attributeType">The type of the attribute to find</param>
    /// <returns>The matching attributes that decorate the attribute</returns>
    public static IEnumerable<IAttributeInfo> GetCustomAttributes(this IAttributeInfo attributeInfo, Type attributeType)
    {
        return attributeInfo.GetCustomAttributes(attributeType.AssemblyQualifiedName);
    }

    /// <summary>
    /// Gets all the custom attributes for the method that are of the given type.
    /// </summary>
    /// <param name="methodInfo">The method</param>
    /// <param name="attributeType">The type of the attribute</param>
    /// <returns>The matching attributes that decorate the method</returns>
    public static IEnumerable<IAttributeInfo> GetCustomAttributes(this IMethodInfo methodInfo, Type attributeType)
    {
        return methodInfo.GetCustomAttributes(attributeType.AssemblyQualifiedName);
    }

    }
}