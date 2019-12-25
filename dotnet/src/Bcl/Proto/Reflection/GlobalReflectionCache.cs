

using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public class GlobalReflectionCache
    {
        private static Dictionary<string, IType> s_typeCache = new Dictionary<string, IType>();

        public static IReflectionFactory Factory { get; set; } = new ReflectionBuilder();

        public static IType GetOrAdd(Type type)
        {
            if(s_typeCache.TryGetValue(type.FullName, out IType info))
                return info;

            if(type.IsInterface)
                info = Factory.CreateInterface(type);
            else 
                info  = Factory.CreateType(type);

            s_typeCache.Add(type.FullName, info);

            return info;
        }

        
    }
}