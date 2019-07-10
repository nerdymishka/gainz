using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NerdyMishka.Flex.Reflection
{
    public abstract class FlexVisitor
    {
        private DateTimeFormatAttribute defaultFormat = 
            new DateTimeFormatAttribute("yyyy-MM-ddTHH:mm:ss.FFFFFFFK") { };

        private IFlexCryptoProvider cryptoProvider = null;

        private bool omitNulls = true;
        
        

        public abstract object VisitDocument(object value);

        public abstract object VisitElement(IDictionary value, FlexTypeDefinition definition);

        public abstract object VisitElement(object value, FlexTypeDefinition definition);

        public abstract object VisitArray(IList value, FlexTypeDefinition definition);

        public abstract string VisitValue(object value, FlexPropertyDefinition definition);


        public abstract IList VisitList(object value, FlexTypeDefinition definition);

        public abstract object VisitObject(object value, FlexTypeDefinition definition);

        public abstract IDictionary VisitDictionary(object value, FlexTypeDefinition definition);

        public abstract object VisitPropertyValue(object value, FlexTypeDefinition definition);

        
        public object VisitValue(string value, FlexPropertyDefinition propertyDefinition, FlexTypeDefinition valueDefinition = null)
        {
            if(valueDefinition == null)
                valueDefinition = TypeInspector.GetTypeInfo(propertyDefinition.Info.PropertyType);
    
            if (!valueDefinition.IsDataType)
                throw new Exception($"Mapping Exception: {valueDefinition.Type.FullName}");

            switch (valueDefinition.Type.FullName)
            {
                
                case "System.Byte[]":
                    if(propertyDefinition != null && propertyDefinition.IsEncrypted && this.cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = this.cryptoProvider.DecryptBlob(bytes);
                        return bytes;
                    }

                    return Convert.FromBase64String(value);
                case "System.Char[]":
                    if (propertyDefinition != null && propertyDefinition.IsEncrypted && this.cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = this.cryptoProvider.DecryptBlob(bytes);
                        return Encoding.UTF8.GetChars(bytes);
                    }
                    return value.ToCharArray();
                case "System.String":
                    if (propertyDefinition != null && propertyDefinition.IsEncrypted && this.cryptoProvider != null)
                    {
                        return this.cryptoProvider.DecryptString(value);
                    }

                    return value;
                case "System.Security.SecureString":
                    var ss = new System.Security.SecureString();
                    if(propertyDefinition != null && propertyDefinition.IsEncrypted && this.cryptoProvider != null)
                    {
                        var bytes = Convert.FromBase64String(value);
                        bytes = this.cryptoProvider.DecryptBlob(bytes);
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