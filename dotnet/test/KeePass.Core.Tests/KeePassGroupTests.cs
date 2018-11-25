using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NerdyMishka.KeePass.Core
{
    public static class KeePassGroupTests
    {
        [Fact]
        public static void Ctor()
        {
            var group = new KeePassGroup();
            Assert.NotNull(group.Uuid);
            Assert.NotEmpty(group.Uuid);
            Assert.NotNull(group.AuditFields);
            
            Assert.Null(group.CustomIconUuid);
            Assert.Null(group.DefaultAutoTypeSequence);
            Assert.Null(group.EnableAutoType);
            Assert.Null(group.EnableSearching);
            Assert.Empty(group.Entries);
            Assert.Empty(group.Groups);
          
            Assert.False(group.IsExpanded);
            Assert.NotNull(group.LastTopVisibleEntry);
            Assert.Null(group.Name);
            Assert.Null(group.Notes);
            Assert.Null(group.Owner);
            Assert.Equal(48, group.IconId);
        }

        [Fact]
        public static void Implicit_AddEntry()
        {
            var group = new KeePassGroup("MyGroup") {
                // adds the entry
                new KeePassEntry() {
                    Name = "Test",
                    Url = "www.google.com"
                } 
            };

            Assert.Equal("MyGroup", group.Name);
            Assert.NotEmpty(group.Entries);
            
            var entry = group.Entries.First();
            Assert.Equal("Test", entry.Name);
            Assert.Equal("www.google.com", entry.Url);
        }

        [Fact]
        public static void AddSubGroup()
        {
            var group = new KeePassGroup("root");
            group.Add(new KeePassGroup("group1"));

            Assert.Equal("root", group.Name);
            Assert.NotEmpty(group.Groups);

            var subGroup = group.Groups.First();
            Assert.Equal("group1", subGroup.Name);
        }

        [Fact]
        public static void GetEntry()
        {
            var group = new KeePassGroup("MyGroup") {
                // adds the entry
                new KeePassEntry() {
                    Name = "Test",
                    Url = "www.google.com"
                } 
            };

            var entry = group.Entry(0);
            Assert.NotNull(entry);
            Assert.Equal("Test", entry.Name);

            // case insensitive
            var entry2 = group.Entry("test");
            Assert.NotNull(entry2);
            
            var entry3 = group.Entry("test2");
            Assert.Null(entry3);

            var entry4 = group.Entry(-1);
            Assert.Null(entry4);

            var entry5 = group.Entry(3);
            Assert.Null(entry5);
        }

        [Fact]
        public static void GetGroup()
        {
            var root = new KeePassGroup("root");
            root.Add(new KeePassGroup("group1"));

            var group = root.Group(0);
            Assert.NotNull(group);
            Assert.Equal("group1", group.Name);

            var group2 = root.Group("GROUP1");
            Assert.NotNull(group2);

            var group3 = root.Group("band");
            Assert.Null(group3);

            var group4 = root.Group(-1);
            Assert.Null(group4);

            var group5 = root.Group(4);
            Assert.Null(group5);
        }

        [Fact]
        public static void CopyTo()
        {
            var past = DateTime.UtcNow.AddDays(-1);
            var group = new KeePassGroup("Test", "my notes", 2);
            group.IsExpanded = true;
            group.AuditFields.CreationTime = past;
            group.AuditFields.LastAccessTime = past;
            group.AuditFields.LastModificationTime = past;
            group.Add(new KeePassGroup("sub group"));
            group.Add(new KeePassEntry(true) { Name = "child" });

            var dest = new KeePassGroup("Dest", "x notes", 1);

            // only meta properties should be copied
            group.CopyTo(dest);

            Assert.Empty(dest.Groups);
            Assert.Empty(dest.Entries);

            Assert.Equal(group.IsExpanded, dest.IsExpanded);
            Assert.Equal(group.Uuid, dest.Uuid);
            Assert.Equal(group.AuditFields, group.AuditFields);
        }
    }
}