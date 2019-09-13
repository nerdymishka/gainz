using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Newtonsoft.Json.Linq;
using NerdyMishka.Security;
using NerdyMishka.Security.Cryptography;
using System.IO;
using NerdyMishka.Flex.Reflection;
using NerdyMishka.Flex.Json;

namespace NerdyMishka.Flex.Tests 
{
    public class JsonNetFlexVisitorTests
    {
        [Fact]
        public static void ValuesToYamlScalar()
        {
            var visitor = new JsonNetFlexVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());

            // byte[]
            var node = visitor.VisitProperty(sample.Bytes, classInfo.Properties["bytes"]);
            var bytes = Convert.ToBase64String(sample.Bytes);
            Assert.Equal(bytes, node.Value);

            // char[]
            node = visitor.VisitProperty(sample.Chars, classInfo.Properties["chars"]);
            Assert.Equal(new String(sample.Chars), node.Value);

            // string
            node = visitor.VisitProperty(sample.StringValue, classInfo.Properties["string"]);
            Assert.Equal(sample.StringValue.ToString(), node.Value);

            // short
            node = visitor.VisitProperty(sample.Port, classInfo.Properties["port"]);
            Assert.Equal(sample.Port, node.Value<short>());

            // short? null
            node = visitor.VisitProperty(sample.NullPort, classInfo.Properties["nullPort"]);
            Assert.Null(node.Value<short?>());

            // short? with value
            node = visitor.VisitProperty(
                sample.NullPortWithValue, 
                classInfo.Properties["nullPortWithValue"]);
            Assert.Equal(sample.NullPortWithValue, node.Value<short>());

            // int
            node = visitor.VisitProperty(sample.Age, classInfo.Properties["age"]);
            Assert.Equal(sample.Age, node.Value<int>());

            // long
            node = visitor.VisitProperty(sample.Id, classInfo.Properties["id"]);
            Assert.Equal(sample.Id, node.Value<long>());

            // datetime utc should use ISO format that Json uses. 
            node = visitor.VisitProperty(sample.CreatedAt, classInfo.Properties["createdAt"]);
            Assert.Equal(sample.CreatedAt, node.Value<DateTime>());

            // datetime utc with format
            node = visitor.VisitProperty(sample.CreatedAt, classInfo.Properties["publishedOn"]);
            Assert.Equal(sample.PublishedOn.ToString("yyyy-MM-dd"), node.Value<string>());

            // bool 
            node = visitor.VisitProperty(sample.IsEnabled, classInfo.Properties["enabled"]);
            Assert.Equal("yes", node.Value<string>());

            // bool? null
            node = visitor.VisitProperty(sample.IsEnabledNull, classInfo.Properties["enabledNull"]);
            Assert.Null(node.Value<bool?>());
        }

        [Fact]
        public static void YamlScalarToValue()
        {
            var visitor = new JsonNetFlexVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());


            var node = new JValue(Convert.ToBase64String(sample.Bytes));
           

            var data = visitor.VisitProperty(node, classInfo.Properties["bytes"]);
           
            //Assert.Equal(sample.Bytes, ;

            node.Value = new string(sample.Chars);
            data = visitor.VisitProperty(node, classInfo.Properties["chars"]);
       
            //Assert.Equal(sample.Chars, data.Value<char[]>());

            node.Value = sample.StringValue;
            data = visitor.VisitProperty(node, classInfo.Properties["string"]);
            Assert.IsType<string>(data);
            Assert.Equal(sample.StringValue, (string)data);


            // short
            node.Value = sample.Port.ToString();
            data = visitor.VisitProperty(node, classInfo.Properties["port"]);
            Assert.IsType<short>(data);
            Assert.Equal(sample.Port, (short)data);

            // short? null
            node.Value = "null";
            data = visitor.VisitProperty(node, classInfo.Properties["nullPort"]);
            Assert.Null(data);
            Assert.Equal(sample.NullPort, (short?)data);

            node.Value = sample.NullPortWithValue.ToString();
            data = visitor.VisitProperty(node, classInfo.Properties["nullPortWithValue"]);
            Assert.IsType<short>(data);
            Assert.Equal(sample.NullPortWithValue, (short?)data);

            // int
            node.Value = sample.Age.ToString();
            data = visitor.VisitProperty(node, classInfo.Properties["age"]);
            Assert.IsType<int>(data);
            Assert.Equal(sample.Age, (int)data);

            // custom date
            node.Value = sample.PublishedOn.ToString("yyyy-MM-dd");
            data = visitor.VisitProperty(node, classInfo.Properties["publishedOn"]);
            Assert.IsType<DateTime>(data);
            Assert.Equal(sample.PublishedOn, (DateTime)data);

            // default date
            node = visitor.VisitProperty(sample.CreatedAt, classInfo.Properties["createdAt"]);
            data = visitor.VisitProperty(node, classInfo.Properties["createdAt"]);
            Assert.IsType<DateTime>(data);
            Assert.Equal(sample.CreatedAt, (DateTime)data);

