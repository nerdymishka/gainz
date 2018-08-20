using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdyMishka.Flex.Tests
{
    public class FlexAttributeTests
    {

        [Fact]
        public static void Ctor()
        {
            var attr = new FlexAttribute("Tag", "Test");
            Assert.Equal(FlexType.FlexAttribute, attr.FlexType);
            Assert.Equal("Tag", attr.Name);
            Assert.Equal("Test", attr.Value);
        }

        [Fact]
        public static void CtorThrowsArgumentEmptyException()
        {
            Assert.Throws<ArgumentEmptyException>(() =>
            {
                new FlexAttribute(null, null);
            });

            Assert.Throws<ArgumentEmptyException>(() =>
            {
                new FlexAttribute("   ", null);
            });
        }

        [Fact]
        public static void EqualsMethod()
        {
            var attr1 = new FlexAttribute("Tag", "Test");
            var attr2 = new FlexAttribute("Tag", "Test");
            var attr3 = new FlexAttribute("Test", "NotTest");

            Assert.False(attr1.Equals((FlexAttribute)null));
            Assert.True(attr1.Equals(attr2));
            Assert.False(attr1.Equals(attr3));
        }

        [Fact]
        public static void Eq()
        {
            var attr1 = new FlexAttribute("Tag", "Test");
            var attr2 = new FlexAttribute("Tag", "Test");
            var attr3 = new FlexAttribute("Test", "NotTest");

            Assert.False(attr1 == null);
            Assert.False(null == attr1);
            Assert.True(attr1 == attr2);
            Assert.False(attr1 == attr3);
        }


        [Fact]
        public static void Neq()
        {
            var attr1 = new FlexAttribute("Tag", "Test");
            var attr2 = new FlexAttribute("Tag", "Test");
            var attr3 = new FlexAttribute("Test", "NotTest");

            Assert.True(attr1 != null);
            Assert.True(null != attr1);
            Assert.False(attr1 != attr2);
            Assert.True(attr1 != attr3);
        }

        [Fact]
        public static void ToStringMethod()
        {
            var attr = new FlexAttribute("Tag", "Test");
            Assert.Equal("Tag=Test", attr.ToString());
        }

        [Fact]
        public static void Unbind()
        {
            var attr = new FlexAttribute("Tag", "Test");
            var kvp = attr.Unbox();
            Assert.True(kvp is KeyValuePair<string,string>);
            var pair = (KeyValuePair<string, string>)kvp;
            Assert.True(pair.Key == attr.Name);
            Assert.True(pair.Value == attr.Value);
        }
    }
}
