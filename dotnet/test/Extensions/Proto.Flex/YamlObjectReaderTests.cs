using System;
using System.Collections.Generic;
using NerdyMishka.ComponentModel.DataAnnotations;
using NerdyMishka.Extensions.Flex;
using NerdyMishka.Reflection.Extensions;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace Tests
{
    public class YamlObjectReaderTests
    {

        [Fact]
        public static void VisitElement()
        {
            var reader = new YamlObjectReader();
            var map = new YamlMappingNode();
            var config1 = new SimpleConfigurationValues();
            var properties = typeof(SimpleConfigurationValues).AsTypeInfo().Properties;

            foreach(var propInfo in properties)
            {
                var value = propInfo.GetValue(config1);
                string strValue = null;

                if(value == null)
                    strValue = "null";

                if(value != null)
                {
                    switch(value)
                    {
                        case byte[] bytes:
                            strValue = Convert.ToBase64String(bytes);
                        break;
                        case char[] chars:
                            strValue = new string(chars);
                        break;
                        case DateTime dt:
                            if(dt.Kind != DateTimeKind.Utc)
                                throw new Exception("bad date");
                            strValue = dt.ToString("o");
                        break;
                        default:
                           strValue = value.ToString();
                        break;
                    }
                }
                var scalar = new YamlScalarNode(strValue);

                map.Add(
                    propInfo.Name[0].ToString().ToLowerInvariant() + propInfo.Name.Substring(1),
                    scalar);
                
            }

            var config2 = reader.VisitElement(map, null, config1.GetType().AsTypeInfo());
            Assert.NotNull(config2);
            Assert.IsType<SimpleConfigurationValues>(config2);

            foreach(var propInfo in properties)
            {
                var expected = propInfo.GetValue(config1);
                var actual = propInfo.GetValue(config2);

                Assert.Equal(expected, actual);
            }
        }


        [Fact]
        public static void VisitDictionary()
        {
            var reader = new YamlObjectReader();
            var map = new YamlMappingNode();
            map.Add("one", "1");
            map.Add("two", "2");
            map.Add("three", "3");


            var @object = reader.VisitElement(map, null, typeof(Dictionary<string, object>).AsTypeInfo());
            Assert.NotNull(@object);
            Assert.IsType<Dictionary<string, object>>(@object);
            var dictionary = (Dictionary<string, object>)@object;

            Assert.Equal("1", dictionary["one"]);
            Assert.Equal("2", dictionary["two"]);
            Assert.Equal("3", dictionary["three"]);
        }


        [Fact]
        public static void VisitList()
        {
            var reader = new YamlObjectReader();
            var sequence = new YamlSequenceNode() {
                "1",
                "2",
                "3",
                "4",
                "5",
                "6"
            };
            
            var list =  reader.VisitArray(sequence, null, typeof(List<string>).AsTypeInfo());
            Assert.NotNull(list);
            var set = (IList<string>)list;
            Assert.Collection(set, 
                (v) => Assert.Equal("1", ((YamlScalarNode)v).Value),
                (v) => Assert.Equal("2", ((YamlScalarNode)v).Value),
                (v) => Assert.Equal("3", ((YamlScalarNode)v).Value),
                (v) => Assert.Equal("4", ((YamlScalarNode)v).Value),
                (v) => Assert.Equal("5", ((YamlScalarNode)v).Value),
                (v) => Assert.Equal("6", ((YamlScalarNode)v).Value)
            );
        }


        [Fact]
        public void Visit()
        {
            var reader = new YamlObjectReader();
            var configuration = new SimpleConfigurationValues();
            var properties = configuration.GetType().AsTypeInfo().Properties;

            foreach(var propInfo in properties)
            {
                var value = propInfo.GetValue(configuration);
                string strValue = null;

                if(value == null)
                    strValue = "null";

                if(value != null)
                {
                    switch(value)
                    {
                        case byte[] bytes:
                            strValue = Convert.ToBase64String(bytes);
                        break;
                        case char[] chars:
                            strValue = new string(chars);
                        break;
                        case DateTime dt:
                            if(dt.Kind != DateTimeKind.Utc)
                                throw new Exception("bad date");
                            strValue = dt.ToString("o");
                        break;
                        default:
                           strValue = value.ToString();
                        break;
                    }
                }
                var scalar = new YamlScalarNode(strValue);
                var convertedValue = reader.VisitValue(scalar, propInfo, propInfo.ClrType.AsTypeInfo());

                Assert.Equal(value, convertedValue);
            }
        }
    }
}