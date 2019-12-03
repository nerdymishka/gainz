using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;

namespace Tests 
{
    [Trait("tag", "unit")]
    public class DynamicTheory
    {
        [Fact]
        public void Test()
        {
            dynamic d = new DankDynamicObject();
            d.Test = "z";

            Assert.Equal("z", d.Test);

            dynamic s = new Special();
            s.MyProp = "Mine";

            Assert.Equal("Mine", s.MyProp);
        }

        public class Special : DankDynamicObject
        {
            public string MyProp { get; set; }
        }
    }


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
                  var prop  = this.properties.SingleOrDefault(o => o.Name == name);
                if(prop != null) {
                       
                    
                    return new DynamicMetaObject(
                        
                         Expression.Constant(prop.GetValue(this.value)), BindingRestrictions.GetTypeRestriction(Expression, LimitType));
                 
                }

               
                var mi = this.type.GetMethod("GetValue", BindingFlags.NonPublic | BindingFlags.Instance);

                return new DynamicMetaObject(
                    Expression.Call(
                        Expression.Convert(Expression, LimitType),
                        mi, 
                        new Expression[] { Expression.Constant(name)}
                    ),BindingRestrictions.GetTypeRestriction(Expression, this.type)
                );

            }

            public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
            {
                var name = binder.Name;
                
                var prop = this.properties.SingleOrDefault(o => o.Name == name);
                  var obj = Expression.Convert(value.Expression, typeof(object));
                if(prop != null) {
                   prop.SetValue(this.value, value.Value);
                    return new DynamicMetaObject(
                        
                        Expression.Block( Expression.Empty(), obj), BindingRestrictions.GetTypeRestriction(Expression, LimitType));

                 
                
                }

                var mi = this.type.GetMethod("SetValue", BindingFlags.NonPublic | BindingFlags.Instance);

              

                var args = new Expression[] {
                    Expression.Constant(name),
                    obj
                };

                var self = Expression.Convert(Expression, typeof(DankDynamicObject));

                return new DynamicMetaObject(
                    Expression.Block(
                        Expression.Call(self, mi, args),
                        obj 
                    ),
                    BindingRestrictions.GetTypeRestriction(Expression, typeof(DankDynamicObject))
                );
            }
        }
    }

}

