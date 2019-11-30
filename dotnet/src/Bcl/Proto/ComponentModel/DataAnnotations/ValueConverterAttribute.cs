using System;
using System.Linq;
using NerdyMishka.ComponentModel.ValueConversion;
using NerdyMishka.Reflection;

namespace NerdyMishka.ComponentModel.DataAnnotations

{

    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public abstract class ValueConverterAttribute : Attribute, IValueConverterAttribute
    {
        private bool initialized = false;

        public Type ValueConverter { get; set; }

        public int Position { get; set; } = int.MaxValue;

        public ValueConverter Instance { get; set; }



        public ValueConverterAttribute(Type valueConverter = null)
        {
            this.ValueConverter = valueConverter;
        }

        public virtual void Initialize()
        {
            if(initialized)
                return;

            if(this.ValueConverter != null && this.Instance == null)
            {
                var obj = (ValueConverter)Activator.CreateInstance(this.ValueConverter);
                var attrProps = ReflectionCache.GetOrAdd(this.GetType()).Properties;
                var converterProps = ReflectionCache.GetOrAdd(this.ValueConverter)
                    .LoadProperties(true).Properties;

                var names = converterProps.Select(o => o.Name).ToList();
                foreach(var prop in attrProps)
                {
                    if(names.Contains(prop.Name))
                    {
                        var setter = converterProps.FirstOrDefault(o => o.Name == prop.Name);
                        if(setter.PropertyInfo.PropertyType == prop.PropertyInfo.PropertyType)
                        {
                            var value = prop.GetValue(obj);
                            setter.SetValue(obj, value);
                        }
                        continue;
                    }
                }

                this.Instance = obj;
            }

            this.initialized = true;
        }
    }

}


   