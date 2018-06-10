using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NerdyMishka.Flex.Tests
{
    public class FlexSymbolTests
    {
        [Fact]
        public static void Ctor()
        {
            var symbol = new FlexSymbol("Test");
            Assert.Equal(FlexType.FlexSymbol, symbol.FlexType);
        }


        [Fact]
        public static void ToStringMethod()
        {
            var symbol = new FlexSymbol("Test");
            Assert.True(symbol.ToString() == "Test");
        }


        [Fact]
        public static void Unbox()
        {
            var symbol = new FlexSymbol("Test");
            Assert.True(symbol.Unbox().ToString() == "Test");
        }

        [Fact]
        public static void Eq()
        {
            var symbol = new FlexSymbol("Test");
            var symbol1 = new FlexSymbol("Test");

            Assert.False(symbol == null);
            Assert.False(null == symbol);
            Assert.True(symbol == symbol1);
        }


        [Fact]
        public static void EqualsMethod()
        {
            var symbol = new FlexSymbol("Test");
            var symbol1 = new FlexSymbol("Test");

            Assert.False(symbol.Equals((FlexSymbol)null));
            Assert.True(symbol.Equals(symbol1));
            Assert.True(symbol.Equals("Test"));
        }

        [Fact]
        public static void NotEq()
        {
            var symbol = new FlexSymbol("Test");
            var symbol1 = new FlexSymbol("Test");

            Assert.True(symbol != null);
            Assert.True(null != symbol);
            Assert.False(symbol != symbol1);
        }

        [Fact]
        public static void For()
        {
            var symbol = FlexSymbol.For("Test");
            var symbol1 = FlexSymbol.For("Test");
            
            Assert.NotNull(symbol);
            Assert.NotNull(symbol1);
            Assert.True(Object.ReferenceEquals(symbol, symbol1));
        }
    }
}
