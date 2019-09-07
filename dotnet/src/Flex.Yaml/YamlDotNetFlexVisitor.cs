using System.Collections;
using NerdyMishka.Flex.Reflection;
using YamlDotNet.RepresentationModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NerdyMishka.Flex
{
    public class YamlDotNetFlexVisitor : FlexVisitor<YamlNode, YamlScalarNode, YamlMappingNode, YamlSequenceNode, YamlDocument>
    {

        public YamlDotNetFlexVisitor(FlexSettings options = null)
        {
            this.FlexSettings = options ?? new FlexSettings();
        }

        public override string Name => "YamlDotNet";

        public override YamlSequenceNode VisitArray(IList value, FlexTypeDefinition definition)
        {
            if (value == null)
                return null;

            var def = definition;       
            var valueTypeInfo = TypeInspector.GetTypeInfo(def.ValueType);
            var node = new YamlSequenceNode();

            foreach(var item in value)
            {
                var nextNode = this.Visit(item, valueTypeInfo);
                if (nextNode != null)
                    node.Add(nextNode);
            }

            return node;
        }

        public override IList VisitArray(YamlSequenceNode value, FlexTypeDefinition definition)
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

            for (int i = 0; i < node.Children.Count; i++)
            {
                var nextNode = node.Children[i];
                var nextClassInfo = TypeInspector.GetTypeInfo(def.ValueType);
               
            
                var obj = this.Visit(nextNode, nextClassInfo, null);
                list.Add(obj);
            }

            return list;
        }

        public override YamlDocument VisitDocument(object value)
        {
            var node = this.Visit(value);
            var doc = new YamlDocument(node);
            return doc;
        }

        public override object VisitObject(YamlDocument value, Type type)
        {
            var typeDef = TypeInspector.GetTypeInfo(type);
            return this.Visit(value.RootNode, typeDef, null);
        }

        public override YamlMappingNode VisitElement(IDictionary value, FlexTypeDefinition definition)
        {  
            if (value == null)
                return null;

            var node = new YamlMappingNode();
            foreach(var key in value.Keys)
            {
                var clrValue = value[key];
                var child = this.Visit(clrValue);
                if (child != null)
                    node.Add(key.ToString(), child);
            }

            return node;
        }

        public override YamlNode Visit(object value)
        {
            if (value == null)
                return null;

            var def = TypeInspector.GetTypeInfo(value.GetType());
            return this.Visit(value, def);
        }

        public override object Visit(YamlNode node, FlexTypeDefinition typeDef, FlexPropertyDefinition propertyDef)
        {
            
            switch(node)
            {
                case YamlMappingNode map:
                    return this.VisitElement(map, typeDef);
                case YamlSequenceNode seq:
                    return this.VisitArray(seq, typeDef);
                case YamlScalarNode scalar:
                    return this.VisitProperty(scalar, propertyDef, typeDef);
                default:
                    throw new NotSupportedException($"{node.GetType().FullName}");
            }
        }

        public override YamlNode Visit(object value, FlexTypeDefinition def)
        {
            if (value == null)
                return null;

            
            if(def.IsDataType)
            {
                var scalar = new YamlScalarNode();
                var v = this.VisitValue(value, null);
                scalar.Value = v;
                if(v == null)
                {
                    if(this.FlexSettings.OmitNulls)
                        return null;

                    scalar.Value = "null";
                }

                return scalar;
            }
           

            if (def.IsDictionary)
                return this.VisitElement((IDictionary)value, def);

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

           
            return this.VisitElement(value, def);
        }




        public override YamlMappingNode VisitElement(object value, FlexTypeDefinition definition)
        {
            if (value == null)
                return null;

           
            var def = definition;
            var node = new YamlMappingNode();
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
                        if (scalar.Value == null && this.FlexSettings.OmitNulls)
                            continue;

                        node.Add(nextPropSet.Key, scalar);
                    }
                    continue;
                }

                var valueNode = this.Visit(propValue, propertyClassInfo);
                if(valueNode != null)
                {
                    node.Add(nextPropSet.Key, valueNode);
                }
            }

            return node;
        }

        public override object VisitElement(YamlMappingNode node, FlexTypeDefinition definition)
        {
            var def = definition;
            if (def.IsDataType || def.IsList)
            {
                throw new Exception("Invalid Mapping Exception");
            }

            

            IDictionary dictionary = null;
            object instance = Activator.CreateInstance(def.Type);
            if (def.IsDictionary)
                dictionary = (IDictionary)instance;

            foreach(YamlScalarNode child in node.Children.Keys)
            {
                var name = child.Value;
                var nextNode = node.Children[name];
                
               
                if (!def.Properties.TryGetValue(name, out FlexPropertyDefinition propertyTypeInfo))
                    continue;

                if (propertyTypeInfo.IsIgnored)
                    continue;

                var childInfo = TypeInspector.GetTypeInfo(propertyTypeInfo.Info.PropertyType);
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

     

        public override YamlScalarNode VisitProperty(object value, FlexPropertyDefinition definition)
        {
            var v = this.VisitValue(value, definition);
            if(v == null && this.FlexSettings.OmitNulls)
                return null;

        

            return new YamlScalarNode() {
                Value = v ?? "null"
            };
        }

        public override object VisitProperty(YamlScalarNode value, FlexPropertyDefinition definition, FlexTypeDefinition valueDefinition)
        {
           var v = this.VisitValue(value.Value, definition, valueDefinition);
           if(v is string && v == "null")
                return null;

            return v;
        }
    }
}