using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;

namespace NerdyMishka.Flex.Tests
{
    public class FlexAttributeListTests
    {

        [Fact]
        public static void Ctor()
        {
            var list = new FlexAttributeList();
            Assert.Equal(0, list.Count);
            Assert.True(list.FlexType == FlexType.FlexAttributeList);
        }

        [Fact]
        public static void Add()
        {
            var list = new FlexAttributeList();
            list.Set("Tag", "String");

            Assert.Equal(1, list.Count);
            Assert.Equal(1, (int)list.Count());
        }

        [Fact]
        public static void StringIndexer()
        {
            var list = new FlexAttributeList();
            list.Set("Tag", "String");

            Assert.Equal("String", list["Tag"]);


            
            var r = new Random();
            var size = r.Next(3000, 5000);

            for (var i = 0; i < size; i++)
            {
                list.Set($"Tag{i}", $"Value{i}");
            }

            var last = size - 1;
            Assert.Equal($"Value{last}", list[$"Tag{last}"]);
        }

        [Fact]
        public static void IntIndexer()
        {
            var list = new FlexAttributeList();
            var r = new Random();
            var size = r.Next(10, 5000);

            for (var i = 0; i < size; i++)
            {
                list.Set($"Tag{i}", $"Value{i}");
            }

            var last = size - 1;
            Assert.Equal($"Value{last}", list[last]);
        }

        [Fact]
        public static void AddMany()
        {
            var list = new FlexAttributeList();
            var r = new Random();
            var size = r.Next(6000, 20000);
            for(var i = 0; i < size; i++)
            {
                list.Set($"Tag{i}", $"Value{i}");
            }

            Assert.Equal(size, list.Count);
        }


        [Fact]
        public static void Remove()
        {
            var list = new FlexAttributeList();
            var r = new Random();
            var size = r.Next(10, 5000);

            for (var i = 0; i < size; i++)
            {
                list.Set($"Tag{i}", $"Value{i}");
            }

            for(var i = 0; i < size; i++)
            {
                list.Remove($"Tag{i}");
            }

            
        }
    }
}
