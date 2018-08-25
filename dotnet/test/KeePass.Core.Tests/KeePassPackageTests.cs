using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NerdyMishka.KeePass.Core
{
    public static class KeePassPackageTests
    {

        [Fact]
        public static void CreateGroup()
        {
            var package = new KeePassPackage();

            var group = package.CreateGroup("nerdy/mishka/services", true);
            Assert.NotNull(group);
            Assert.Equal("services", group.Name);

            Assert.Equal("nerdy", package.Document.RootGroup.Name);
        }

        [Fact]
        public static void CreateGroup_WithEntity()
        {
            var package = new KeePassPackage();

            var group = package.CreateGroup("nerdy/mishka/services", 
                new KeePassGroup() {
                    new KeePassEntry() {
                        Name = "test"
                    }
                }  
            , true);

             Assert.NotNull(group);
             Assert.Equal("services", group.Name);
             Assert.NotNull(group.Entry(0));

        }

        [Fact]
        public static void FindEntry()
        {
            var package = new KeePassPackage();

            var group = package.CreateGroup("nerdy/mishka/services", 
                new KeePassGroup() {
                    new KeePassEntry() {
                        Name = "test"
                    }
                }  
            , true);

            var entry = package.FindEntry("nerdy/mishka/services/test");
            Assert.NotNull(entry);
            Assert.Equal("test", entry.Name);
        }
    }

}