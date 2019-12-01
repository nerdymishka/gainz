
using System;

namespace NerdyMishka.Reflection
{
    public interface IUnderlyingType
    {
        IType UnderlyingType { get; set; }
    }
}