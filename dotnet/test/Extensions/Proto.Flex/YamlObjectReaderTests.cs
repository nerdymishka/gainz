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