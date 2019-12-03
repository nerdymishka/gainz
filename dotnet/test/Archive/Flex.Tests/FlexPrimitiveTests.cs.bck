using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdyMishka.Flex.Tests
{
    public class FlexPrimitiveTests
    {
        [Fact]
        public static void Ctor()
        {
            var flexPrimitive = new FlexPrimitive(20);
            Assert.Equal(FlexType.FlexPrimitive, flexPrimitive.FlexType);
            Assert.Equal(typeof(int), flexPrimitive.ClrType);
            Assert.NotNull(flexPrimitive.Attributes);
            Assert.Equal(20, flexPrimitive);
        }

        [Fact]
        public static void ToStringMethod()
        {
            var flexPrimitive = new FlexPrimitive(20L);
            Assert.Equal("20", flexPrimitive.ToString());
        }

        [Fact]
        public static void Eq()
        {
            var flex1 = new FlexPrimitive(20L);
            var flex2 = new FlexPrimitive("20");
            var flex3 = new FlexPrimitive(20L);
            var flex4 = new FlexPrimitive(20);

            Assert.False(flex1 == null);
            Assert.False(flex1 == flex2);
            Assert.True(flex1 == flex3);
            Assert.False(flex1 == flex4);
        }


        [Fact]
        public static void NotEq()
        {
            var flex1 = new FlexPrimitive(20L);
            var flex2 = new FlexPrimitive("20");
            var flex3 = new FlexPrimitive(20L);
            var flex4 = new FlexPrimitive(20);

            Assert.True(flex1 != null);
            Assert.True(flex1 != flex2);
            Assert.False(flex1 != flex3);
            Assert.True(flex1 != flex4);
        }

        [Fact]
        public static void Unbox()
        {
            var flexPrimitive = new FlexPrimitive(20L);
            Assert.Equal(20L, flexPrimitive.Unbox());
        }

        [Fact]
        public static void CastByte()
        {
            var bit = new Byte();
            var flexPrimitive = new FlexPrimitive(bit);
            Assert.Equal(bit, flexPrimitive);
        }

        [Fact]
        public static void CastNullableByte()
        {
            byte? bit = null;
            var flexPrimitive = new FlexPrimitive(bit);
            Assert.Equal(bit, flexPrimitive);
            Assert.Null((byte?)flexPrimitive);

            bit = new byte();
            flexPrimitive = new FlexPrimitive(bit);
            Assert.Equal(bit, flexPrimitive);
            Assert.NotNull((byte?)flexPrimitive);
        }

        [Fact]
        public static void CastChar()
        {
            var value = new Char();
            var flexPrimitive = new FlexPrimitive(value);
            Assert.Equal(value, flexPrimitive);
        }


        [Fact]
        public static void CastNullableChar()
        {
            char? value = null;
            var flexPrimitive = new FlexPrimitive(value);
            Assert.Equal(value, flexPrimitive);
            Assert.Null((char?)flexPrimitive);

            value = new char();
            flexPrimitive = new FlexPrimitive(value);
            Assert.Equal(value, flexPrimitive);
            Assert.NotNull((char?)flexPrimitive);
        }
    }
}
