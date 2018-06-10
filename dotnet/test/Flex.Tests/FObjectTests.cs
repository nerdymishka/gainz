using System;
using Xunit;

namespace NerdyMishka.Flex.Tests
{
    public class FObjectTests
    {
        [Fact]
        public static void Ctor()
        {
            var obj = new FObject();
            Assert.Equal(FlexType.FObject, obj.FlexType);
        }

        [Fact]
        public static void ToStringValue()
        {
            var obj = new FObject();
            Assert.Equal(string.Empty, obj.ToString());
        }


        [Fact]
        public static void Unbox()
        {
            var obj = new FObject();
            Assert.Null(obj.Unbox());
        }


    }
}