            // bool
            node.Value = "yes";
            data = visitor.VisitProperty(node, classInfo.Properties["enabled"]);
            Assert.IsType<bool>(data);
            Assert.Equal(sample.IsEnabled, (bool)data);

            node.Value = "";
            data = visitor.VisitProperty(node, classInfo.Properties["enabledNull"]);
            Assert.Null(data);
            Assert.Equal(sample.IsEnabledNull, (bool?)data);
        }

        [Fact]
        public static void SimpleToMappingNode()
        {
            var visitor = new JsonNetFlexVisitor();
            var sample = new ValueSample();
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());

            var map = visitor.VisitComplexObject(sample, classInfo);

            Assert.NotNull(map);

            Assert.Equal(sample.Age.ToString(), (map["age"]).Value<string>());
        }

        [Fact]
        public static void SampleEncryptNode()
        {

            var visitor = new JsonNetFlexVisitor(new FlexSettings() {
                CryptoProvider = new FlexCryptoProvider()
            });
            var sample =  new EncryptedSample() { MyConnectionString = "Hello, World" };
            var classInfo = TypeInspector.GetTypeInfo(sample.GetType());

            var map = visitor.VisitComplexObject(sample, classInfo);
            Assert.NotNull(map);
            var encrypted = (map["myConnectionString"]).Value<string>();
            Assert.NotEqual(sample.MyConnectionString, encrypted);

            var sample2 = (EncryptedSample)visitor.Visit(map, typeof(EncryptedSample));
            Assert.NotEqual(sample2.MyConnectionString, encrypted);
            Assert.NotNull(sample2.MyConnectionString);
            Assert.Equal(sample2.MyConnectionString, sample.MyConnectionString);
        }

        [Fact]
        public static void ComplexTest()
        {
              var visitor = new JsonNetFlexVisitor(new FlexSettings() {
                CryptoProvider = new FlexCryptoProvider()
            });
            var encrypted =  new EncryptedSample() { MyConnectionString = "Hello, World" };
            var complex = new ComplexSample() {
                Crypto = encrypted,
                Values = new ValueSample() 
            };

            var classInfo = TypeInspector.GetTypeInfo(complex.GetType());

            var doc = visitor.VisitComplexObject(complex);
            Assert.NotNull(doc);

            Assert.NotNull(doc["values"]);
            Assert.NotNull(doc["values"]["port"]);
            Assert.Equal("80", (doc["values"]["port"]));

            var complex2 = visitor.VisitDocument<ComplexSample>(doc);

            Assert.NotNull(complex2);
            Assert.NotNull(complex2.Values);
            Assert.NotNull(complex2.Crypto);
            Assert.Equal(80, complex2.Values.Port);
            Assert.NotNull(doc["crypto"]["myConnectionString"]);
            Assert.NotEqual(complex2.Crypto.MyConnectionString, 
                (doc["crypto"]["myConnectionString"]).Value<string>());
            Assert.Equal(complex.Crypto.MyConnectionString, complex2.Crypto.MyConnectionString);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public static void FileTests()
        {
            var dir = Env.ResolvePath("~/Data");
            if(Directory.Exists(dir))
                Directory.Delete(dir, true);
                
            Directory.CreateDirectory(dir);

            var encrypted =  new EncryptedSample() { MyConnectionString = "Hello, World" };
            var complex = new ComplexSample() {
                Crypto = encrypted,
                Values = new ValueSample() 
            };

            var builder = new FlexBuilder()
                .SetCryptoProvider(new FlexCryptoProvider());

            var file = Env.ResolvePath("~/Data/complex.yml");
            
            builder.ToJsonFile<ComplexSample>(file, complex);
            Assert.True(File.Exists(file));

            var complex2 = builder.FromJsonFile<ComplexSample>(file);


            Assert.NotNull(complex2);
            Assert.NotNull(complex2.Values);
            Assert.NotNull(complex2.Crypto);
            Assert.Equal(80, complex2.Values.Port);
            Assert.Equal(complex.Crypto.MyConnectionString, complex2.Crypto.MyConnectionString);
        }


        public class FlexCryptoProvider : IFlexCryptoProvider
        {
            private static byte[] pass = System.Text.Encoding.UTF8.GetBytes("my-great-and-terrible-password");

            public byte[] DecryptBlob(byte[] blob, byte[] privateKey = null)
            {
               return DataProtection.DecryptBlob(blob, privateKey ?? pass);
              
            }

            public string DecryptString(string value, byte[] privateKey = null)
            {
                return DataProtection.DecryptString(value, privateKey ?? pass);
            }

            public byte[] EncryptBlob(byte[] blob, byte[] privateKey = null)
            {
                return DataProtection.EncryptBlob(blob, privateKey ?? pass);
            }

            public string EncryptString(string value, byte[] privateKey = null)
            {
                return DataProtection.EncryptString(value, privateKey ?? pass);
            }
        }

        public class EncryptedSample
        {

            [Symbol("myConnectionString")]
            [Encrypt]
            public string MyConnectionString { get; set; }
        }

        public class ComplexSample
        {
            [Symbol("values")]
            public ValueSample Values { get; set;}

            [Symbol("crypto")]
            public EncryptedSample Crypto {get; set; }
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
