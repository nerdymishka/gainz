using System;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IReflectionFactory
    {
        IParameter CreateParameter(ParameterInfo info);

        IMethod CreateMethod(MethodInfo info);

        IMethod CreateMethod(MethodInfo info, ParameterInfo[] parameters);

        IProperty CreateProperty(PropertyInfo info, IType declaringType = null);

        IProperty CreateProperty(FieldInfo info, IType declaringType = null);

        IType CreateType(Type info);

        IType CreateType(TypeInfo info);

        IInterface CreateInterface(Type info);

        IInterface CreateInterface(TypeInfo info);
    }
}