using Xunit;
using System.Linq;
using NerdyMishka.Search.Documents;

namespace NerdyMishka.Search.Tests.Documents
{
    public class DocumentTests
    {
        [Fact]
        public static void Ctor()
        {
            var document = new Document();

            Assert.NotNull(document);
            Assert.Equal(0, document.Count);
        }

        [Fact]
        public static void Enumerable()
        {
            var document1 = new Document();
            document1.Add(Field.Text("body", "Kickin' as I'm Trying to sleep got the mud beneath my shoes"));

            Assert.NotNull(document1);
            Assert.Equal(1, document1.Count);

            var field1 = document1.FirstOrDefault(o => o.Name == "body");
            Assert.NotNull(field1);
            Assert.Equal("body", field1.Name);
        }

        [Fact]
        public static void Add()
        {
            // enumerable element with Add(item)
            // allows list like initalization in C#
            var document = new Document() {
                Field.Keyword("test", "one"),
                Field.Text("body", "i am smelling like a rose")
            };

            Assert.NotNull(document);
            Assert.Equal(2, document.Count);
        }

        [Fact]
        public static void FieldByIndex()
        {
            var document = new Document() {
                Field.Keyword("test", "one"),
                Field.Text("body", "i am smelling like a rose")
            };

            var field = document["body"];
            Assert.NotNull(field);
            Assert.Equal("body", field.Name);

            var nullField = document["BODY"];
            Assert.Null(nullField);

            nullField = document["nope"];
            Assert.Null(nullField);
        }

        
        [Fact]
        public static void Contains()
        {
            var document = new Document() {
                Field.Keyword("test", "one"),
                Field.Text("body", "i am smelling like a rose")
            };

            Assert.False(document.Contains("text"));
            Assert.True(document.Contains("test"));
            Assert.False(document.Contains("TEST"));
        }

        [Fact]
        public static void GetValue()
        {
            var document = new Document() {
                Field.Keyword("test", "one"),
                Field.Text("body", "i am smelling like a rose")
            };

            Assert.Equal("one", document.GetValue("test"));
        }

        [Fact]
        public static void GetValues()
        {
            var document = new Document() {
                Field.Text("body", "i am smelling like a rose"),
                Field.Text("body", "that somebody gave me on my birthday deadbed"),
            };

            var values = document.GetValues("dud");
            Assert.NotNull(values);
            Assert.Empty(values);

            values = document.GetValues("body");
            Assert.Collection(values, 
                f1 => Assert.Equal("i am smelling like a rose", f1),
                f2 => Assert.Equal("that somebody gave me on my birthday deadbed", f2));
        }

        [Fact]
        public static void Remove()
        {
            var document = new Document() {
                Field.Keyword("test", "one"),
                Field.Text("body", "i am smelling like a rose")
            };

            Assert.Equal(2, document.Count);
            Assert.True(document.Contains("test"));
            Assert.False(document.Remove("text"));
            Assert.True(document.Remove("test"));
            Assert.Equal(1, document.Count);
            System.Threading.Thread.Sleep(500);
            Assert.False(document.Contains("test"));

            document = new Document() {
                Field.Text("body", "i am smelling like a rose"),
                Field.Text("body", "that somebody gave me on my birthday deadbed"),
            };

            Assert.Equal(2, document.Count);
            Assert.True(document.Contains("body"));
            Assert.True(document.Remove("body", true));
            Assert.Equal(0, document.Count);
            Assert.False(document.Contains("body"));

             document = new Document() {
                Field.Text("body", "i am smelling like a rose"),
                Field.Text("body", "that somebody gave me on my birthday deadbed"),
            };

            Assert.Equal(2, document.Count);
            Assert.True(document.Contains("body"));
            Assert.True(document.Remove("body"));
            Assert.Equal(1, document.Count);
            Assert.True(document.Contains("body"));
        }

        
    }
}