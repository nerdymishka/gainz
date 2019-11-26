

using System;
using NerdyMishka.Validation;

namespace NerdyMishka.Reflection.Extensions
{
    public static class ReflectionExtensions
    {

        public static object Invoke(this object instance, string methodName, params object[] parameters)
        {
            return instance.GetMethod(methodName)
                .Invoke(instance, parameters);
        }

        public static IMethod GetMethod(this object instance, string methodName, Type[] parameterTypes = null)
        {
            Check.NotNull(nameof(instance), instance);

            var type = ReflectionCache.GetOrAdd(instance.GetType());
            return type.GetMethod(methodName, parameterTypes);
        }

        public static IProperty GetProperty(this object instance, string propertyName)
        {
            Check.NotNull(nameof(instance), instance);

            var type = ReflectionCache.GetOrAdd(instance.GetType());
            return type.GetProperty(propertyName);
        }

        public static T GetValue<T>(this object instance, string propertyName) where T:class 
        {
            return instance.GetProperty(propertyName)
                        .GetValue<T>(instance);
        }

        public static void SetValue<T>(this object instance, string propertyName, T value) where T:class 
        {
            instance.GetProperty(propertyName)
                .SetValue(instance, value);
        }
    }
}