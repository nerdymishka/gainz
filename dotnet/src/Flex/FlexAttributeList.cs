using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexAttributeList : FObject,
        IEnumerable<FlexAttribute>
    {
        private FlexAttribute[] attributes;
        private int size = 0;
        private int growth = 5;
        private IEqualityComparer<string> comparer;

        public FlexAttributeList()
        {
            this.attributes = Array.Empty<FlexAttribute>();
            this.comparer = EqualityComparer<string>.Default;
        }

        public string this[string attributeName]
        {
            get {
                var attr = this.Get(attributeName);
                if (attr == null)
                    return null;

                return attr.Value;
            }
            set {
                this.Set(attributeName, value);
            }
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index > this.size)
                    return null;

                return this.attributes[index].Value;
            }
            set
            {
                if (index < 0 || index >= this.size)
                    throw new IndexOutOfRangeException(
                        $"Index {index} must be greater than 0 or less than or equal to {this.size}.");



                this.attributes[index].Value = value;
            }
        }

        public long Count => this.size;

       
        public bool TryGetAttribute(string attributeName, out FlexAttribute attribute)
        {
            var attr = this.Get(attributeName);
            if(attr != null)
            {
                attribute = attr;
                return true;
            }

            attribute = null;
            return false;
        }

        public bool Remove(string attributeName)
        {
            var index = this.IndexOf(attributeName);

            if(index > 1)
            {
                this.size--;
               
               
                Array.Copy(this.attributes, index+1, this.attributes, index, this.size - index);
                this.attributes[size] = null;
                return true;
            }

            return false;
        }

        public FlexAttribute Get(string attributeName)
        {
            var index = this.IndexOf(attributeName);
            if (index > -1)
                return this.attributes[index];

            return null;
        }

        public void Set(string attributeName, string value)
        {
            var index = this.IndexOf(attributeName);
            FlexAttribute attr = null;
            if (index == -1)
            {
                attr = new FlexAttribute(attributeName, value);
                
                this.Insert(this.size, attr, true);
                return;
            }

            attr = this.attributes[index];
            attr.Value = value;
        }

        protected void Insert(int index, FlexAttribute flexAttribute, bool add = false)
        {
            if (index < 0 || index > this.size)
                throw new IndexOutOfRangeException($"Index  {index} must be greater than 0 or less than or equal to {this.size}.");

          
            if(!add)
            {
                var existingIndex = this.IndexOf(flexAttribute.Name);
                if (existingIndex > -1 && existingIndex != index)
                    throw new Exception($"FlexAttribute with name {flexAttribute.Name} already exists at index {existingIndex}");
            }

           
            this.size++;
            this.Grow();

            if(add)
            {
                flexAttribute.HashCode = comparer.GetHashCode(flexAttribute.Name.ToString()) & 0x7FFFFFFF;
                this.attributes[index] = flexAttribute;
                return;
            }

            Array.Copy(this.attributes, index, this.attributes, index + 1, this.size - index);
            this.attributes[index] = flexAttribute;
        }

        protected int IndexOf(string attributeName)
        {
            // 0x7FFFFFFF; signed int max value
            // avoid negative indexing.
            var hashCode = comparer.GetHashCode(attributeName) & 0x7FFFFFFF;
            for (int i = 0; i < this.size; i++)
            {
                var attr = this.attributes[i];
                if (hashCode == attr.HashCode && attr.Name == attributeName)
                    return i;
            }

            return -1;
        }


        private void Grow()
        {
            if(this.size  >= this.attributes.Length)
            {
                var next = new FlexAttribute[this.size + this.growth];
                var l = Math.Min(this.attributes.Length, this.size);
                Array.Copy(this.attributes, next, l);
                this.attributes = next;
            }
        }

        private void Shrink()
        {
            if(this.attributes.Length - this.size >= this.growth)
            {
               
                var next = new FlexAttribute[this.attributes.Length - this.growth];
                Array.Copy(this.attributes, next, next.Length);

                this.attributes = next;
            }
        }

        public bool Contains(string attributeName)
        {
            if (this.size == 0)
                return false;

            return this.IndexOf(attributeName) != -1;
        }

        public IEnumerator<FlexAttribute> GetEnumerator()
        {
            for (int i = 0; i < this.size; i++)
            {
                yield return this.attributes[i];
            }
        }


        public override object Unbox()
        {
            var dictionary = new Dictionary<string, string>();
            for(var i = 0; i < this.size; i++)
            {
                var attr = this.attributes[i];
                dictionary.Add(attr.Name, attr.Value);

            }

            return dictionary;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
