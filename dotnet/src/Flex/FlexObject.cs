using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexObject : FlexMeta
    {
        private FlexProperty[] properties;
        private int size = 0;
        private int growth = 10;
        private static readonly StringComparer stringComparer = new Comparer();

        public override FlexType FlexType => FlexType.FlexObject;

        public FObject this[string propertyName]
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

        protected FObject Get(string key)
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
            if (this.properties.Length == this.size)
                this.Resize();

      

            var entry = new FlexProperty(key, value);

            for (var i = 0; i < this.size; i++)
            {
                var next = this.properties[i];
                if (next.GetHashCode() == entry.GetHashCode() && next.Name.Equals(entry.Name))
                {
                    next.Value = entry.Value;
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


      

        public override object Unbox()
        {
            return null;
        }

        public class FlexProperty: FlexMeta
        {
            public FlexProperty(string name, object value)
            {
                this.Name = name;
                this.Value = FlexConverter.ToFObject(value);
            }

            public FlexSymbol Name { get; protected set; }

            public FObject Value { get; set; }

            public bool Equals(FlexProperty property)
            {
                return this.Name == property.Name && property.Value.Equals(this.Value);
            }

            public override bool Equals(object obj)
            {
                if (obj is FlexProperty)
                    return this.Equals((FlexProperty)obj);
                
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode() + this.Value.GetHashCode();
            }

            public override object Unbox()
            {
                return new KeyValuePair<string, object>(this.Name, this.Value.Unbox());
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
                return obj.GetHashCode();
            }
        }
    }

}
