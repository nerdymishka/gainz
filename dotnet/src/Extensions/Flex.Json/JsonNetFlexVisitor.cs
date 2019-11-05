using System.Collections;
using NerdyMishka.Flex.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NerdyMishka.Flex
{
    public class JsonNetFlexVisitor : FlexVisitor<JToken, JValue, JObject, JArray, JContainer>
    {

        public JsonNetFlexVisitor(FlexSettings options = null)
        {
            this.FlexSettings = options ?? new FlexSettings();
        }

        public override string Name => "JsonDotNet";

        protected override void AddChild(JObject documentElement, string name, object value)
            => documentElement.Add(name, (JToken)value);
        

        protected override void AddChild(JArray documentArray, JToken value)
            => documentArray.Add(value);

        protected override JContainer CreateDocument(JToken root)
        {
            switch(root)
            {
                case JArray array:
                    return array;
                case JObject obj:
                    return obj;

                default:
                    return new JObject(root);
            }
        }

        protected override JValue CreateValue(string value)
        {
            if(value == null)
                return JValue.CreateNull();

            return new JValue(value);
        }

        protected override IEnumerator<JToken> GetEnumerator(JArray array)
            => array.GetEnumerator();

        protected override JToken GetRootNode(JContainer document)
            => document.Root;

        protected override string GetValue(JValue documentValue)
            => documentValue.Value.ToString();

        protected override bool IsNull(JValue documentValue)
            => documentValue.Value == null;

        protected override void SetValue(JValue documentValue, object value)
            => documentValue.Value = value;

        /* 
                public override JArray VisitArray(IList value, FlexTypeDefinition definition)
                {
                    if (value == null)
                        return null;

                    var def = definition;       
                    var valueTypeInfo = TypeInspector.GetTypeInfo(def.ValueType);
                    var node = new JArray();

                    foreach(var item in value)
                    {
                        var nextNode = this.Visit(item, valueTypeInfo);
                        if (nextNode != null)
                            node.Add(node);
                    }

                    return node;
                }

                public override IList VisitArray(JArray value, FlexTypeDefinition definition)
                {
                    var node = value;
                    var def = definition;
                    if (!def.IsList)
                        throw new Exception("Mapping Mismatch");

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

                    var children= node.Children();
                    for (int i = 0; i < children.Count(); i++)
                    {
                        var nextNode = (JToken)children[i];
                        var nextClassInfo = TypeInspector.GetTypeInfo(def.ValueType);
                        var obj = this.Visit(nextNode, nextClassInfo, null);
                        list.Add(obj);
                    }

                    return list;
                }

                public override JContainer VisitDocument(object value)
                {
                    return (JContainer)this.Visit(value);
                }

                public override object VisitObject(JContainer value, Type type)
                {
                    var typeDef = TypeInspector.GetTypeInfo(type);
                    return this.Visit(value, typeDef, null);
                }

                public override JObject VisitElement(IDictionary value, FlexTypeDefinition definition)
                {  
                    if (value == null)
                        return null;

                    var node = new JObject();
                    foreach(var key in value.Keys)
                    {
                        var clrValue = value[key];
                        var child = this.Visit(clrValue);
                        if (child != null)
                            node.Add(key.ToString(), child);
                    }

                    return node;
                }

                public override JToken Visit(object value)
                {
                    if (value == null)
                        return null;

                    var def = TypeInspector.GetTypeInfo(value.GetType());
                    return this.Visit(value, def);
                }

                public override object Visit(JToken node, FlexTypeDefinition typeDef, FlexPropertyDefinition propertyDef)
                {

                    switch(node)
                    {
                        case JObject map:
                            return this.VisitElement(map, typeDef);
                        case JArray seq:
                            return this.VisitArray(seq, typeDef);
                        case JValue scalar:
                            return this.VisitProperty(scalar, propertyDef, typeDef);
                        default:
                            throw new NotSupportedException($"{node.GetType().FullName}");
                    }
                }

                public override JToken Visit(object value, FlexTypeDefinition def)
                {
                    if (value == null)
                        return null;


                    if(def.IsDataType)
                    {

                        var v = this.VisitValue(value, null);
                        if(v == null && this.FlexSettings.OmitNulls)
                            return null;

                        return new JValue(v);
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
                            var nextNode = new JValue(false);

                            return nextNode;
                        }
                    }

                    set = properties.FirstOrDefault(o => o.Value.IsDefault);

                    if(set.Value != null)
                    {
                        var defaultValue = set.Value.Info.GetValue(value);
                        if(defaultValue != null)
                        {
                            var nextNode = new JValue(defaultValue.ToString());
                            return nextNode;
                        }
                    }


                    return this.VisitElement(value, def);
                }





                public override JObject VisitElement(object value, FlexTypeDefinition definition)
                {
                    if (value == null)
                        return null;


                    var def = definition;
                    var node = new JObject();
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

                public override object VisitElement(JObject node, FlexTypeDefinition definition)
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

                    foreach(var pair in node)
                    {
                        var name = pair.Key;
                        var nextNode = pair.Value;


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



                public override JValue VisitProperty(object value, FlexPropertyDefinition definition)
                {
                    var v = this.VisitValue(value, definition);
                    if(v == null && this.FlexSettings.OmitNulls)
                        return null;

                    if(v == null || v == "null")
                        return JValue.CreateNull();



                    return new JValue(v) ;
                }

                public override object VisitProperty(JValue value, FlexPropertyDefinition definition, FlexTypeDefinition valueDefinition)
                {
                    if(value.Type == JTokenType.Null )
                        return null;

                    return this.VisitValue(value.ToString(), definition, valueDefinition);
                }
                */
    }
}