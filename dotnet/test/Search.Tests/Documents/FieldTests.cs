using Xunit;
using NerdyMishka.Search.Documents;

namespace NerdyMishka.Search.Tests.Documents
{
    public class FieldTests
    {
        [Fact]
        public static void KeyWord()
        {
            var field = Field.Keyword("keyword", "test");
            Assert.NotNull(field);
            Assert.Equal("keyword", field.Name);
            Assert.Equal("test", field.Value);
            Assert.Equal(StorageStrategy.Store, field.Storage);
            Assert.Equal(IndexStrategy.NotAnalyzed, field.Index);
            Assert.Null(field.Reader);
        }

        [Fact]
        public static void Stored()
        {
            var field = Field.Stored("stored", "test1");
            Assert.NotNull(field);
            Assert.Equal("stored", field.Name);
            Assert.Equal("test1", field.Value);
            Assert.Equal(StorageStrategy.Store, field.Storage);
            Assert.Equal(IndexStrategy.None, field.Index);
            Assert.Null(field.Reader);
        }

        [Fact]
        public static void Indexed()
        {
            var field = Field.Indexed("indexed", "x1");
            Assert.NotNull(field);
            Assert.Equal("indexed", field.Name);
            Assert.Equal("x1", field.Value);
            Assert.Equal(StorageStrategy.None, field.Storage);
            Assert.Equal(IndexStrategy.Analyzed, field.Index);
            Assert.Null(field.Reader);
        }

        [Fact]
        public static void Text()
        {
            var field = Field.Text("text", "my awesome para");
            Assert.NotNull(field);
            Assert.Equal("text", field.Name);
            Assert.Equal("my awesome para", field.Value);
            Assert.Equal(StorageStrategy.Store, field.Storage);
            Assert.Equal(IndexStrategy.Analyzed, field.Index);
            Assert.Null(field.Reader);
        }
    }
}