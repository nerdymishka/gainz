

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Reflection
{

    public class ReflectionPropertyMember : ReflectionMember, IProperty
    {
        private Delegate getter;
        private Delegate setter;

        public ReflectionPropertyMember(PropertyInfo info)
        {
            this.Name = info.Name;
            this.PropertyInfo = info;
            this.CanWrite = info.CanWrite;
            this.CanRead = info.CanRead;
            this.IsStatic = info.GetMethod.IsStatic;
            this.IsPublic = info.GetMethod.IsPublic || info.SetMethod.IsPublic;
            this.IsPrivate = !this.IsPublic;
            this.IsInstance = !this.IsStatic;
        }

        public ReflectionPropertyMember(FieldInfo info)
        {
            this.FieldInfo = info;
            this.IsField = true;
            this.CanRead = true;
            this.CanWrite = true;
            this.IsPublic = info.IsPublic;
            this.IsPrivate = !info.IsPublic;

            this.IsStatic = info.IsStatic;
        }

        public bool CanRead { get; protected set; }

        public bool CanWrite { get; protected set;}

        public bool IsStatic { get; protected set; }

        public bool IsPublic { get; protected set; }

        public bool IsInstance { get; protected set; }

        public bool IsPrivate { get; protected set; }

        public bool IsSetterPublic { get; protected set; }

        public bool IsField { get; protected set; }

        public PropertyInfo PropertyInfo { get; protected set; }

        public FieldInfo FieldInfo { get; protected set; }

        public virtual object GetValue(object instance)
        {
            if(!this.CanRead)
                throw new InvalidOperationException($"Property {this.Name} prohibits reading the value.");

            if(this.getter == null)
            {
                var oVariable = Expression.Parameter(this.PropertyInfo.DeclaringType, "o");
                var invokePropertyGet = Expression.Property(oVariable, this.PropertyInfo);
                var b = Expression.Block(
                    invokePropertyGet
                );
                this.getter = Expression.Lambda(b, oVariable).Compile();
            }

            return this.getter.DynamicInvoke(instance);
        }

        public virtual T GetValue<T>(object instance)
        {
            return (T)this.GetValue(instance);
        }

        public virtual void SetValue(object instance, object value)
        {
            if(!this.CanWrite)
                throw new InvalidOperationException($"Property {this.Name} prohibits reading the value.");

            if(this.setter == null)
            {
                var oVariable = Expression.Parameter(this.PropertyInfo.DeclaringType, "o");
                var invokePropertyGet = Expression.Property(oVariable, this.PropertyInfo);
                var valueVariable = Expression.Variable(this.ClrType, "value");
                var b = Expression.Block(
                    Expression.Assign(invokePropertyGet, valueVariable)
                );
         
                this.setter = Expression.Lambda(b, oVariable, valueVariable).Compile();
            }

            this.setter.DynamicInvoke(instance, value);
        }

        
        public override IReflectionMember LoadAttributes(bool inherit = true)
        {
            this.SetAttibutes(
                CustomAttributeExtensions.GetCustomAttributes(this.PropertyInfo, inherit));

            return this;
        }
    }

}