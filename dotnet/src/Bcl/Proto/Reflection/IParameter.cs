
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public interface IParameter : IReflectionMember
    {

        ParameterInfo ParameterInfo { get; }
        
        object DefaultValue { get; }

        int Position { get; }

        bool IsOut { get; }

        bool IsOptional { get; }
    }
}