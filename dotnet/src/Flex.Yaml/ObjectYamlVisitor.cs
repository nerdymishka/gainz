using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YamlDotNet.RepresentationModel;

namespace NerdyMishka.Flex.Yaml
{
    public class ObjectYamlVisitor
    {
        private Dictionary<Type, ClassTypeInfo> typeInformation 
            = new Dictionary<Type, ClassTypeInfo>();

        private DateTimeFormatAttribute defaultFormat = 
            new DateTimeFormatAttribute("yyyy-MM-ddTHH:mm:ss.FFFFFFFK") { };

        private IFlexCryptoProvider cryptoProvider = null;

        private bool omitNulls = true;
        
        public ObjectYamlVisitor(IFlexCryptoProvider flexCryptoProvider = null)
        {
            this.cryptoProvider = flexCryptoProvider;
        }

        

        private ClassTypeInfo GetTypeInfo(Type type)
        {
            return TypeInspector.GetTypeInfo(type);
            /*
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
            var arrayTypes = new[] { typeof(char[]), typeof(byte[]) };
            var dataTypes = new[] { typeof(string), typeof(char[]), typeof(byte[]) };

            if(type.IsValueType || dataTypes.Contains(type) )
            {
                if (arrayTypes.Contains(type))
                    info.IsArray = true;

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
            */
        }

        public YamlDocument ToDoc(object value)
        {
            var node = this.Visit(value);
            var doc = new YamlDocument(node);
            return doc;
        }

        public T Visit<T>(YamlDocument doc)
        {
            return Visit<T>(doc.RootNode);
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
                    var scalar = (YamlScalarNode)this.Visit(propValue, 
                         propertyTypeInfo,
                         propertyClassInfo,
                        new YamlScalarNode());

                    if(scalar != null)
                    {
                        if (scalar.Value == null && omitNulls)
                            continue;

                        node.Add(nextPropSet.Key, scalar);
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
                    var scalar = (YamlScalarNode)this.Visit(item, null, info, new YamlScalarNode());
                    if (scalar != null)
                    {
                        if (scalar.Value == null && omitNulls)
                            continue;

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

        public virtual object Visit(YamlScalarNode node, PropertyTypeInfo propInfo, ClassTypeInfo classInfo = null)
        {
            if (propInfo != null)
                classInfo = this.GetTypeInfo(propInfo.Info.PropertyType);

            if (!classInfo.IsDataType)
                throw new Exception($"Mapping Exception: {classInfo.Type.FullName}");


       
            switch (classInfo.Type.FullName)
            {
                
                case "System.Byte[]":
                    if(propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(node.Value);
                        this.cryptoProvider.DecryptBlob(bytes);
                        return bytes;
                    }

                    return Convert.FromBase64String(node.Value);
                case "System.Char[]":
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        var decryptedString = this.cryptoProvider.DecryptString(node.Value);
                        return decryptedString.ToCharArray();
                    }
                    return node.Value.ToCharArray();
                case "System.String":
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        return this.cryptoProvider.DecryptString(node.Value);
                    }

                    return node.Value;
                case "System.Boolean":
                case "System.Nullable[System.Boolean]":
                    switch (node.Value)
                    {
                        case "":
                        case null:
                        case "null":
                            if (classInfo.Type.FullName != "System.Nullable[System.Boolean]")
                                throw new InvalidCastException("Cannot convert null to boolean");
                            return null;
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
                    if (classInfo.IsNullable)
                    {
                        if (string.IsNullOrWhiteSpace(node.Value) || node.Value.ToLower() == "null")
                        {
                            return null;
                        }

                        return Convert.ChangeType(node.Value, classInfo.ValueType);
                    }

                    return Convert.ChangeType(node.Value, classInfo.Type);
            }
        }

        public string Visit(object value, PropertyTypeInfo propInfo, ClassTypeInfo classInfo = null)
        {
            switch (value)
            {
                case null:
                    return null;
                case byte[] bytes:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        bytes = this.cryptoProvider.EncryptBlob(bytes);
                    }
                    return Convert.ToBase64String(bytes);
                case char[] chars:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        return this.cryptoProvider.EncryptString(new string(chars));
                    }
                    return new string(chars);
                case string stringValue:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        return this.cryptoProvider.EncryptString(stringValue);
                    }

                    return stringValue;
                case DateTime dt:
                    if (propInfo != null && propInfo.DateTimeFormats != null)
                    {
                        var format = propInfo.DateTimeFormats.FirstOrDefault(o =>
                                    o.Provider != null && o.Provider == "yaml");

                        if (format == null)
                            format = propInfo.DateTimeFormats.FirstOrDefault();

                        

                        if (format != null)
                            return dt.ToString(format.Format);
                    }

                    if (this.defaultFormat.IsUtc)
                        return dt.ToUniversalTime().ToString(this.defaultFormat.Format);

                    return dt.ToString(this.defaultFormat.Format);
                case Boolean b:
                    if (propInfo != null && propInfo.IsSwitch)
                    {
                        if (b)
                            return propInfo.Switch.Yes;
                        return propInfo.Switch.No;
                    }

                    return  b ? "true" : "false";
     
                default:
                    return value.ToString();
            }
        }

