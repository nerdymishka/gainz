
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;

namespace NerdyMishka.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        private static object s_lock = new object();
        private static readonly IDictionary<IServiceCollection, IList<IModule>> s_modules = 
            new Dictionary<IServiceCollection, IList<IModule>>();

        /// <summary>
        /// Clears the registered modules stored in memory to prevent duplicate registrations. This
        /// will not remove the reigistered depedencies when the Apply method is called.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public static void ClearRegisteredModules(this IServiceCollection services)
        {
            lock(s_lock)
            {
                if(s_modules.TryGetValue(services, out IList<IModule> set))
                    set.Clear();
            }

        }

        /// <summary>
        /// Registers a module that will modify the <see cref="IServiceCollection" />
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="module">The module to register.</param>
        /// <returns></returns>
        public static IServiceCollection RegisterModule(this IServiceCollection services, IModule module)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            if(module == null)
                throw new ArgumentNullException(nameof(module));

            lock(s_lock)
            {
                if(!s_modules.TryGetValue(services, out IList<IModule> set))
                {
                    set = new List<IModule>();
                    s_modules.Add(services, set);
                }

                if(!set.Contains(module))
                {
                    module.Apply(services);
                    set.Add(module);
                }
            }
            return services;
        }


        /// <summary>
        /// Scans the assembly for any public class that is not abstract that implements the <see cref="IModule" />
        /// contract.  
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">The assemblies to scan</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection RegisterModuleAssemblies(this IServiceCollection services, IList<Assembly> assemblies)
        {
            if(services == null)
                throw new ArgumentNullException(nameof(services));

            if(assemblies == null)
                throw new ArgumentNullException(nameof(assemblies));

            foreach(var assembly in assemblies)
            {
                var modules = assembly.GetTypes()
                    .Where(o => o.IsClass && o.IsPublic && !o.IsAbstract)
                    .Where(o => o.GetInterfaces().Any(i => i == typeof(IModule)))
                    .ToList();

                if(modules.Count > 0)
                {
                    foreach(var moduleType in modules)
                    {
                        var module = (IModule)System.Activator.CreateInstance(moduleType);
                        services.RegisterModule(module);
                    }
                }
            }

            return services;
        }
    }
}