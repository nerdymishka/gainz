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
                var attrProps = ReflectionCache.FindOrAdd(this.GetType()).Properties;
                var converterProps = ReflectionCache.FindOrAdd(this.ValueConverter)
                    .LoadProperties(true).Properties;

                var names = converterProps.Select(o => o.Name).ToList();
                foreach(var attrProp in attrProps)
                {
                    if(names.Contains(attrProp.Name))
                    {
                        var setter = converterProps.FirstOrDefault(o => o.Name == attrProp.Name);
                        if(setter.PropertyInfo.PropertyType == attrProp.PropertyInfo.PropertyType)
                        {
                            var value = attrProp.GetValue(this);
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


   