        public YamlNode Visit(object value, PropertyTypeInfo propInfo, ClassTypeInfo classInfo, YamlScalarNode node)
        {
            switch(value)
            {
                case null:
                    return null;
                case byte[] bytes:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        var encryptedBytes = this.cryptoProvider.EncryptBlob(bytes);
                        node.Value = Convert.ToBase64String(encryptedBytes);
                        return node;
                    }
                    node.Value = Convert.ToBase64String(bytes);
                    return node;
                case char[] chars:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                         var encryptedString = this.cryptoProvider.EncryptString(new string(chars));
                        node.Value = encryptedString;
                        return node;
                    }
                    node.Value = new string(chars);
                    return node;
                case string stringValue:
                    if (propInfo != null && propInfo.IsEncrypted && this.cryptoProvider != null)
                    {
                        var encryptedString = this.cryptoProvider.EncryptString(stringValue);
                        node.Value = encryptedString;
                        return node;
                    }

                    node.Value = stringValue;
                    return node;
                case DateTime dt:
                    if(propInfo != null && propInfo.DateTimeFormats != null && propInfo.DateTimeFormats.Length > 0)
                    {
                        var format = propInfo.DateTimeFormats.FirstOrDefault(o =>
                                    o.Provider != null && o.Provider == "yaml");
                        if (format == null)
                            format = propInfo.DateTimeFormats.FirstOrDefault(o => o.Provider == null);

                        
                        if(format != null)
                        {
                            node.Value = dt.ToString(format.Format);
                            return node;
                        }
                    }

                    node.Value = dt.ToString(defaultFormat.Format);
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

        
        


        public object Visit(YamlMappingNode node, ClassTypeInfo info)
        {
            

            if (info.IsDataType || info.IsList)
            {
                throw new Exception("Invalid Mapping Exception");
            }

            IDictionary dictionary = null;
            object instance = Activator.CreateInstance(info.Type);
            if (info.IsDictionary)
                dictionary = (IDictionary)instance;

            foreach(YamlScalarNode child in node.Children.Keys)
            {
                var name = child.Value;
                var nextNode = node.Children[name];
                
               
                if (!info.Properties.TryGetValue(name, out PropertyTypeInfo propertyTypeInfo))
                    throw new Exception(name);

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var childInfo = this.GetTypeInfo(propertyTypeInfo.Info.PropertyType);
                var value = this.Visit(nextNode, childInfo , propertyTypeInfo);

                if(dictionary != null)
                {
                   
                    dictionary.Add(name, value);
                    continue;
                }

                
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


        public object Visit(YamlNode node, ClassTypeInfo classInfo = null, PropertyTypeInfo propInfo = null)
        {
            
            switch(node)
            {
                case YamlMappingNode map:
                    return this.Visit(map, classInfo);
                case YamlSequenceNode seq:
                    return this.Visit(seq, classInfo);
                case YamlScalarNode scalar:
                    return this.Visit(scalar, propInfo,  classInfo);
                default:
                    throw new NotSupportedException($"{node.GetType().FullName}");
            }
        }

        public T Visit<T>(YamlNode node)
        {
            return (T)this.Visit(node, typeof(T));
        }

        public object Visit(YamlNode node, Type expectedType)
        {
            var classInfo = this.GetTypeInfo(expectedType);
            return this.Visit(node, classInfo);
        }

        public object Visit(YamlSequenceNode node, ClassTypeInfo classTypeInfo, PropertyTypeInfo propInfo = null)
        {
            var info = classTypeInfo;
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
                var nextClassInfo = this.GetTypeInfo(info.ValueType);
                var value = this.Visit(nextNode, nextClassInfo);
                list.Add(value);
            }

            return list;
        }
    }
}
