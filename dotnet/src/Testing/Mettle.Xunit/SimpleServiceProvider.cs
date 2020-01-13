
using System;
using System.Collections.Concurrent;

namespace Mettle
{
    public class SimpleServiceProvider : IServiceProvider
    {
        private ConcurrentDictionary<Type, Func<object>> factories = 
            new ConcurrentDictionary<Type, Func<object>>(); 

        static SimpleServiceProvider()
        {
            Current = new SimpleServiceProvider();
        }

        protected SimpleServiceProvider()
        {
            factories.TryAdd(typeof(IAssert), () => { return AssertImpl.Current; });
            factories.TryAdd(typeof(ITestLogger), () => {
                return new SerilogTestLogger();
            });
        }

        public static IServiceProvider Current { get; set; }

        public void AddSingleton(Type type, object instance)
        {
            this.factories.TryAdd(type, () => instance);
        }

        public void AddTransient(Type type)
        {
            this.factories.TryAdd(type, () => Activator.CreateInstance(type));
        }

        public void AddTransient(Type type, Func<object> activator)
        {
            this.factories.TryAdd(type, activator);
        }

        public object GetService(Type type)
        {
            if(this.factories.TryGetValue(type, out Func<object> factory))
                return factory();

            return default;
        }

    }
}

