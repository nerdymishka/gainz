

using System;
using System.Collections;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public class ReflectionCache : IReflectionCache, IEnumerable
    {
        private static ReflectionCache s_globalCache = new ReflectionCache();

        private int capacity = -1;

        private Dictionary<string, WeakReference<IType>> typeCache = new Dictionary<string, WeakReference<IType>>();

        public IReflectionFactory Factory { get; set; } = new ReflectionBuilder();

        public int Capacity => capacity;

        public ReflectionCache() { }

        public ReflectionCache(int capacity)
        {
            this.capacity = capacity;
            this.typeCache = new Dictionary<string, WeakReference<IType>>(capacity);
        }

        public int Count => this.typeCache.Count;

        public static ReflectionCache Global => s_globalCache;

        public static IType FindOrAdd(Type type)
        {
            return s_globalCache.GetOrAdd(type);
        }

        public IType GetOrAdd(Type type)
        {
            if(this.typeCache.TryGetValue(type.FullName, out WeakReference<IType> weak))
            {
                if(weak.TryGetTarget(out IType target))
                    return target;

                this.typeCache.Remove(type.FullName);
            }

            IType typeInfo = null;  
            

            if(type.IsInterface)
                typeInfo = Factory.CreateInterface(type);
            else 
                typeInfo  = Factory.CreateType(type);

            this.typeCache.Add(type.FullName, new WeakReference<IType>(typeInfo));

            return typeInfo;
        }

        public void Clear()
        {
            this.typeCache.Clear();
        }

        public IEnumerator<IType> GetEnumerator()
        {
            foreach(var pair in this.typeCache)
            {
                if(pair.Value.TryGetTarget(out IType type))
                    yield return type;
            }
        }

        

        public void Remove(Type type)
        {
            this.typeCache.Remove(type.FullName);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach(var pair in this.typeCache)
            {
                if(pair.Value.TryGetTarget(out IType type))
                    yield return type;
            }
        }
    }
}