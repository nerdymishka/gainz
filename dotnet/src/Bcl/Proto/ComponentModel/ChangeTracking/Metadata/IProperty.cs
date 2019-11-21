
using System;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.ComponentModel.ChangeTracking.Metadata
{
    public interface IProperty : ITypeInfo
    {
    

        PropertyInfo PropertyInfo { get; }

        FieldInfo Field { get; }

        IEnumerable<Attribute>  CustomAttributes { get; }
    }
}