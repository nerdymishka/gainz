using System;
using System.Collections;
using YamlDotNet.RepresentationModel;
using NerdyMishka.Reflection;
using NerdyMishka.Reflection.Extensions;

namespace NerdyMishka.Extensions.Flex 
{
    public class YamlObjectReader
    {

        public object Visit(YamlNode node, IType typeInfo)
        {
            switch(node)
            {
                case YamlMappingNode element:
                    return this.VisitElement(element, typeInfo);
                case YamlSequenceNode array:
                    return this.VisitArray(array, typeInfo);
                case YamlScalarNode value:
                    return this.VisitValue(value, typeInfo);
                
            }

            return null;
        }

        public object VisitDocument(YamlDocument document, IType typeInfo)
        {
            var rootNode = document.RootNode;
            return this.Visit(rootNode, typeInfo);
        }


        public string VisitComment(YamlNode node)
        {
            return null;
        }


        public object VisitAttribute(YamlNode node, IType typeInfo)
        {
             return null;
        }


        public object VisitValue(YamlScalarNode node, IType typeInfo)
        {
             return null;
        }

        public object VisitElement(YamlMappingNode node, IType typeInfo)
        {
            var isDictionaryLike = typeInfo.IsIDictionaryOfKv() || typeInfo.IsIDictionary();
             return null;
        }


        public IList VisitArray(YamlSequenceNode node, IType typeInfo)
        {
            if(typeInfo.IsArray())
            {
                var childType = typeInfo.AsItemType()?.ItemType;
                var clrType = childType?.ClrType;
                if(childType == null)
                {
                    clrType = typeof(Object);
                    childType = ReflectionCache.GetOrAdd(clrType);
                }

                var array = (Array)Array.CreateInstance(clrType, node.Children.Count);


                for(var i = 0; i < node.Children.Count; i++) 
                {
                    var child = node.Children[i];
                    var value = this.Visit(child, childType);
                    array.SetValue(value, i);
                }

                return array;

            } else {
                var listLike = typeInfo.IsICollectionOfT() || typeInfo.IsIListOfT() || typeInfo.IsICollection() || typeInfo.IsIList();
                if(!listLike) 
                    throw new Exception($"{typeInfo.FullName} is not array or list like.");

                var childType = typeInfo.AsItemType()?.ItemType;
                var clrType = childType?.ClrType;
                if(childType == null)
                {
                    clrType = typeof(Object);
                    childType = ReflectionCache.GetOrAdd(clrType);
                }
                
                var list = (IList)Activator.CreateInstance(typeInfo.ClrType);

                for(var i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    var value = this.Visit(child, childType);
                    list.Add(value);
                }

                return list;
            }
        }
    }
}