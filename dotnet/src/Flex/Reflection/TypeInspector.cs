using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NerdyMishka.Flex.Reflection
{
    public class TypeInspector
    {
        private static readonly object s_object = new object();
        private static Dictionary<Type, FlexTypeDefinition> s_classTypeInfo
            = new Dictionary<Type, FlexTypeDefinition>();


        public static FlexTypeDefinition GetTypeInfo(Type type)
        {
            lock (s_object)
            {

                if (s_classTypeInfo.TryGetValue(type, out FlexTypeDefinition info))
                    return info;



                info = new FlexTypeDefinition()
                {
                    Type = type
                };
                s_classTypeInfo.Add(type, info);

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    info.IsNullable = true;
                    info.ValueType = type.GetGenericArguments()[0];
                    info.IsDataType = true;
                    return info;
                }
                var arrayTypes = new[] { typeof(char[]), typeof(byte[]) };
                var dataTypes = new[] { typeof(string), typeof(char[]), typeof(byte[]) };

                if (type.IsValueType || dataTypes.Contains(type))
                {
                    if (arrayTypes.Contains(type))
                        info.IsArray = true;

                    info.IsDataType = true;
                    return info;
                }

                var interfaces = type.GetInterfaces();
                foreach (var contract in interfaces)
                {
                    if (contract.IsGenericTypeDefinition)
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

                    if (contract is IDictionary)
                    {
                        info.KeyType = typeof(object);
                        info.ValueType = typeof(object);
                        info.IsDictionary = true;
                        break;
                    }

                    if (contract is IList)
                    {
                        info.ValueType = typeof(object);
                        info.IsList = true;
                        break;
                    }
                }

                if (info.IsList || info.IsDictionary)
                {
                    return info;
                }



                var propertyInfos = new List<FlexPropertyDefinition>();
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var propertyTypeInfo = new FlexPropertyDefinition()
                    {
                        Info = property
                    };

                    var dateTimes = new List<DateTimeFormatAttribute>();
                    var attributes = property.GetCustomAttributes();
                    foreach (var attribute in attributes)
                    {
                        switch (attribute)
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
        }
    }
}
