

using System;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionBuilder : IReflectionFactory
    {
        public virtual IProperty BuildProperty(PropertyInfo info)
        {
            return new ReflectionPropertyMember(info);
        }

        public static Type[] DataTypes { get; set; } = new []{ 
            typeof(string), 
            typeof(DateTime), 
            typeof(TimeSpan), 
            typeof(char[]), 
            typeof(byte[]) 
        };

        public virtual IType BuildType(Type info)
        {
            return new ReflectionType(info, this);
        }
        public IParameter CreateParameter(ParameterInfo info)
        {
             return new ReflectionParameter(info);
        }

        public IMethod CreateMethod(MethodInfo info)
        {
            return new ReflectionMethod(info, this);
        }

        public IMethod CreateMethod(MethodInfo info, ParameterInfo[] parameters)
        {
            return new ReflectionMethod(info, parameters, this);
        }

        public IProperty CreateProperty(PropertyInfo info)
        {
            return new ReflectionPropertyMember(info);
        }

        public IProperty CreateProperty(FieldInfo info)
        {
            return new ReflectionPropertyMember(info);
        }

        public IType CreateType(Type info)
        {
            return ReflectionCache.GetOrAdd(info);
        }

        public IType CreateType(TypeInfo info)
        {
            return ReflectionCache.GetOrAdd(info.AsType());
        }

        public IInterface CreateInterface(Type info)
        {
            throw new NotImplementedException();
        }

        public IInterface CreateInterface(TypeInfo info)
        {
            throw new NotImplementedException();
        }
    }
}