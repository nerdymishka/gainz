

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Reflection.Extensions
{
    public static class TypeExtensions
    {

        public static IType AsTypeInfo(this Type clrType)
        {
            return ReflectionCache.FindOrAdd(clrType);
        }

        public static IItemType AsItemType(this IType type)
        {
            return type as IItemType;
        }

        public static bool IsArray(this IType type)
        {
            bool searched = type.HasFlag("Searched:Array");
            if(searched)
                return type.HasFlag("Array");

           
            var result = type.ClrType.IsArray;
            if(result && type is IItemType)
            {
                var t = ReflectionCache.FindOrAdd(type.ClrType.GetElementType());
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:Array", true);
            type.SetFlag("Array", result);
            return result;
        }

        public static bool IsNullableOfT(this IType type)
        {
            if(!type.ClrType.IsGenericType)
                return false;

            bool searched = type.HasFlag("Searched:IsNullable");
            if(searched)
                return type.HasFlag("INullable<>");

            var query = typeof(Nullable<>);
            var result = false;
            if(type.ClrType.IsGenericType && !type.ClrType.IsGenericTypeDefinition)
            {
                if(type.ClrType.GetGenericTypeDefinition() == query)
                {
                    result = true;
                }
            }
            if(result && type is IUnderlyingType)
            {
                var arg = type.ClrType.GetGenericArguments()[0];
                var t = arg.AsTypeInfo();
                ((IUnderlyingType)type).UnderlyingType = t;
                if(type is IItemType)
                ((IItemType)type).ItemType = t;
            }

            type.SetFlag("Searched:IsNullable<>", true);
            type.SetFlag("IsNullable<>", result);
           
            return result;
        }

        public static bool IsIList(this IType type)
        {
            bool searched = type.HasFlag("Searched:IList");
            if(searched)
                return type.HasFlag("IList");

           
            var query = typeof(IList);
            var result = type.Interfaces.Any(o => o.ClrType == query);
            if(result && type is IItemType)
            {
                var t = ReflectionCache.FindOrAdd(typeof(Object));
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:IList", true);
            type.SetFlag("IList", result);
            return result;
        }

        public static bool IsIDictionary(this IType type)
        {
            bool searched = type.HasFlag("Searched:IDictionary");
            if(searched)
                return type.HasFlag("IDictionary");

           
            var query = typeof(IDictionary);
            var result = type.Interfaces.Any(o => o.ClrType == query);
            if(result && type is IItemType)
            {
                var t = ReflectionCache.FindOrAdd(typeof(Object));
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:IDictionary", true);
            type.SetFlag("IDictionary", result);
            return result;
        }

        public static bool IsICollection(this IType type)
        {
            bool searched = type.HasFlag("Searched:ICollection");
            if(searched)
                return type.HasFlag("ICollection");

           
            var query = typeof(ICollection<>);
            var result = type.Interfaces.Any(o => o.ClrType == query);
            if(result && type is IItemType)
            {
                var t = ReflectionCache.FindOrAdd(typeof(Object));
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:ICollection", true);
            type.SetFlag("ICollection", result);
            return result;
        }

        public static bool IsIListOfT(this IType type)
        {
            bool searched = type.HasFlag("Searched:IList<>");
            if(searched)
                return type.HasFlag("IList<>");

           
            var query = typeof(IList<>);
            var result = false;
            foreach(var iinterface in type.Interfaces)
            {
                var clrType = iinterface.ClrType;
                if(clrType.IsGenericType && !clrType.IsGenericTypeDefinition)
                {
                    clrType = clrType.GetGenericTypeDefinition();
                    if(clrType == query)
                    {
                        result = true;
                        break;
                    }
                }
            }

            if(result && type is IItemType)
            {
                var arg = type.ClrType.GetGenericArguments()[0];
                var t = ReflectionCache.FindOrAdd(arg);
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:IList<>", true);
            type.SetFlag("IList<>", result);
            return result;
        }

        public static bool IsIDictionaryOfKv(this IType type)
        {
            bool searched = type.HasFlag("Searched:IDictionary<,>");
            if(searched)
                return type.HasFlag("IDictionary<,>");

           
            var query = typeof(IDictionary<,>);
            var result = false;
            foreach(var iinterface in type.Interfaces)
            {
                var clrType = iinterface.ClrType;
                if(clrType.IsGenericType && !clrType.IsGenericTypeDefinition)
                {
                    clrType = clrType.GetGenericTypeDefinition();
                    if(clrType == query)
                    {
                        result = true;
                        break;
                    }
                }
            }
            if(result && type is IItemType)
            {
                var args = type.ClrType.GetGenericArguments();
                var arg = args[1];
                var t = ReflectionCache.FindOrAdd(arg);
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:IDictionary<,>", true);
            type.SetFlag("IDictionary<,>", result);
            return result;
        }

        public static bool IsICollectionOfT(this IType type)
        {
            bool searched = type.HasFlag("Searched:ICollection<>");
            if(searched)
                return type.HasFlag("ICollection<>");

           
            var query = typeof(ICollection<>);
            var result = false;
            foreach(var iinterface in type.Interfaces)
            {
                var clrType = iinterface.ClrType;
                if(clrType.IsGenericType && !clrType.IsGenericTypeDefinition)
                {
                    clrType = clrType.GetGenericTypeDefinition();
                    if(clrType == query)
                    {
                        result = true;
                        break;
                    }
                }
            }
            if(result && type is IItemType)
            {
                var arg = type.ClrType.GetGenericArguments()[0];
                var t = ReflectionCache.FindOrAdd(arg);
                if(type is IItemType)
                ((IItemType)type).ItemType = t;
            }
            type.SetFlag("Searched:ICollection<>", true);
            type.SetFlag("ICollection<>", result);
            return result;
        }
    }
}