

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionParameter : ReflectionMember, IParameter
    {
        public ReflectionParameter(ParameterInfo info)
        {
            this.Name = info.Name;
            this.ClrType = info.ParameterType;
        }
        

        public object DefaultValue => this.ParameterInfo.DefaultValue;

        public int Position => this.ParameterInfo.Position;

        public bool IsOut => this.ParameterInfo.IsOut;

        public bool IsOptional => this.ParameterInfo.IsOptional;

        public ParameterInfo ParameterInfo { get; protected set; }

       
        public override IReflectionMember LoadAttributes(bool inherit = true)
        {
            this.SetAttributes(
                CustomAttributeExtensions.GetCustomAttributes(this.ParameterInfo, inherit));

            return this;
        }
    }
}