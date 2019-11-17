

using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public class ReflectionCache
    {
        private static Dictionary<Type, FlexTypeInfo> s_typeCache = new Dictionary<Type, FlexTypeInfo>();

        public static FlexTypeInfo GetOrAdd(Type type)
        {
            if(s_typeCache.TryGetValue(type, out FlexTypeInfo info))
                return info;

            info = new FlexTypeInfo(type);
            info.Inspect();

            s_typeCache.Add(type, info);

            return info;
        }
    }
}