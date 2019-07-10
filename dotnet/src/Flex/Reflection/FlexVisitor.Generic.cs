using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace NerdyMishka.Flex.Reflection
{
    public abstract class FlexVisitor<TBase, TValue, TObject, TArray, TDocument>  
        where TValue: TBase
        where TObject: TBase
        where TArray: TBase
    {
       

        public FlexSettings FlexSettings { get; set; }

        public abstract string Name { get; }
    

        public abstract TDocument VisitDocument(object value);

        public abstract object VisitObject(TDocument value, Type type);

        public T VisitObject<T>(TDocument value)
        {
            return (T)this.VisitObject(value, typeof(T));
        }

        public abstract TObject VisitElement(IDictionary value, FlexTypeDefinition definition);

        public abstract TObject VisitElement(object value, FlexTypeDefinition definition);

        public abstract TArray VisitArray(IList value, FlexTypeDefinition definition);

        public abstract TValue VisitProperty(object value, FlexPropertyDefinition definition);

        public abstract TBase Visit(object value);

        public abstract TBase Visit(object value, FlexTypeDefinition definition);

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
                    return this.VisitElement(map, typeDefinition);

                case TArray array:
                    return this.VisitArray(array, typeDefinition);

                case TValue property:
                    return this.VisitProperty(property, propertyDefinition);

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

       


        public abstract IList VisitArray(TArray value, FlexTypeDefinition definition);

        public abstract object VisitElement(TObject value, FlexTypeDefinition definition);


        public abstract object VisitProperty(TValue value, FlexPropertyDefinition definition);
        
        
        public virtual object VisitValue(string value, FlexPropertyDefinition propertyDefinition, FlexTypeDefinition valueDefinition = null)
        {
            if(valueDefinition == null)
                valueDefinition = propertyDefinition.TypeDefinition;

            if(valueDefinition == null)
                valueDefinition = TypeInspector.GetTypeInfo(propertyDefinition.Info.PropertyType);            
    
            if (!valueDefinition.IsDataType)
                throw new Exception($"Mapping Exception: {valueDefinition.Type.FullName}");

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
                    if (propertyDefinition != null && propertyDefinition.IsEncrypted && cryptoProvider != null)
                    {
                        return cryptoProvider.DecryptString(value);
                    }

                    if(value != null && value.ToLower() == "null")
                        return null;

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