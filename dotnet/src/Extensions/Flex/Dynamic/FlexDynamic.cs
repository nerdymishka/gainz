

using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace NerdyMishka.Flex.Dyanmic 
{
    public class FlexObject : IDynamicMetaObjectProvider, IEnumerable<KeyValuePair<Symbol, object>>
    {
        private Dictionary<string, object> storage;





        protected void SetValue<T>(string name, T value)
        {
            this.storage = storage ?? new Dictionary<string, object>();
            this.storage.Add(name, value);
        }


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

        protected T GetValue<T>(string name)
        {
            return (T)this.GetValue(name);
        }


        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new FlexMetaObject(parameter, BindingRestrictions.Empty, this);
        }

        public IEnumerator<KeyValuePair<Symbol, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private class FlexMetaObject: DynamicMetaObject
        {
            private readonly IDynamicMetaObjectProvider innerProvider;
            private Type type;

            private object value;
            private PropertyInfo[] properties;

         

            public FlexMetaObject(Expression expr, BindingRestrictions restrictions, object value)
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


