using System;
using NerdyMishka.ComponentModel.DataAnnotations;
using NerdyMishka.Extensions.Flex;
using NerdyMishka.Reflection.Extensions;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace Tests
{
    public class YamlObjectWriterTests
    {
        [Fact]
        public void VisitValue_WithProperties()
        {
            var configuration = new SimpleConfigurationValues();
            var typeInfo = configuration.GetType().AsTypeInfo();
            var properties = typeInfo.Properties;

            var writer = new YamlObjectWriter();

            foreach(var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(configuration);
                var node = writer.VisitValue(value, propertyInfo, propertyInfo.ClrType.AsTypeInfo());

                if(value == null)
                {
                    Assert.Null(node);
                    continue;
                }

                Assert.IsType(typeof(YamlScalarNode), node);
                var scalar = (YamlScalarNode)node;

                switch(value)
                {
                    case byte[] bytes:
                        Assert.Equal(Convert.ToBase64String(bytes), scalar.Value);
                    break;
                    case char[] chars:
                        Assert.Equal(new string(chars), scalar.Value);
                    break;
                    default:
                        Assert.Equal(value.ToString(), scalar.Value);
                    break;
                }
            }
        }

        [Fact]
        public void VisitValue_WithAttributes()
        {
            var configuration = new SimpleConfigurationAttributes();
            var typeInfo = configuration.GetType().AsTypeInfo();
            var properties = typeInfo.Properties;

            var writer = new YamlObjectWriter();

            foreach(var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(configuration);
                var node = writer.VisitValue(value, propertyInfo, propertyInfo.ClrType.AsTypeInfo());

                if(value == null)
                {
                    Assert.Null(node);
                    continue;
                }

                Assert.IsType(typeof(YamlScalarNode), node);
                var scalar = (YamlScalarNode)node;

                switch(propertyInfo.Name)
                {
                    case "IsEnabled":
                        Assert.Equal("enabled", scalar.Value);
                    break;  
                    case "ConnectionString":
                        Assert.NotEqual(configuration.ConnectionString, scalar.Value);
                    break;
                    case "Password":  //HashAttribute does not convert
                        Assert.Equal(configuration.Password, scalar.Value);
                    break;
                }
            }
        }

        public class SimpleConfigurationAttributes
        {
            [Bit("enabled", "disabled")]
            public bool IsEnabled { get; set; } = true;

            [Encrypt]
            public string ConnectionString { get; set; } = "Server=localhost;";

            [Hash]
            public string Password { get; set; } = "my-great-and-terrible-pw";
        }

        public class SimpleConfigurationValues
        {
            public string String { get; set; } = "test-value";

            public int Int32 { get; set; } = 443;

            public int? NullableInt32 { get; set; } = null;

            public long Int64 { get; set; } = long.MaxValue;

            public long? NullableInt64 { get; set; } = long.MaxValue;

            public DateTime DateTime { get; set; } = DateTime.UtcNow;

            public Double Double { get; set; } = 80923.2342;

            public Decimal Decimal { get; set; } = 3421231234.23423M;

            public byte[] Bytes { get; set; } = System.Text.Encoding.UTF8.GetBytes("bytes");

            public char[] Chars { get; set; } = "chars".ToCharArray();

            public bool Boolean { get; set; } = true;
        }
    }
}
