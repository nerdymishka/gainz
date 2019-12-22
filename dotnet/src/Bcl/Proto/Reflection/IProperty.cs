

using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IProperty : IReflectionMember
    {
        IType DeclaringType { get; }

        PropertyInfo PropertyInfo { get; }

        FieldInfo FieldInfo { get; }

        bool CanRead { get; }

        bool CanWrite { get; }

        bool IsPublic { get; }

        bool IsStatic { get; }

        bool IsInstance { get; }

        bool IsPrivate { get;  }

        bool IsSetterPublic { get; }


        object GetValue(object instance);

        T GetValue<T>(object instance);

        void SetValue(object instance, object value);

    
    }
}