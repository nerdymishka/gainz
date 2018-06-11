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
        public static void ObjectToString()
        {
            var visitor = new ObjectYamlVisitor();
            var sample = new Sample();
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
            Assert.Equal(sample.NullPort, (short?)null);

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
        }



        public class Sample
        {
            public Sample()
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
                this.PublishedOn = DateTime.Now;
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
