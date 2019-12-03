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

        [Fact]
        public static void FindEntry_ByTitle()
        {
            var package = new KeePassPackage();

            var entries = package.FindEntriesByTitle("test");
            Assert.NotNull(entries);
            Assert.Empty(entries);

            var group = package.CreateGroup("nerdy/mishka/services", 
                new KeePassGroup() {
                    new KeePassEntry() {
                        Name = "test"
                    },
                    new KeePassEntry() {
                        Name = "frank"
                    },
                    new KeePassEntry() {
                        Name = "TEST"
                    },
                    new KeePassEntry() {
                        Name = "test-2"
                    }
                }  
            , true);

            // case insensitive
            entries = package.FindEntriesByTitle("test");
            Assert.NotNull(entries);
            Assert.Equal(2, entries.Length);

            // case insensitive 
            entries = package.FindEntriesByTitle("test", true, StringValueComparison.Equal);
            Assert.NotNull(entries);
            Assert.Collection(entries, (c) => Assert.Equal("test", c.Name));

            // starts with
            entries = package.FindEntriesByTitle("test", true, StringValueComparison.StartsWith);
            Assert.NotNull(entries);
            Assert.Equal(2, entries.Length);
            Assert.Equal("test-2", entries[1].Name);
        }

        [Fact]
        public static void FindEntity_ByField()
        {
            var package = new KeePassPackage();
        
            var entries = package.FindEntriesMatchesByField("url", "www.google.com");
            Assert.NotNull(entries);
            Assert.Empty(entries);

            var group = package.CreateGroup("nerdy/mishka/services", 
                new KeePassGroup() {
                    new KeePassEntry() {
                        Name = "test",
                        Url = "www.bing.com"
                    },
                    new KeePassEntry() {
                        Name = "frank",
                        Url = "google.com"
                    },
                    new KeePassEntry() {
                        Url = "www.google.com"
                    },
                    new KeePassEntry() {
                        Name = "WWW.google.com",
                        Url = "WWW.google.com"
                    }
                }  
            , true);

            entries = package.FindEntriesMatchesByField("url", "www.google.com");
            Assert.NotNull(entries);
            Assert.Collection(entries, 
                (c) => Assert.Equal("www.google.com", c.Url),
                (c) => Assert.Equal("WWW.google.com", c.Url));
        }
    }
}