

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class FlexTypeInfo
    {
        private bool isList;

        public static readonly IList<Type> s_dataTypes;
            
        private static readonly IList<Type> s_arrayTypes;

        private static readonly List<Func<Type, FlexTypeInfo, bool>> s_typeMap;

        private static readonly List<Func<Type, FlexTypeInfo, bool>> s_interfaceMap;

        static FlexTypeInfo()
        {
            s_dataTypes = new []{ typeof(string), typeof(DateTime), typeof(TimeSpan), typeof(char[]), typeof(byte[]) };
            s_arrayTypes = new []{typeof(char[]), typeof(byte[])};

            s_typeMap = new List<Func<Type, FlexTypeInfo, bool>>();
            var nullableType = typeof(Nullable<>);

            s_typeMap.Add((t, ft) =>{
                
                if(t.IsGenericType)
                {
                    var typeDef = t.GetGenericTypeDefinition();
                    if(typeDef == nullableType)
                    {
                        var underlyingType = t.GetGenericArguments()[0];
                        if(underlyingType.IsPrimitive)
                        {
                            ft.IsNullable = true;
                            ft.valueType = underlyingType;
                            ft.IsDataType = true;
                            return true;
                        }
                    }
                }

                if(t.IsPrimitive || s_dataTypes.Contains(t))
                {
                    ft.IsDataType = true;
                    return true;
                }

                var interfaces = t.GetInterfaces();

                foreach(var contract in interfaces)
                {
                    if(contract.IsGenericType)
                    {
                        var typeDef = contract.GetGenericTypeDefinition();
                        foreach(var command in s_interfaceMap)
                        {
                            if(command(typeDef, ft))
                            {
                                return true;
                            }
                        }
                    }

                    foreach(var command in s_interfaceMap)
                    {
                        if(command(t, ft))
                            return true;
                    }
                }


               

                return false;
            });

            s_typeMap.Add((t, ft) => {

                var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach(var prop in properties)
                {
                    ft.properties.Add(prop.Name, new FlexPropertyInfo(prop));
                }       
                return true;
            });

           

            s_interfaceMap = new List<Func<Type, FlexTypeInfo, bool>>();

            var genericDictionaryType = typeof(IDictionary<,>);
            var genericCollectionType = typeof(ICollection<>);
            var genericListType = typeof(IList<>);
            var dictionaryType = typeof(IDictionary);
            var collectionType = typeof(ICollection);
            var listType = typeof(IList);

           
           
            s_interfaceMap.Add((c, ft) => {
                if(c == genericDictionaryType)
                {
                    var types = c.GetGenericArguments();
                    ft.keyType = types[0];
                    ft.valueType = types[1];
                    ft.IsDictionary = true;
                    return true;
                }
                return false;
            });


            s_interfaceMap.Add((c, ft) => {
                if(c == genericCollectionType)
                {
                    var types = c.GetGenericArguments();
                    ft.valueType = types[0];
                    ft.IsCollection = true;
                    return true;
                }
                return false;
            });

            s_interfaceMap.Add((c, ft) => {
                if(c == genericListType)
                {
                    var types = c.GetGenericArguments();
                    ft.valueType = types[0];
                    ft.IsList = true;
                    return true;
                }
                return false;
            });

            s_interfaceMap.Add((c, ft) => {
                if(c == dictionaryType)
                {
                    ft.keyType = typeof(object);
                    ft.valueType = typeof(object);
                    ft.IsDictionary = true;
                    return true;
                }

                return false;
            });


            s_interfaceMap.Add((c, ft) => {
                if(c == collectionType)
                {
                    ft.valueType = typeof(object);
                    ft.IsCollection = true;
                    return true;
                }
                return false;
            });


            s_interfaceMap.Add((c, ft) => {
                if(c == listType)
                {
                    ft.valueType = typeof(object);
                    ft.IsList = true;
                    return true;
                }
                return false;
            });

        }

        public FlexTypeInfo(Type type)
        {
            this.type = type;
        }

        private Type type;

        private Type valueType;

        private Type keyType;

        private IDictionary<string, FlexPropertyInfo> properties = new Dictionary<string, FlexPropertyInfo>();

        public string Name => this.type.Name;

        public string FullName => this.type.FullName;

        public virtual bool IsArray => this.type.IsArray;

        public virtual bool IsDictionary { get; internal protected set; }

        public virtual bool IsList { get; internal protected set; }

        public virtual bool IsCollection { get; internal protected set; }

        public virtual bool IsNullable { get; internal protected set; }

        public virtual bool IsDataType { get; internal protected set; }

        public virtual IDictionary<string, FlexPropertyInfo> Properties => this.properties;

        public virtual void Inspect()
        {
            var typeDef = this.type;
            foreach(var cmd in s_typeMap)
            {
                if(cmd(typeDef, this))
                    return;
            }
        }

     

    }
}