using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace NerdyMishka.Primitives
{
    public enum FlexType
    {
        Primordial = 0,
        Undefined = 100,
        FlexSymbol = 1,
        FlexValue = 2,
        FlexProperty = 3,
        FlexList = 4,
        FlexObject = 5
    }

    public class FlexPrimitive :
         IEnumerable<FlexPrimitive>
    {
        private object value;

        public virtual FlexType FlexType => FlexType.Primordial;

        public virtual FlexPrimitive Child(string key)
        {
            return null;
        }

        public virtual object Unbox()
        {
            return null;
        }

        public virtual IEnumerator<FlexPrimitive> GetEnumerator()
        {
            return Enumerable.Empty<FlexPrimitive>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class FlexUndefined : FlexPrimitive
    {

        public static FlexUndefined Value { get; } = new FlexUndefined();

        public override FlexType FlexType => FlexType.Undefined;

        public override object Unbox()
        {
            return this;
        }
    }

    public class FlexValue<T> : FlexValue
    {
        public FlexValue(T value):base(value)
        {
            this.ClrType = typeof(T);
        }

        public new T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value;  }
        }
    }

    public class FlexValue : FlexPrimitive
    {
        private Type clrType;

        public FlexValue(object value)
        {
            this.Value = value;
            if (value != null)
                this.ClrType = value.GetType();
        }

        public override FlexType FlexType => FlexType.FlexValue;

        public virtual object Value { get; set; }

        public virtual Type ClrType
        {
            get
            {
                if(this.clrType == null)
                {
                    this.clrType = typeof(object);

                    if (this.Value != null)
                        this.clrType = this.Value.GetType();
                }

                return this.clrType;
            }
            protected set
            {
                this.clrType = value;
            }
        }
        public bool Equals(FlexValue value)
        {
            if (value == null)
                return false;

            return this.Value == value.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null && this.Value == null)
                return true;

            if (obj is FlexValue)
                return this.Equals((FlexValue)obj);

            return this.Value.Equals(obj);
        }


        public static implicit operator string(FlexValue primordial)
        {
            return primordial.Value as string;
        }

        public static implicit operator int(FlexValue primordial)
        {
            var v = primordial.Value;
            if (v is int)
                return (int)v;

            if (v is short)
                return (short)v;

            throw new InvalidCastException();
        }

        public static implicit operator long(FlexValue primordial)
        {
            var v = primordial.Value;
            if (v is long)
                return (long)v;

            if (v is int)
                return (int)v;
            

            if (v is short)
                return (short)v;

            throw new InvalidCastException();
        }

        public static bool operator ==(FlexValue left, FlexValue right)
        {
            return left.Value == right.Value;
        }

        public static bool operator !=(FlexValue left, FlexValue right)
        {
            return left.Value != right.Value;
        }

        public void Update(object value)
        {
            this.Value = value;
        }

        public override object Unbox()
        {
            if (this.Value is FlexPrimitive)
                return ((FlexPrimitive)this.Value).Unbox();

            return this.Value;
        }

        public override IEnumerator<FlexPrimitive> GetEnumerator()
        {
            if (this.Value is FlexObject)
                return ((IEnumerable<FlexPrimitive>)this.Value).GetEnumerator();

            if (this.Value is FlexList)
                return ((IEnumerable<FlexPrimitive>)this.Value).GetEnumerator();


            return base.GetEnumerator();
        }
    }

    public class FlexSymbol: FlexPrimitive
    {
        private string symbol = null;
        private int hashCode;

        public FlexSymbol(string value)
        {
            this.symbol = value;
            this.hashCode = value.GetHashCode() + 31;
        }

        public override FlexType FlexType => FlexType.FlexSymbol;

        public static implicit operator string(FlexSymbol value)
        {
            return value.symbol;
        }

        public static implicit operator FlexSymbol(string value)
        {
            return new FlexSymbol(value);
        }

        public static bool operator ==(FlexSymbol left, FlexSymbol right)
        {
            return left.symbol == right.symbol;
        }

        public static bool operator !=(FlexSymbol left, FlexSymbol right)
        {
            return left.symbol != right.symbol;
        }

       

        public bool Equals(string other)
        {
            return this.symbol.Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is FlexSymbol)
                return this.symbol.Equals(obj.ToString());

            if (obj is string)
                return this.symbol.Equals(obj as string);

            return false;
        }

        public override string ToString()
        {
            return this.symbol;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override object Unbox()
        {
            return this.symbol;
        }
    }




    public class FlexProperty : FlexPrimitive
    {


        public FlexSymbol Name { get; set; }

        public FlexPrimitive Value { get; set; }

        public FlexProperty()
        {

        }

        public FlexProperty(string name, object value)
        {
            this.Name = new FlexSymbol(name);

            if (value is FlexPrimitive)
            {
                this.Value = (FlexPrimitive)value;
                return;
            }

            this.Value = FlexConverter.ToPrimordial(value);
        }

        public FlexProperty(KeyValuePair<string, object> keyValuePair)
        {
            this.Name = keyValuePair.Key;
            if(keyValuePair.Value is FlexPrimitive)
            {
                this.Value = (FlexPrimitive)keyValuePair.Value;
                return;
            }

            this.Value = FlexConverter.ToPrimordial(keyValuePair.Value);
        }

        public override FlexType FlexType => FlexType.FlexProperty;


        public static bool operator ==(FlexProperty left, FlexProperty right)
        {
            return left.Name == right.Name && left.Value == right.Value;
        }

        public static bool operator !=(FlexProperty left, FlexProperty right)
        {
            return left.Name != right.Name || left.Value != right.Value;
        }

        public bool Equals(FlexProperty property)
        {
            if (!this.Name.Equals(property.Name))
                return false;

            return this.Value.Equals(property.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is FlexProperty)
                return this.Equals((FlexProperty)obj);

            return false;
        }

        public override IEnumerator<FlexPrimitive> GetEnumerator()
        {
            yield return this.Name;
            yield return this.Value;
        }

        public override object Unbox()
        {
            return new KeyValuePair<string, object>(this.Name.ToString(), this.Value.Unbox());
        }
    }

    public class FlexList : FlexPrimitive, IFlexList
    {
        private FlexPrimitive[] items;
        private int size = 0;
        private const int growth = 10;

        public object this[int index]
        {
            get
            {
                if (index > this.size || index < 0)
                    return null;

                return this.items[index];
            }
            set
            {

            }
        }

        public override FlexType FlexType => FlexType.FlexList;

        public long Count => throw new NotImplementedException();

        public void Add(object value)
        {
            this.Insert(this.size, value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public FlexPrimitive GetPrimordial(int index)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            if (index > this.size)
                throw new ArgumentOutOfRangeException("index is larger than the size of the list");

            FlexPrimitive actualValue = new FlexValue(null);

            

            

    
        }

        public bool Remove(object value)
        {
            throw new NotImplementedException();
        }

        public object RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public override object Unbox()
        {
            var list = new List<object>();
            for(int i = 0; i < this.size; i++)
            {
                this.Add(this.items[i].Unbox());
            }

            return list;
        }


        private void Resize()
        {
            var next = new FlexPrimitive[this.items.Length + growth];
            Array.Copy(this.items, next, this.items.Length);

            this.items = next;
        }
    }

    public interface IFlexList :
        IEnumerable<FlexPrimitive>
    {
        object this[int index] { get; set; }

        FlexPrimitive Child(string key);

        FlexPrimitive GetPrimordial(int index);

        void Add(object value);

        void Clear();

        void Insert(int index, object value);

        bool Remove(object value);

        object RemoveAt(int index);

        long Count { get; }
    }



    public interface IFlexObject: IEnumerable<FlexPrimitive>
    {
        object this[string key] { get; set; }

        object Get(string key);
        void Set(string key, object value);
    }

    public class FlexObject : FlexPrimitive, 
        IList<FlexProperty>,
        IDynamicMetaObjectProvider,
       
        IFlexObject
    {
        private FlexProperty[] properties;
        private int size = 0;
        private int growth = 10;
        private static readonly StringComparer stringComparer = new Comparer();

        public override FlexType FlexType => FlexType.FlexObject;

        int ICollection<FlexProperty>.Count => this.size;

        bool ICollection<FlexProperty>.IsReadOnly => false;

        FlexProperty IList<FlexProperty>.this[int index]
        {
            get { return this.properties[index]; }
            set
            {
                this.properties[index] = value;
            }
        }

        private class Comparer : StringComparer
        {
            public override int Compare(string x, string y)
            {
                return x.CompareTo(y);
            }

            public override bool Equals(string x, string y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(string obj)
            {
                return 81;
            }
        }



        public object this[string propertyName]
        {
            get
            {
                return this.Get(propertyName);
            }
            set
            {
                this.Set(propertyName, value);
            }
        }

        protected object Get(string key)
        {
            var code = key.GetHashCode() + 31;

            for (var i = 0; i < this.size; i++)
            {
                var next = this.properties[i];
                if (next.Name.GetHashCode() == code && next.Name.Equals(key))
                {
                    return next.Value;
                }
            }

            return null;
        }

        protected void Add(string key, object value)
        {
            this.Set(key, value, true);
        }
        
        protected void Set(string key, object value)
        {
            this.Set(key, value, true);
        }

        private void Set(string key, object value, bool add)
        {
            if(this.properties.Length == this.size)
                this.Resize();

            FlexValue actual = new FlexValue(value);
            if (value is FlexValue)
                actual = (FlexValue)value;

            var entry = new FlexProperty() { Name = key, Value = actual };

            for(var i = 0; i < this.size; i++)
            {
                var next = this.properties[i];
                if(next.GetHashCode() == entry.GetHashCode() && next.Name.Equals(entry.Name))
                {
                    entry.Value = actual;
                    return;
                }
            }

            this.properties[this.size] = entry;
        }

        private void Resize()
        {
            var next = new FlexProperty[this.properties.Length + growth];
            Array.Copy(this.properties, next, this.properties.Length);

            this.properties = next;
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.size; i++)
                yield return this.properties[i];
        }

        int IList<FlexProperty>.IndexOf(FlexProperty item)
        {
            return Array.IndexOf(this.properties, item);
        }

        void IList<FlexProperty>.Insert(int index, FlexProperty item)
        {
            var code = item.Name.GetHashCode();
            var name = item.Name;
            for (var i = 0; i < this.size; i++)
            {
                var next = this.properties[i];
              
                if (name == next.Name)
                {
                    throw new Exception("Key already exists");
                }
            }

            Array.Copy(this.properties, index, this.properties, index + 1, size - index);
            this.properties[index] = item;
        }



        void IList<FlexProperty>.RemoveAt(int index)
        {
            this.size--;
            Array.Copy(this.properties, index + 1, this.properties, index, this.size - index);
        }

        void ICollection<FlexProperty>.Add(FlexProperty item)
        {
            for (var i = 0; i < this.size; i++)
            {
                var next = this.properties[i];
                if (item == next)
                {
                    throw new Exception("Key already exists");
                }
            }

            if(this.properties.Length == this.size)
                this.Resize();

            this.properties[this.size] = item;
        }

        void ICollection<FlexProperty>.Clear()
        {
            this.size = 0;
            this.properties = Array.Empty<FlexProperty>();
        }

        bool ICollection<FlexProperty>.Contains(FlexProperty item)
        {
            return this.properties.Contains(item);
        }

        void ICollection<FlexProperty>.CopyTo(FlexProperty[] array, int arrayIndex)
        {


            Array.Copy(this.properties, 0, array, arrayIndex, this.size);
        }

        bool ICollection<FlexProperty>.Remove(FlexProperty item)
        {
            var index = Array.IndexOf(this.properties, item);
            if (index == -1)
                return false;

            ((IList<FlexProperty>)this).RemoveAt(index);

            return true;
        }

        IEnumerator<FlexProperty> IEnumerable<FlexProperty>.GetEnumerator()
        {
            for (int i = 0; i < this.size; i++)
                yield return this.properties[i];
        }

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new FlexObjectMetaObject(parameter, this);
        }


        void IFlexObject.Set(string key, object value)
        {
            this.Set(key, value);
        }

        object IFlexObject.Get(string key)
        {
            return this.Get(key);
        }

        public override object Unbox()
        {
            var dictionary = new Dictionary<string, object>();
            foreach(var property in this.properties)
            {
                dictionary.Add(property.Name.ToString(), property.Value.Unbox());
            }

            return dictionary;
        }
    }


    public class FlexObjectMetaObject : System.Dynamic.DynamicMetaObject
    {
        private const string SetMethod = "Set";


        public FlexObjectMetaObject(Expression exppression, IFlexObject value):base(exppression, BindingRestrictions.Empty, value)
        {

        }


        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            // Method call in the containing class:
            string methodName = "Get";

            // One parameter
            var args = new Expression[]
            {
                Expression.Constant(binder.Name)
            };

            var call = Expression.Call(
                            Expression.Convert(Expression, LimitType),
                            typeof(IFlexObject).GetMethod(methodName), args);

            var get = new DynamicMetaObject(
                call,
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return get;
        }        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            var indexProperty = typeof(IFlexObject).GetProperty("Item");
            var keyParam = Expression.Parameter(typeof(string));

            IndexExpression indexExpr = Expression.Property(Expression.Convert(Expression, LimitType), indexProperty, keyParam);



            var get = new DynamicMetaObject(
               indexExpr,
               BindingRestrictions.GetTypeRestriction(Expression, LimitType));

            return get;
        }        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            var indexProperty = typeof(IFlexObject).GetProperty("Item");
            var keyParam = Expression.Parameter(typeof(string));

            IndexExpression indexExpr = Expression.Property(Expression.Convert(Expression, LimitType), indexProperty, keyParam);
           
            var assign = Expression.Assign(indexExpr, Expression.Convert(value.Expression, typeof(object)));

            var set = new DynamicMetaObject(
               indexExpr,
               BindingRestrictions.GetTypeRestriction(Expression, LimitType));

            return set;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);


            var args = new Expression[2];

            args[0] = Expression.Constant(binder.Name);
            args[1] = Expression.Convert(value.Expression, typeof(object));

            Expression self = Expression.Convert(Expression, LimitType);

            Expression methodCall = Expression.Call(self,
                typeof(IFlexObject).GetMethod(SetMethod),
                args);

            DynamicMetaObject set = new DynamicMetaObject(
                methodCall,
                restrictions);


            // return that dynamic object
            return set;
        }
    }
}
