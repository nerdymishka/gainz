using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace NerdyMishka.Flex.Yaml
{
    public class YamlVisitor
    {
        private Dictionary<Type, ClassTypeInfo> typeInformation 
            = new Dictionary<Type, ClassTypeInfo>();

        public class ClassTypeInfo
        {
            public Type Type { get; set; }

            public bool IsList { get; set; }

            public bool IsDictionary { get; set; }

            public bool IsNullable { get; set; }

            public Type ValueType { get; set; }

            public Type KeyType { get; set; }

            public bool IsArray { get; set; }

            public Type ListType { get; set; }

            public Dictionary<string, PropertyTypeInfo> Properties { get; set; } = new Dictionary<string, PropertyTypeInfo>();
           
            public bool IsDataType { get; set; }
        }

        public class PropertyTypeInfo
        {
            private string symbol;

            public string Name => this.Info.Name;

            public PropertyInfo Info { get; set; }

            public DefaultPropertyAttribute Default { get; set; }

            public SwitchAttribute Switch { get; set; }

            public EncryptAttribute Encrypt { get; set; }

            public IgnoreAttribute Ignore { get; set; }

            public SymbolAttribute Symbol { get; set; }

            public DateTimeFormatAttribute[] DateTimeFormats { get; set; }

            public bool IsIgnored => this.Ignore != null;

            public bool IsEncrypted => this.Encrypt != null;

            public bool IsSwitch => this.Switch != null;

            public bool IsDefault => this.Default != null;

            public bool HasSymbol => this.Symbol != null;
        }

        private ClassTypeInfo GetTypeInfo(Type type)
        {
            if (this.typeInformation.TryGetValue(type, out ClassTypeInfo info))
                return info;



            info = new ClassTypeInfo()
            {
                Type = type
            };
            this.typeInformation.Add(type, info);

            if (type.IsGenericTypeDefinition && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                info.IsNullable = true;
                info.ValueType = type.GetGenericArguments()[0];
                info.IsDataType = true;
                return info;
            }
            var dataTypes = new[] { typeof(string), typeof(char[]), typeof(byte[]) };

            if(type.IsValueType || dataTypes.Contains(type) )
            {
                info.IsDataType = true;
                return info;
            }

            var interfaces = type.GetInterfaces();
            foreach(var contract in interfaces)
            {
                if(contract.IsGenericTypeDefinition)
                {
                    var typeDef = contract.GetGenericTypeDefinition();
                    if (typeDef == typeof(IDictionary<,>))
                    {
                        var types = contract.GetGenericArguments();
                        info.KeyType = types[0];
                        info.ValueType = types[1];
                        info.IsList = true;
                        break;
                    }

                    if (typeDef == typeof(IList<>))
                    {
                        var types = contract.GetGenericArguments();
                        info.ValueType = types[0];
                        info.IsList = true;
                        break;
                    }
                }

                if(contract is IDictionary)
                {
                    info.KeyType = typeof(object);
                    info.ValueType = typeof(object);
                    info.IsDictionary = true;
                    break;
                }

                if(contract is IList)
                {
                    info.ValueType = typeof(object);
                    info.IsList = true;
                    break;
                }
            }

            if(info.IsList || info.IsDictionary)
            {
                return info;
            }

            

            var propertyInfos = new List<PropertyTypeInfo>();
            var properties = type.GetProperties();
            foreach(var property in properties)
            {
                var propertyTypeInfo = new PropertyTypeInfo()
                {
                    Info = property
                };

                var dateTimes = new List<DateTimeFormatAttribute>();
                var attributes = property.GetCustomAttributes();
                foreach(var attribute in attributes)
                {
                    switch(attribute)
                    {
                        case SymbolAttribute symbol:
                            propertyTypeInfo.Symbol = symbol;
                            break;
                        case DefaultPropertyAttribute def:
                            propertyTypeInfo.Default = def;
                            break;
                        case EncryptAttribute encrypt:
                            propertyTypeInfo.Encrypt = encrypt;
                            break;
                        case SwitchAttribute swi:
                            propertyTypeInfo.Switch = swi;
                            break;
                        case IgnoreAttribute ignore:
                            propertyTypeInfo.Ignore = ignore;
                            break;
                        case DateTimeFormatAttribute dt:
                            dateTimes.Add(dt);
                            break;
                        default:
                            break;
                    }
                }

                propertyTypeInfo.DateTimeFormats = dateTimes.ToArray();

                string symbolName = null;
                if (propertyTypeInfo.HasSymbol)
                    symbolName = propertyTypeInfo.Symbol.Name;
                if (symbolName == null)
                    symbolName = propertyTypeInfo.Name;

                
                info.Properties.Add(symbolName, propertyTypeInfo);
            }

            return info;
        }

        public object Visit(YamlDocument node, Type expectedType)
        {
            return this.Visit(node.RootNode, expectedType);
        }

        public YamlNode Visit(object value)
        {
            if (value == null)
                return null;

            var info = this.GetTypeInfo(value.GetType());

            if (info.IsDictionary)
                return this.Visit((IDictionary)value, info, new YamlMappingNode());

            if (info.IsList)
                return this.Visit((IList)value, info, new YamlSequenceNode());
           
            return this.Visit(value, info, new YamlMappingNode());
        }

        public YamlNode Visit(IDictionary value, ClassTypeInfo info, YamlMappingNode node)
        {
            if (value == null)
                return null;

            foreach(var key in value.Keys)
            {
                var clrValue = value[key];
                var child = this.Visit(clrValue);
                if (child != null)
                    node.Add(key.ToString(), child);
            }

            return node;
        }

        public YamlNode Visit(object value, ClassTypeInfo info, YamlMappingNode node)
        {
            if (value == null)
                return null;

            var properties = info.Properties;

            var set =  properties.FirstOrDefault(o => o.Value.IsSwitch &&
            o.Value.Switch.IsDefault);

            if(set.Value != null)
            {
                var switchValue  = (bool?)set.Value.Info.GetValue(value);
                if(switchValue.HasValue && !switchValue.Value)
                {
                    var nextNode = new YamlScalarNode();
                    nextNode.Value = set.Value.Switch.No;
                    return nextNode;
                }
            }

            set = properties.FirstOrDefault(o => o.Value.IsDefault);

            if(set.Value != null)
            {
                var defaultValue = set.Value.Info.GetValue(value);
                if(defaultValue != null)
                {
                    var nextNode = new YamlScalarNode();
                    nextNode.Value = defaultValue.ToString();
                    return nextNode;
                }
            }

               
            foreach(var nextPropSet in properties)
            {
                var propertyTypeInfo = nextPropSet.Value;

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var propValue = propertyTypeInfo.Info.GetValue(value);
                var propertyClassInfo = this.GetTypeInfo(propertyTypeInfo.Info.PropertyType);

                if(propertyClassInfo.IsDataType)
                {
                    var nextNode = this.Visit(propValue, 
                         propertyTypeInfo,
                         propertyClassInfo,
                        new YamlScalarNode());
                    if(nextNode != null)
                    {
                        node.Add(nextPropSet.Key, nextNode);
                        
                    }
                    continue;
                }

                if(propertyClassInfo.IsDictionary)
                {
                    var nextNode = this.Visit((IDictionary)propValue, 
                        propertyClassInfo, 
                        new YamlMappingNode());

                    if(nextNode != null)
                    {
                        node.Add(nextPropSet.Key, nextNode);
                    }
                    continue;
                }

                if(propertyClassInfo.IsList)
                {
                    var nextNode = this.Visit((IList)propValue,
                        propertyClassInfo,
                        new YamlSequenceNode());

                    if(nextNode != null)
                    {
                        node.Add(nextPropSet.Key, nextNode);
                    }

                    continue;
                }

                var objNode = this.Visit(propValue,
                    propertyClassInfo,
                    new YamlMappingNode());

                if(objNode != null)
                {
                    node.Add(nextPropSet.Key, objNode);
                }
            }

            return node;
        }

        public YamlNode Visit(IList value, ClassTypeInfo info, YamlSequenceNode node)
        {
            if (value == null)
                return null;

           
            var valueTypeInfo = this.GetTypeInfo(info.ValueType);

            foreach(var item in value)
            {
                YamlNode next = null;
                if (valueTypeInfo.IsDataType)
                {
                    next = this.Visit(item, null, info, new YamlScalarNode());
                    if (next != null)
                    {
                        node.Add(next);
                        continue;
                    }
                }
                
                if(info.IsDictionary)
                {
                    next = this.Visit((IDictionary)item, valueTypeInfo, new YamlMappingNode());
                    if (next != null)
                    {
                        node.Add(next);
                        continue;
                    }
                }

                if(info.IsList)
                {
                    next = this.Visit((IList)item, valueTypeInfo, new YamlSequenceNode());
                    if (next != null)
                    {
                        node.Add(next);
                        continue;
                    }
                }


                var nextNode = this.Visit(item, valueTypeInfo, new YamlMappingNode());
                if (nextNode != null)
                    node.Add(node);
            }

            return node;
        }

        public YamlNode Visit(object value, PropertyTypeInfo propInfo, ClassTypeInfo classInfo, YamlScalarNode node)
        {
            switch(value)
            {
                case null:
                    return null;
                case byte[] bytes:
                    if (propInfo != null && propInfo.IsEncrypted)
                    {
                        // TODO: perform encryption.
                    }
                    node.Value = Convert.ToBase64String(bytes);
                    return node;
                case char[] chars:
                    if (propInfo != null && propInfo.IsEncrypted)
                    {
                        // TODO: perform encryption.
                    }
                    node.Value = new string(chars);
                    return node;
                case string stringValue:
                    if (propInfo != null && propInfo.IsEncrypted)
                    {
                        // TODO: perform encryption.
                    }

                    node.Value = stringValue;
                    return node;
                case DateTime dt:
                    if(propInfo != null && propInfo.DateTimeFormats != null)
                    {
                        var format = propInfo.DateTimeFormats.FirstOrDefault(o =>
                                    o.Provider != null && o.Provider == "yaml");
                        if (format == null)
                            format = propInfo.DateTimeFormats.FirstOrDefault();

                        node.Value = dt.ToString(format.Format);
                    }

                    node.Value = dt.ToString();
                    return node;
                case Boolean b:
                    if(propInfo != null && propInfo.IsSwitch)
                    {
                        if (b)
                            node.Value = propInfo.Switch.Yes;
                        else
                            node.Value = propInfo.Switch.No;

                        return node;
                    }

                    node.Value = b ? "true" : "false";
                    return node;
                default:
                    node.Value = value.ToString();
                    return node;
            }
        }

        



        public object Visit(YamlMappingNode node, Type expectedType)
        {
            var info = this.GetTypeInfo(expectedType);

            if (info.IsDataType || info.IsList)
            {
                throw new Exception("Invalid Mapping Exception");
            }

            IDictionary dictionary = null;
            object instance = Activator.CreateInstance(expectedType);
            if (info.IsDictionary)
                dictionary = (IDictionary)instance;

            foreach(YamlScalarNode child in node.Children.Keys)
            {
                var name = child.Value;
                var nextNode = node.Children[name];

                if(dictionary != null)
                {
                    var dictionaryValue = this.Visit(nextNode, info.ValueType);
                    dictionary.Add(name, dictionaryValue);
                    continue;
                }

                if (!info.Properties.TryGetValue(name, out PropertyTypeInfo propertyTypeInfo))
                    continue;

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var value = this.Visit(nextNode, propertyTypeInfo.Info.PropertyType);

                propertyTypeInfo.Info.SetValue(instance, value);
            }
            return instance;
        }

        public object Visit(YamlScalarNode node, Type expectedType)
        {
            var info = this.GetTypeInfo(expectedType);

            if (!info.IsDataType)
                throw new Exception("Mapping Exception");

            var instance = Activator.CreateInstance(expectedType);

            switch(instance)
            {
                case byte[] bytes:
                    return Convert.FromBase64String(node.Value);
                case char[] chars:
                    return node.Value.ToCharArray();
                case Boolean b:
                    switch(node.Value)
                    {
                        case "yes":
                        case "y":
                        case "Y":
                        case "Yes":
                        case "true":
                        case "on":
                            return true;
                        default:
                            return false;
                    }
                default:
                    if(info.IsNullable || info.Type == typeof(string))
                    {
                        if (string.IsNullOrWhiteSpace(node.Value) || node.Value.ToLower() == "null")
                        {
                            instance = null;
                            return instance;
                        }

                        instance = Convert.ChangeType(node.Value, info.ValueType);
                        return instance;
                    }

                    return Convert.ChangeType(node.Value, info.Type);
            }
        }

        public object Visit(YamlNode node, Type expectedType)
        {
            return null;
        }

        public object Visit(YamlSequenceNode node, Type expectedType)
        {
            var info = this.GetTypeInfo(expectedType);
            if (!info.IsList)
                throw new Exception("Mapping Mismatch");

            IList list = null;
            if (info.IsArray)
            {
                var listType = typeof(List<>).MakeGenericType(new[] { info.ValueType });
                list = (IList)Activator.CreateInstance(listType);
            }
            else
            {
                list = (IList)Activator.CreateInstance(info.Type);
            }

            for (int i = 0; i < node.Children.Count; i++)
            {
                var nextNode = node.Children[i];
                var value = this.Visit(nextNode, info.ValueType);
                list.Add(value);
            }

            return list;
        }
    }
}
