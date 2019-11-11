

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DankInventory.Data
{
    public class DankDynamicObject : IDynamicMetaObjectProvider
    {
        private Dictionary<string, object> storage;

        protected void SetValue(string name, object value)
        {
            this.storage = storage ?? new Dictionary<string, object>();
            this.storage.Add(name, value);
        }

        protected object GetValue(string name)
        {
            this.storage = storage ?? new Dictionary<string, object>();
            this.storage.TryGetValue(name, out object value);
            return value;
        }


        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DankMetaObject(parameter, BindingRestrictions.Empty, this);
        }

        private class DankMetaObject: DynamicMetaObject
        {
            private readonly IDynamicMetaObjectProvider innerProvider;
            private Type type;

            private object value;
            private PropertyInfo[] properties;

         

            public DankMetaObject(Expression expr, BindingRestrictions restrictions, object value)
                : base(expr, BindingRestrictions.Empty, value)
            {
                this.value = value;
                this.type = value.GetType();
                this.properties = this.type.GetProperties();
            }

            public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
            {
                var name = binder.Name;

                if(this.properties.Any(o => o.Name == name)) {
                    return new DynamicMetaObject(
                        Expression.Property(Expression.Convert(Expression, this.type), name),
                        BindingRestrictions.Empty
                    );
                }

               
                var mi = this.type.GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance);

                return new DynamicMetaObject(
                    Expression.Call(
                        Expression.Convert(Expression, LimitType),
                        mi, 
                        new Expression[] { Expression.Constant(name)}
                    ),BindingRestrictions.Empty
                );

            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                var name = binder.Name;
                
                var prop = this.properties.SingleOrDefault(o => o.Name == name);
                if(prop != null) {
                   
                    return new DynamicMetaObject(
                        Expression.Call(
                            Expression.Convert(Expression, this.type), 
                            prop.GetSetMethod(), value.Expression),
                        BindingRestrictions.Empty
                    );
                }

                var mi = this.type.GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);

                var args = new Expression[] {
                    Expression.Constant(name),
                    Expression.Convert(value.Expression, typeof(object))
                };

                var self = Expression.Convert(Expression, LimitType);

                return new DynamicMetaObject(
                    Expression.Call(self, mi, args),
                    BindingRestrictions.Empty
                );
            }
        }
    }
}