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
    public class YamlObjectReader
    {
        private ITextTransform propertyTransform = new PascalCaseTransform();

        private IFlexSerializationSettings settings;

        public YamlObjectReader(IFlexSerializationSettings settings = null)
        {
            this.settings = settings ?? new FlexSerializationSettings();
        }


        public object Visit(YamlNode node, IProperty propertyInfo, IType typeInfo)
        {
            switch(node)
            {
                case YamlMappingNode element:
                    return this.VisitElement(element, propertyInfo, typeInfo);
                case YamlSequenceNode array:
                    return this.VisitArray(array, propertyInfo, typeInfo);
                case YamlScalarNode value:
                    return this.VisitValue(value, propertyInfo, typeInfo);
            }

            return null;
        }

        public T VisitDocument<T>(YamlDocument document)
        {
            return (T)this.VisitDocument(document, typeof(T).AsTypeInfo());
        }

        public object VisitDocument(YamlDocument document, Type clrType)
        {
            return this.VisitDocument(document, clrType.AsTypeInfo());
        }

        internal object VisitDocument(YamlDocument document, IType typeInfo)
        {
            var rootNode = document.RootNode;
            return this.Visit(rootNode, null, typeInfo);
        }


        public string VisitComment(YamlNode node)
        {
            return null;
        }


        public object VisitAttribute(YamlNode node, IType typeInfo)
        {
             return null;
        }


        public object VisitValue(YamlScalarNode node, IProperty property, IType type = null)
        {
            if(node.Value == "null")
                return null;

            IReadOnlyCollection<ValueConverter> converters = new List<ValueConverter>();
            string propertyName = "unknown property";
            Type clrType = type?.ClrType;
            if(property != null)
            {
                propertyName = property.Name;
                type = property.ClrType.AsTypeInfo();
                 
                converters = property.GetValueConverters(typeof(string));
               
                foreach(var converter in converters)
                {
                    if(converter.CanConvertFrom(clrType) && converter.CanConvertTo(typeof(string)))
                        return converter.ConvertFrom(node.Value);
                }
            }

            if(this.settings.ValueConverters != null && this.settings.ValueConverters.Count > 0)
            {
                foreach(var converter in this.settings.ValueConverters)
                {
                    // Property  => Store
                    if(converter.CanConvertFrom(clrType) && converter.CanConvertTo(typeof(string)))
                        return converter.ConvertFrom(node.Value);
                }
            }

          

            if(!(type.IsDataType || type.IsNullableOfT() && type.UnderlyingType.IsDataType))
                throw new MappingException("Scalar Nodes must be a data type");


            switch(type.FullName)
            {
                case "System.Byte[]":
                    return Convert.FromBase64String(node.Value);

                case "System.Char[]":
                    return node.Value.ToCharArray();

                case "System.String":
                    return node.Value;

                case "System.DateTime":
                    var dt = DateTime.Parse(node.Value, null, System.Globalization.DateTimeStyles.AssumeUniversal);
                    if(dt.Kind != DateTimeKind.Utc)
                        return dt.ToUniversalTime();

                    return dt;
                case "System.Boolean":
                case "System.Nullable[System.Boolean]":
                    switch (node.Value)
                    {
                        case "":
                        case null:
                        case "null":
                            if (type.FullName != "System.Nullable[System.Boolean]")
                                throw new InvalidCastException("Cannot convert null to boolean");
                            return null;
                       
                        case "y":
                        case "Y":
                        case "yes":
                        case "Yes":
                        case "YES":
                        case "true":
                        case "True":
                        case "TRUE":
                        case "on":
                        case "On":
                        case "ON":
                            return true;
                        default:
                            return false;
                    }
                default: 
                    bool isNull = string.IsNullOrWhiteSpace(node.Value) || node.Value.Match("null");
                    clrType = type.ClrType;
                    if(type.IsNullableOfT())
                    {
                        if(isNull)
                            return null;

                        clrType = type.UnderlyingType.ClrType;
                    }                    

                    if(isNull)
                        throw new MappingException($"{propertyName} is type {type.FullName} which must not be null.");
                
                    return Convert.ChangeType(node.Value, clrType);
            }
                

        }

        public class YamlPropertyEnumerator : IEnumerator<KeyValuePair<string, YamlNode>>
        {
          
            private YamlMappingNode node;

            private List<YamlScalarNode> keys;

            private int index = -1;
        

            public YamlPropertyEnumerator(YamlMappingNode node)
            {
                this.node = node;
                this.keys = this.node.Children.Keys
                    .Cast<YamlScalarNode>()
                    .ToList();
            }

            public KeyValuePair<string, YamlNode> Current 
            {
                get{
                    
                    var key = this.keys[this.index];
                    return new KeyValuePair<string, YamlNode>(key.Value, node.Children[key]);
                }
            }

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                this.node = null;
                this.keys = null;
            }

            public bool MoveNext()
            {
                if(this.index >= (this.keys.Count - 1))
                    return false;

                this.index = this.index + 1;
                return true;
            }

            public void Reset()
            {
                this.index = -1;
            }
        }

        private string ConvertToPropertyName(string key)
        {
            // TODO: cache property convertions in a dictionary with a max cap
            var propertyName = key;
            if(this.propertyTransform != null)
                propertyName = this.propertyTransform.Transform(propertyName).ToString();

            if(String.IsInterned(propertyName) == null)
                string.Intern(propertyName);
            
            return propertyName;
        }

        public object VisitElement(YamlMappingNode node, IProperty propertyInfo = null, IType typeInfo = null)
        {
            if(propertyInfo != null)
            {
                if(typeInfo == null)
                    typeInfo = propertyInfo.ClrType.AsTypeInfo();
            }
            
            var isDictionaryLike = typeInfo.IsIDictionaryOfKv() || typeInfo.IsIDictionary();
            var childType = typeInfo.AsItemType()?.ItemType;
            var clrType = childType?.ClrType;
            if(childType == null || clrType == typeof(Object))
            {
                clrType = typeof(string);
                childType = ReflectionCache.GetOrAdd(clrType);
            }

            if(isDictionaryLike)
            {
                var dictionary = (IDictionary)Activator.CreateInstance(typeInfo.ClrType);


                

                var enumerator = new YamlPropertyEnumerator(node);
                while(enumerator.MoveNext())
                {
                    var child = enumerator.Current;
                   
                    dictionary.Add(
                        child.Key, 
                        this.Visit(child.Value, null, childType));
                }

                return dictionary;
            }

            {
                var properties = typeInfo.Properties.ToList();
                var enumerator = new YamlPropertyEnumerator(node);
                
                var obj = Activator.CreateInstance(typeInfo.ClrType);
                var defaultProperty = properties
                    .FirstOrDefault(o => o.Attributes
                    .Any(a => a is DynamicDefaultAttribute));
                    
                if(defaultProperty != null)
                {
                    while(enumerator.MoveNext())
                    {
                        if(enumerator.Current.Key.Match(defaultProperty.Name))
                        {
                            var value = enumerator.Current.Value;
                            if(value != null)
                            {
                                return this.Visit(value, defaultProperty, defaultProperty.ClrType.AsTypeInfo());
                            }

                            properties.Remove(defaultProperty);
                            break;
                        }
                    }
                    enumerator.Reset();
                }

                if(typeInfo.IsDataType)
                    throw new MappingException("YamlMappingNode cannot be converted into a data type");

                while(enumerator.MoveNext())
                {
                    var child = enumerator.Current;
                    var propertyName = this.ConvertToPropertyName(child.Key);
                    var property = properties.FirstOrDefault(o => o.Name.Match(propertyName));
                    if(property == null)
                        continue;

                    if(!property.CanWrite)
                        continue;

                    if(property.Attributes.Any(o => o is IgnoreAttribute))
                        continue;

                    property.SetValue(obj, 
                        this.Visit(child.Value, property, property.ClrType.AsTypeInfo()));
                }

                return obj;
            }
        }


        public IList VisitArray(YamlSequenceNode node, IProperty propertyInfo, IType typeInfo)
        {
            if(propertyInfo != null)
            {
                if(typeInfo == null)
                    typeInfo = propertyInfo.ClrType.AsTypeInfo();
            }

            if(!typeInfo.IsListLike()) 
                throw new MappingException($"{typeInfo.FullName} is not an array or list like.");

            var childType = typeInfo.AsItemType()?.ItemType;
            var clrType = childType?.ClrType;
            if(childType == null)
            {
                clrType = typeof(Object);
                childType = ReflectionCache.GetOrAdd(clrType);
            }

            if(typeInfo.IsArray())
            {
                var array = (Array)Array.CreateInstance(clrType, node.Children.Count);
                for(var i = 0; i < node.Children.Count; i++) 
                {
                    var child = node.Children[i];
                    var value = this.Visit(child, null, childType);
                    array.SetValue(value, i);
                }

                return array;
            }

            var list = (IList)Activator.CreateInstance(typeInfo.ClrType);
            for(var i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                var value = this.Visit(child, null, childType);
                list.Add(value);
            }

            return list;
        }
    }
}