
using System;
using System.Collections.Concurrent;

namespace Mettle
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private ConcurrentDictionary<Type, Func<IServiceProvider, object>> factories = 
            new ConcurrentDictionary<Type, Func<IServiceProvider, object>>(); 


        public SimpleServiceProvider()
        {
            factories.TryAdd(typeof(IAssert), (s) => { return AssertImpl.Current; });
        }

        public void AddSingleton(Type type, object instance)
        {
            this.factories.TryAdd(type, (s) => instance);
        }

        public void AddTransient(Type type)
        {
            this.factories.TryAdd(type, (s) => Activator.CreateInstance(type));
        }

        public void AddTransient(Type type, Func<IServiceProvider, object> activator)
        {
            this.factories.TryAdd(type, activator);
        }

        public object GetService(Type type)
        {
            if(this.factories.TryGetValue(type, out Func<IServiceProvider, object> factory))
                return factory(this);

            if(type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

    }
}

