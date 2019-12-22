using System;
using System.Collections;
using YamlDotNet.RepresentationModel;
using NerdyMishka.Reflection;
using NerdyMishka.Reflection.Extensions;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.Text;
using NerdyMishka.ComponentModel.DataAnnotations;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.Extensions.Flex 
{
    public class YamlObjectWriter
    {
        private ITextTransform propertyTransform = new CamelCaseTransform();

        private IFlexSerializationSettings settings;

        public YamlObjectWriter(IFlexSerializationSettings settings = null)
        {
            this.settings = settings ?? new FlexSerializationSettings();
        }

        public YamlNode Visit<T>(T value)
        {
            return this.Visit(value, null, typeof(T).AsTypeInfo());
        }

        public YamlNode Visit(object value)
        {
            if(value == null)                
            {
                if(this.settings.OmitNulls)
                    return null;

                return new YamlScalarNode("null");
            }
            
            return this.Visit(value, null, value.GetType().AsTypeInfo());
        }

        public YamlNode Visit(object value, IProperty property, IType type)
        {
            if(property != null && type == null)
                type = property.ClrType.AsTypeInfo();

            if(type.IsDataType)
                return this.VisitValue(value, property, type);

            if(type.IsDictionaryLike())
                return this.VisitDictionary((IDictionary)value, property, type);

            if(type.IsListLike())
                return this.VisitArray((IList)value, property, type);

            return this.VisitComplexObject(value, property, type);
        }
        
        private string ConvertToSymbol(string key)
        {
            var symbol = key;
            if(this.propertyTransform != null)
                symbol = this.propertyTransform.Transform(symbol).ToString();

            if(string.IsInterned(symbol) == null)
                symbol = string.Intern(symbol);

            return symbol;
        }
        

        public YamlNode VisitComplexObject(object @object, IProperty propertyInfo, IType typeInfo)
        {
            if(@object == null)
                return null;

            if(propertyInfo != null && typeInfo == null)
                typeInfo = propertyInfo.ClrType.AsTypeInfo();

            if(typeInfo == null)
                typeInfo = @object.GetType().AsTypeInfo();

            if(typeInfo.IsDictionaryLike())
                return this.VisitDictionary((IDictionary)@object, propertyInfo, typeInfo);

            var node = new YamlMappingNode();
            var properties = typeInfo.Properties.ToList();
            var defaultProperty = properties.FirstOrDefault(o => 
                o.Attributes
                .Any(a => a is DynamicDefaultAttribute));
            
            if(defaultProperty != null)
            {
                var value = defaultProperty.GetValue(@object);
                if(value != null)
                {
                    return this.VisitValue(value, defaultProperty, defaultProperty.ClrType.AsTypeInfo());
                }

                properties.Remove(defaultProperty);
            }

            foreach(var property in properties)
            {
                if(property.Attributes.Any(o => o is IgnoreAttribute)) 
                    continue;

                if(!property.CanWrite)
                    continue;

                var value = property.GetValue(@object);
                
                var nextNode = this.Visit(value, property, property.ClrType.AsTypeInfo());
                if(nextNode == null)
                {
                    if(this.settings.OmitNulls)
                        continue;

                    nextNode = new YamlScalarNode("null");
                }

                node.Add(this.ConvertToSymbol(property.Name), nextNode);                
            }

            return node;
        }

        public YamlNode VisitArray(IList list, IProperty property, IType type)
        {
            if(list == null)
                return null;

            var node = new YamlSequenceNode();
            if(property != null && type == null)
                type = property.ClrType.AsTypeInfo();

            if(type == null)
                type = list.GetType().AsTypeInfo();

            if(!type.IsListLike())
                throw new InvalidCastException($" {type.FullName} is not a list or an array");

            var childType = type.AsItemType()?.ItemType;
            if(childType == null)
                childType = typeof(object).AsTypeInfo();

            for(var i =  0; i < list.Count; i++)
            {
                var nextItem = list[i];
                var nextValue = this.Visit(nextItem, null, childType);
                if(nextValue == null)
                {
                    if(this.settings.OmitNulls)
                        continue;

                    nextValue = new YamlScalarNode("null");
                }
                node.Add(nextValue);
            }

            return node;
        }

        public YamlNode VisitDictionary(IDictionary dictionary, IProperty property, IType type)
        {
            if(dictionary == null)
                return null;

            var node = new YamlMappingNode();
            if(property != null && type == null)
                type = property.ClrType.AsTypeInfo();

            if(type == null)
                type = dictionary.GetType().AsTypeInfo();

            var childType = type.AsItemType()?.ItemType;
            if(childType == null)
                childType = typeof(object).AsTypeInfo();

            foreach(var key in dictionary.Keys)
            {
                var symbol = this.ConvertToSymbol(key.ToString());
                var value = dictionary[key];
                var nextNode = this.Visit(value, null, childType);
                if(nextNode == null)
                {
                    if(this.settings.OmitNulls)
                        continue;

                    nextNode = new YamlScalarNode("null");
                }

                node.Add(symbol, nextNode);
            }

            return node;
        }

        public YamlNode VisitValue(object value, IProperty propertyInfo, IType typeInfo)
        {
            if(value == null)
                return null;

            IReadOnlyCollection<ValueConverter> converters = null;
            string propertyName = "unknown property";
            if(propertyInfo != null)
            {
                propertyName = propertyInfo.Name;
                if(typeInfo == null)
                    typeInfo = propertyInfo.ClrType.AsTypeInfo();

                converters = propertyInfo.GetValueConverters(typeof(string));
            
                foreach(var converter in converters)
                {
                    if(converter.CanConvertFrom(typeInfo.ClrType) && converter.CanConvertTo(typeof(string)))
                        return new YamlScalarNode(converter.ConvertTo(value).ToString());
                }
            }

            if(typeInfo == null)
                typeInfo = value.GetType().AsTypeInfo();

            if(this.settings?.ValueConverters != null && this.settings.ValueConverters.Count > 0)
            {
                foreach(var converter in this.settings.ValueConverters)
                {
                    // Property (From) to DataStore (To)
                    if(converter.CanConvertFrom(typeInfo.ClrType) && converter.CanConvertTo(typeof(string)))
                        return new YamlScalarNode(converter.ConvertFrom(value).ToString());
                }
            }

            
            switch(value)
            {
                case byte[] bytes:
                    return new YamlScalarNode(Convert.ToBase64String(bytes));
                
                case char[] chars:
                    return new YamlScalarNode(new string(chars));
                
                default:
                    return new YamlScalarNode(value.ToString());
            }
        }
    }
}