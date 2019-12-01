

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionMember : IReflectionMember
    {
        private List<Attribute> attributes;
        
        private Dictionary<string, bool> flags;

        private Dictionary<string, object> metadata;

        public string Name { get; protected set; }

        public Type ClrType { get; protected set; }

        public IReflectionFactory Factory { get; set; }

        public Dictionary<string, bool> Flags 
        {
            get
            { 
                this.flags = this.flags ?? new Dictionary<string, bool>();
                return this.flags;
            }
        }

        public Dictionary<string, object> Metadata
        {
            get 
            { 
                this.metadata = this.metadata ?? new Dictionary<string, object>();
                return this.metadata;
            }
        }

        public T GetMetadata<T>(string name)
        {
            if(this.Metadata.TryGetValue(name, out object value))
                return (T)value;

            return default(T);
        }

        public IReflectionMember SetMetadata<T>(string name, T value)
        {
            this.Metadata[name] = value;
            return this;
        }

        public bool HasFlag(string flag)
        {
            if(this.Flags.TryGetValue(flag, out bool result))
                return result;

            return false;
        } 
        
        public IReflectionMember SetFlag(string flag, bool value)
        {
            this.Flags[flag] = value;
            return this;
        }


        public virtual IReadOnlyCollection<Attribute> Attributes 
        {
            get 
            {
                if(this.attributes == null)
                {
                    this.LoadAttributes();
                }
                return this.attributes;
            }
           
        }

        public virtual IReflectionMember LoadAttributes(bool inherit = true)
        {
            this.SetAttibutes(
                CustomAttributeExtensions.GetCustomAttributes(this.ClrType, inherit));
            
            return this;
        }

        protected void SetAttibutes(IEnumerable<Attribute> range)
        {
            this.attributes = new List<Attribute>(range);
        }

        public virtual T FindAttribute<T>() where T: Attribute
        {
            return (T)this.Attributes
                .FirstOrDefault(o => o is T);
        }

        public virtual IEnumerable<T> FindAttributes<T>() where T: Attribute
        {
            return this.Attributes.Where(o => o is T)
                .Cast<T>();
        }
    }
}