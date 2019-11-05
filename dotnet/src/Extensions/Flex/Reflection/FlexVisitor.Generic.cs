using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace NerdyMishka.Flex.Reflection
{
    public abstract partial class FlexVisitor<TBase, TValue, TObject, TArray, TDocument>  
        where TValue: TBase
        where TObject: TBase, new()
        where TArray: TBase,  IEnumerable<TBase>, new()
    {
       

        public FlexSettings FlexSettings { get; set; }

        public abstract string Name { get; }
    

        public virtual TDocument VisitComplexObject(object value)
        {
            var node = this.Visit(value);
            var doc = this.CreateDocument(node);
            return doc;
        }

      

        public virtual object VisitDocument(TDocument value, Type type)
        {
            var typeDef = TypeInspector.GetTypeInfo(type);
            return this.Visit(this.GetRootNode(value), typeDef, null);
        }

        public T VisitDocument<T>(TDocument value)
        {
            return (T)this.VisitDocument(value, typeof(T));
        }

        protected abstract bool IsNull(TValue documentValue);

        protected abstract string GetValue(TValue documentValue);

        protected abstract void SetValue(TValue documentValue, object value);

        protected abstract TValue CreateValue(string value);
    

        protected abstract void AddChild(TObject documentElement, string name, object value);

        protected abstract void AddChild(TArray documentArray, TBase value);

        protected abstract TDocument CreateDocument(TBase root);

        protected abstract TBase GetRootNode(TDocument document);


        protected abstract IEnumerator<TBase> GetEnumerator(TArray array);

        public virtual TObject VisitMap(IDictionary value, FlexTypeDefinition definition)
        {
            if (value == null)
                return default(TObject);

            var node = new TObject();
            foreach(var key in value.Keys)
            {
                var clrValue = value[key];
                var child = this.Visit(clrValue);
                if (child != null)
                    this.AddChild(node, key.ToString(), child);
            }

            return node;
        }

        public virtual TObject VisitComplexObject(object value, FlexTypeDefinition definition)
        {
            if (value == null)
                return default(TObject);

           
            var def = definition;
            var node = new TObject();
           
            var properties = def.Properties;

               
            foreach(var nextPropSet in properties)
            {
                var propertyTypeInfo = nextPropSet.Value;

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var propValue = propertyTypeInfo.Info.GetValue(value);
                var propertyClassInfo = TypeInspector.GetTypeInfo(propertyTypeInfo.Info.PropertyType);

                if(propertyClassInfo.IsDataType)
                {
                    var scalar = this.VisitProperty(propValue, 
                        propertyTypeInfo);

                    if(scalar != null)
                    {
                        if (this.IsNull(scalar) && this.FlexSettings.OmitNulls)
                            continue;

                       this.AddChild(node, nextPropSet.Key, scalar);
                    }
                    continue;
                }

                var valueNode = this.Visit(propValue, propertyClassInfo);
                if(valueNode != null)
                {
                    this.AddChild(node, nextPropSet.Key, valueNode);
                }
            }

            return node;
        }

        public virtual TArray VisitArray(IList value, FlexTypeDefinition definition)
        {
              if (value == null)
                return default(TArray);

            var def = definition;       
            var valueTypeInfo = TypeInspector.GetTypeInfo(def.ValueType);
            var node = new TArray();

            foreach(var item in value)
            {
                var nextNode = this.Visit(item, valueTypeInfo);
                if (nextNode != null)
                    this.AddChild(node, nextNode);
            }

            return node;
        }

        public virtual TValue VisitProperty(object value, FlexPropertyDefinition definition)
        {
            var v = this.VisitValue(value, definition);
            if(v == null && this.FlexSettings.OmitNulls)
                return default(TValue);


            return this.CreateValue(v ?? "null");
        }

        public virtual TBase Visit(object value)
        {
            if (value == null)
                return default(TBase);

            var def = TypeInspector.GetTypeInfo(value.GetType());
            return this.Visit(value, def);
        }

        public virtual TBase Visit(object value, FlexTypeDefinition definition)
        {
             if (value == null)
                return default(TBase);

            var def = definition;

            if(def.IsDataType)
            {
                var v = this.VisitValue(value, null);
                var scalar = this.CreateValue(v);
               
                if(v == null)
                {
                    if(this.FlexSettings.OmitNulls)
                        return default(TBase);

                    this.SetValue(scalar, "null");
                }

                return scalar;
            }
           

            if (def.IsDictionary)
                return this.VisitMap((IDictionary)value, def);

            if (def.IsList)
                return this.VisitArray((IList)value, def);

            var properties = def.Properties;
            var set =  properties.FirstOrDefault(o => o.Value.IsSwitch &&
                o.Value.Switch.IsDefault);
            
           if(set.Value != null)
            {
                var switchValue  = (bool?)set.Value.Info.GetValue(value);
                if(switchValue.HasValue && !switchValue.Value)
                {
                    var nextNode = this.CreateValue(set.Value.Switch.No);
                    return nextNode;
                }
            }

            set = properties.FirstOrDefault(o => o.Value.IsDefault);

            if(set.Value != null)
            {
                var defaultValue = set.Value.Info.GetValue(value);
                if(defaultValue != null)
                {
                    var nextNode = this.CreateValue(defaultValue.ToString());
                    return nextNode;
                }
            }

           
            return this.VisitComplexObject(value, def);
        }

        public virtual string VisitValue(object value, FlexPropertyDefinition definition)
        {
            var propDef = definition;
            var cryptoProvider = this.FlexSettings.CryptoProvider;
            FlexTypeDefinition typeDef = null;
            if(propDef != null)
                typeDef = propDef.TypeDefinition;
            else if(value != null)
                typeDef = TypeInspector.GetTypeInfo(value.GetType()); 

            switch (value)
            {
                case null:
                    return "null";
                case byte[] bytes:
                    if (propDef != null && propDef.IsEncrypted && cryptoProvider != null)
                    {
                        bytes = cryptoProvider.EncryptBlob(bytes);
                    }
                    return Convert.ToBase64String(bytes);
                case char[] chars:
                    if (propDef != null && propDef.IsEncrypted && cryptoProvider != null)
                    {
                        var bytes = System.Text.Encoding.UTF8.GetBytes(chars);
                        bytes = cryptoProvider.EncryptBlob(bytes);
                        return Convert.ToBase64String(bytes);
                    }
                    return new string(chars);
                case System.Security.SecureString ss:
                    {
                        var bytes = ConvertToBytes(ss);

                        if(propDef != null && propDef.IsEncrypted && cryptoProvider != null)
                        {
                             bytes = cryptoProvider.EncryptBlob(bytes);
                             return Convert.ToBase64String(bytes);
                        }

                        return Convert.ToBase64String(bytes);
                    }
                  
                case string stringValue:
              
                    if (propDef != null && propDef.IsEncrypted && cryptoProvider != null)
                    {
                        return cryptoProvider.EncryptString(stringValue);
                    }

                    if(stringValue == null)
                        return "null";

                    return stringValue;
                case DateTime dt:
                    var defaultFormat = this.FlexSettings.DefaultDateTimeOptions;
                    if (propDef != null && propDef.DateTimeFormats != null)
                    {
                        var format = propDef.DateTimeFormats.FirstOrDefault(o =>
                                    o.Provider != null && o.Provider == this.Name);

                        if (format == null)
                        {
                            format = propDef.DateTimeFormats.FirstOrDefault(o => o.Provider == null);

                            if(format == null && this.FlexSettings.DateTimeOptions.Count > 0)
                            {
                                var format2 = this.FlexSettings.DateTimeOptions.FirstOrDefault(o =>
                                    o.Provider != null && o.Provider == this.Name);

                                if(format2 == null)
                                {
                                    format2 = this.FlexSettings.DateTimeOptions.FirstOrDefault(
                                        o => o.Provider == null);
                                }

                                if(format2 != null)
                                {
                                    format = new DateTimeFormatAttribute() {
                                        Format = format2.Format,
                                        IsUtc = format2.IsUtc,
                                        Provider = format2.Provider,
                                        Name = format2.Name
                                    };
                                }
                            }
                        }

                        if (format != null)
                            return dt.ToString(format.Format);
                    }

                    if (defaultFormat.IsUtc)
                        return dt.ToUniversalTime().ToString(defaultFormat.Format);

                    return dt.ToString(defaultFormat.Format);
                case Boolean b:
                    if (propDef != null && propDef.IsSwitch)
                    {
                        if (b)
                            return propDef.Switch.Yes;
                        return propDef.Switch.No;
                    }

                    return  b ? "true" : "false";
     
                default:
                    return value.ToString();
            }
        }


        public virtual object Visit(TBase node, FlexTypeDefinition typeDefinition = null, FlexPropertyDefinition propertyDefinition = null)
        {
            switch(node)
            {
                case TObject map:
                    return this.VisitDocumentElement(map, typeDefinition);

                case TArray array:
                    return this.VisitDocumentArray(array, typeDefinition);

                case TValue property:
                    return this.VisitDocumentProperty(property, typeDefinition, propertyDefinition);

                default:
                    throw new NotSupportedException($"{node.GetType().FullName}"); 
            }
        }

        public T Visit<T>(TBase node)
        {
            return (T)this.Visit(node, typeof(T));
        }

        public object Visit(TBase node, Type castType)
        {
            var typeDef = TypeInspector.GetTypeInfo(castType);
            return this.Visit(node, typeDef);
        }

       


        public virtual IList VisitDocumentArray(TArray value, FlexTypeDefinition definition)
        {
            var node = value;
            var def = definition;
            
            if (!def.IsList && !def.IsArray)
                throw new Exception($"Mapping Mismatch: {def.Type.FullName}");

            IList list = null;
            if (def.IsArray)
            {
                var listType = typeof(List<>).MakeGenericType(new[] { def.ValueType });
                list = (IList)Activator.CreateInstance(listType);
            }
            else
            {
                list = (IList)Activator.CreateInstance(def.Type);
            }

            foreach(var nextNode in node)
            {
                var nextClassInfo = TypeInspector.GetTypeInfo(def.ValueType);

                var obj = this.Visit(nextNode, nextClassInfo, null);
                list.Add(obj);
            }

            return list;
        }

    


    

        public virtual object VisitDocumentElement(TObject value, FlexTypeDefinition definition)
        {
            if (value == null)
                return null;

           
            var def = definition;
            var node = new TObject();
            var properties = def.Properties;

               
            foreach(var nextPropSet in properties)
            {
                var propertyTypeInfo = nextPropSet.Value;

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var propValue = propertyTypeInfo.Info.GetValue(value);
                var propertyClassInfo = TypeInspector.GetTypeInfo(propertyTypeInfo.Info.PropertyType);

                if(propertyClassInfo.IsDataType)
                {
                    var scalar = this.VisitProperty(propValue, 
                        propertyTypeInfo);

                    if(scalar != null)
                    {
                        if (this.IsNull(scalar) && this.FlexSettings.OmitNulls)
                            continue;

                        this.AddChild(node, nextPropSet.Key, scalar);
                    }
                    continue;
                }

                var valueNode = this.Visit(propValue, propertyClassInfo);
                if(valueNode != null)
                {
                    this.AddChild(node, nextPropSet.Key, valueNode);
                }
            }

            return node;
        }


        public virtual object VisitDocumentProperty(
            TValue value, 
            FlexTypeDefinition typeDefinition, 
            FlexPropertyDefinition propertyDefinition)
        {
            return this.VisitDocumentValue(this.GetValue(value), typeDefinition, propertyDefinition);
        }
        
        
        public virtual object VisitDocumentValue(
            string value, 
            FlexTypeDefinition valueDefinition = null, 
            FlexPropertyDefinition propertyDefinition = null)
        {
            if(valueDefinition == null && propertyDefinition == null)
                throw new ArgumentException("Either valueDefinition or propertyDefinition must have a value");

            if(valueDefinition == null)
                valueDefinition = propertyDefinition.TypeDefinition;

            if(valueDefinition == null)
                valueDefinition = TypeInspector.GetTypeInfo(propertyDefinition.Info.PropertyType);            
    
            if (!valueDefinition.IsDataType)
                throw new Exception($"Mapping Exception, value type must be a data type: {valueDefinition.Type.FullName}");

            var cryptoProvider = this.FlexSettings.CryptoProvider;

            switch (valueDefinition.Type.FullName)
            {
                
                case "System.Byte[]":
                    if(propertyDefinition != null && propertyDefinition.IsEncrypted && cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = cryptoProvider.DecryptBlob(bytes);
                        return bytes;
                    }

                    return Convert.FromBase64String(value);
                case "System.Char[]":
                    if (propertyDefinition != null && propertyDefinition.IsEncrypted && cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = cryptoProvider.DecryptBlob(bytes);
                        return Encoding.UTF8.GetChars(bytes);
                    }
                    return value.ToCharArray();
                case "System.String":
                    if(value != null && value.ToLower() == "null")
                        return null;

                    if (propertyDefinition != null && propertyDefinition.IsEncrypted && cryptoProvider != null)
                    {
                        return cryptoProvider.DecryptString(value);
                    }

                    

                    return value;
                case "System.Security.SecureString":
                    var ss = new System.Security.SecureString();
                    if(propertyDefinition != null && propertyDefinition.IsEncrypted && cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = cryptoProvider.DecryptBlob(bytes);
                        var chars = Encoding.UTF8.GetChars(bytes);
                       
                        foreach(var c in chars)
                            ss.AppendChar(c);

                        return ss;
                    }
                    if(value == null || value.Length == 0)
                        return ss;

                    for(var i = 0; i < value.Length; i++) {
                        ss.AppendChar(value[i]);
                    }

                    return ss;
                case "System.Boolean":
                case "System.Nullable[System.Boolean]":
                    switch (value)
                    {
                        case "":
                        case null:
                        case "null":
                            if (valueDefinition.Type.FullName != "System.Nullable[System.Boolean]")
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
                            if(propertyDefinition.IsSwitch)
                            {
                                if(value != null && value.ToLower() == propertyDefinition.Switch.Yes)
                                    return true;
                            }

                            return false;
                    }
                default:
                    if (valueDefinition.IsNullable)
                    {
                        if (string.IsNullOrWhiteSpace(value) || value.ToLower() == "null")
                        {
                            return null;
                        }

                        return Convert.ChangeType(value, valueDefinition.ValueType);
                    }

                    if(value == "null"  || value == null)
                        throw new Exception("I am null");

                    return Convert.ChangeType(value, valueDefinition.Type);
            }
        }

        private static byte[] ConvertToBytes(System.Security.SecureString secureString)
        {
            if (secureString != null)
            {
                IntPtr bstr = IntPtr.Zero;
                char[] charArray = new char[secureString.Length];

                try
                {
                    
                    bstr = Marshal.SecureStringToBSTR(secureString);
                    Marshal.Copy(bstr, charArray, 0, charArray.Length);

                    return Encoding.UTF8.GetBytes(charArray);
                }
                finally
                {
                    Array.Clear(charArray, 0, charArray.Length);
                    Marshal.ZeroFreeBSTR(bstr);
                }
            }

            return new byte[0];
        }
    } 
}