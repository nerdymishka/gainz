using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using YamlDotNet.RepresentationModel;

namespace NerdyMishka.Flex.Yaml.Tests
{
    public class ObjectYamlVisitorTests
    {
        [Fact]
        public static void ValuesToYamlScalar()
        {
            var visitor = new ObjectYamlVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());

            // byte[]
            var node = (YamlScalarNode)visitor.Visit(sample.Bytes, classInfo.Properties["bytes"]);
            var bytes = Convert.ToBase64String(sample.Bytes);
            Assert.Equal(bytes, node.Value);

            // char[]
            node = (YamlScalarNode)visitor.Visit(sample.Chars, classInfo.Properties["chars"]);
            Assert.Equal(new String(sample.Chars), node.Value);

            // string
            node = (YamlScalarNode)visitor.Visit(sample.StringValue, classInfo.Properties["string"]);
            Assert.Equal(sample.StringValue.ToString(), node.Value);

            // short
            node = (YamlScalarNode)visitor.Visit(sample.Port, classInfo.Properties["port"]);
            Assert.Equal(sample.Port, short.Parse(node.Value));

            // short? null
            node = (YamlScalarNode)visitor.Visit(sample.NullPort, classInfo.Properties["nullPort"]);
            Assert.Null(node.Value);

            // short? with value
            node = (YamlScalarNode)visitor.Visit(
                sample.NullPortWithValue, 
                classInfo.Properties["nullPortWithValue"]);
            Assert.Equal(sample.NullPortWithValue, short.Parse(node.Value));

            // int
            node = (YamlScalarNode)visitor.Visit(sample.Age, classInfo.Properties["age"]);
            Assert.Equal(sample.Age, int.Parse(node.Value));

            // long
            node = (YamlScalarNode)visitor.Visit(sample.Id, classInfo.Properties["id"]);
            Assert.Equal(sample.Id, long.Parse(node.Value));

            // datetime utc should use ISO format that Json uses. 
            node = (YamlScalarNode)visitor.Visit(sample.CreatedAt, classInfo.Properties["createdAt"]);
            Assert.Equal(sample.CreatedAt, DateTime.Parse(node.Value));

            // datetime utc with format
            node = (YamlScalarNode)visitor.Visit(sample.CreatedAt, classInfo.Properties["publishedOn"]);
            Assert.Equal(sample.PublishedOn.ToString("yyyy-MM-dd"), node.Value);

            // bool 
            node = (YamlScalarNode)visitor.Visit(sample.IsEnabled, classInfo.Properties["enabled"]);
            Assert.Equal("yes", node.Value);

            // bool? null
            node = (YamlScalarNode)visitor.Visit(sample.IsEnabledNull, classInfo.Properties["enabledNull"]);
            Assert.Null(node.Value);
        }

        [Fact]
        public static void YamlScalarToValue()
        {
            var visitor = new ObjectYamlVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());


            var node = new YamlScalarNode();
            node.Value = Convert.ToBase64String(sample.Bytes);

            var data = visitor.Visit(node, classInfo.Properties["bytes"], null);
            Assert.IsType<byte[]>(data);
            Assert.Equal(sample.Bytes, (byte[])data);

            node.Value = new string(sample.Chars);
            data = visitor.Visit(node, classInfo.Properties["chars"], null);
            Assert.IsType<char[]>(data);
            Assert.Equal(sample.Chars, (char[])data);

            node.Value = sample.StringValue;
            data = visitor.Visit(node, classInfo.Properties["string"], null);
            Assert.IsType<string>(data);
            Assert.Equal(sample.StringValue, (string)data);


            // short
            node.Value = sample.Port.ToString();
            data = visitor.Visit(node, classInfo.Properties["port"], null);
            Assert.IsType<short>(data);
            Assert.Equal(sample.Port, (short)data);

            // short? null
            node.Value = "null";
            data = visitor.Visit(node, classInfo.Properties["nullPort"], null);
            Assert.Null(data);
            Assert.Equal(sample.NullPort, (short?)data);

            node.Value = sample.NullPortWithValue.ToString();
            data = visitor.Visit(node, classInfo.Properties["nullPortWithValue"], null);
            Assert.IsType<short>(data);
            Assert.Equal(sample.NullPortWithValue, (short?)data);

            // int
            node.Value = sample.Age.ToString();
            data = visitor.Visit(node, classInfo.Properties["age"], null);
            Assert.IsType<int>(data);
            Assert.Equal(sample.Age, (int)data);

            // custom date
            node.Value = sample.PublishedOn.ToString("yyyy-MM-dd");
            data = visitor.Visit(node, classInfo.Properties["publishedOn"], null);
            Assert.IsType<DateTime>(data);
            Assert.Equal(sample.PublishedOn, (DateTime)data);

            // default date
            node = (YamlScalarNode)visitor.Visit(sample.CreatedAt, classInfo.Properties["createdAt"]);
            data = visitor.Visit(node, classInfo.Properties["createdAt"], null);
            Assert.IsType<DateTime>(data);
            Assert.Equal(sample.CreatedAt, (DateTime)data);

            // bool
            node.Value = "yes";
            data = visitor.Visit(node, classInfo.Properties["enabled"], null);
            Assert.IsType<bool>(data);
            Assert.Equal(sample.IsEnabled, (bool)data);

            node.Value = "";
            data = visitor.Visit(node, classInfo.Properties["enabledNull"], null);
            Assert.Null(data);
            Assert.Equal(sample.IsEnabledNull, (bool?)data);
        }

        [Fact]
        public static void SimpleToMappingNode()
        {
            var visitor = new ObjectYamlVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());

            var map = (YamlMappingNode)visitor.Visit(sample, classInfo, new YamlMappingNode());

            Assert.NotNull(map);

            Assert.Equal(sample.Age.ToString(), ((YamlScalarNode)map["age"]).Value);
        }


        public class ValueSample
        {
            public ValueSample()
            {
                this.Bytes = Encoding.UTF8.GetBytes("IhaZBytes");
                this.Chars = "IhaZChars".ToCharArray();
                this.StringValue = "IhaZString";
                this.IsEnabled = true;
                this.Age = 10;
                this.Id = 20;
                this.Port = 80;
                this.NullPortWithValue = 443;
                this.CreatedAt = DateTime.Now;
                this.PublishedOn = DateTime.Now.ToShortDate();
            }

            [Symbol("bytes")]
            public byte[] Bytes { get; set; }

            [Symbol("chars")]
            public char[] Chars { get; set; }

            [Symbol("string")]
            public string StringValue { get; set; }

            [Symbol("enabled")]
            [Switch(No = "no", Yes = "yes")]
            public bool IsEnabled { get; set; }

            [Symbol("enabledNull")]
            [Switch(No = "no", Yes = "yes")]
            public bool? IsEnabledNull { get; set; }

            [Symbol("age")]
            public int Age { get; set; }

            [Symbol("nullAge")]
            public int? NullAge { get; set; }

            [Symbol("id")]
            public long Id { get; set; }

            [Symbol("nullId")]
            public long? NullId { get; set; }

            [Symbol("port")]
            public short Port { get; set; }

            [Symbol("nullPort")]
            public short? NullPort { get; set; }

            [Symbol("nullPortWithValue")]
            public short? NullPortWithValue { get; set; }

            [Symbol("createdAt")]
            public DateTime CreatedAt { get; set; }

            [Symbol("publishedOn")]
            [DateTimeFormat("yyyy-MM-dd")]
            public DateTime PublishedOn { get; set; }
        }
    }
}
