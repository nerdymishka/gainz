using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using NerdyMishka.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NerdyMishka.Reflection
{
    /*
    public class FlexPropertyInfo
    {
        private PropertyInfo propertyInfo;

        private FlexTypeInfo typeInfo;

        public string Name => this.propertyInfo.Name;

        private int Ordinal { get; set; }

        private IList<ValidationAttribute> validationAttributes = new List<ValidationAttribute>();

        private IList<IValueConverterAttribute> converterAttributes = new List<IValueConverterAttribute>();

        private IList<UIHintAttribute> uiHints = new List<UIHintAttribute>();

        public IEnumerable<ValidationAttribute> ValidationAttributes => this.validationAttributes;

        public IEnumerable<IValueConverterAttribute> ValueConverterAttributes => this.converterAttributes;

        public IEnumerable<UIHintAttribute> UIHints => this.uiHints;

        public DisplayAttribute DisplayAttribute  { get; private set; }

        public UIHintAttribute UiHintAttribute  { get; private set; }

        public IgnoreAttribute IgnoreAttribute { get; private set; }

        public DefaultValueAttribute DefaulValueAttribute { get; private set; }

        public DescriptionAttribute DescriptionAttribute { get; private set; }

        public KeyAttribute KeyAttribute { get; private set; }

        public EditableAttribute EditableAttribute { get; private set; }

        public PropertyInfo PropertyInfo => this.propertyInfo;

        public FlexTypeInfo FlexTypeInfo => this.typeInfo;


        public FlexPropertyInfo(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
            this.typeInfo = ReflectionCache.GetOrAdd(this.propertyInfo.PropertyType);
            
            var attrs = propertyInfo.GetCustomAttributes();
            foreach(var attr in attrs)
            {
                switch(attr)
                {
                    case ValidationAttribute validateAttr:
                        this.validationAttributes.Add(validateAttr);
                        break;

                    case IValueConverterAttribute valueConverterAttribute:
                        this.converterAttributes.Add(valueConverterAttribute);
                        break;

                    case DisplayAttribute displayAttribute:
                        this.DisplayAttribute = displayAttribute;
                        break;

                    case UIHintAttribute uiHintAttribute:
                        this.uiHints.Add(uiHintAttribute);
                        break;

                    case IgnoreAttribute ignoreAttribute:
                        this.IgnoreAttribute= ignoreAttribute;
                        break;

                    case DefaultValueAttribute defaultValueAttribute:
                        this.DefaulValueAttribute = defaultValueAttribute;
                        break;

                    case DescriptionAttribute descriptionAttribute:
                        this.DescriptionAttribute = descriptionAttribute;
                        break;

                    case KeyAttribute keyAttribute:
                        this.KeyAttribute = keyAttribute;
                        break;

                    case EditableAttribute editableAttribute:
                        this.EditableAttribute = editableAttribute;
                        break;
                }
            }
        }
    }
    */
}