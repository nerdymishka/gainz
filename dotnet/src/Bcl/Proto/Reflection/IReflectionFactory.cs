using System;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionFactory
    {
        IParameter CreateParameter(ParameterInfo info);

        IMethod CreateMethod(MethodInfo info);

        IMethod CreateMethod(MethodInfo info, ParameterInfo[] parameters);

        IProperty CreateProperty(PropertyInfo info);

        IProperty CreateProperty(FieldInfo info);

        IType CreateType(Type info);

        IType CreateType(TypeInfo info);

        IInterface CreateInterface(Type info);

        IInterface CreateInterface(TypeInfo info);
    }
}