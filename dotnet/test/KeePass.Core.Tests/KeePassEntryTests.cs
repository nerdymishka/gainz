using System;
using System.Collections.Generic;
using Xunit;

namespace NerdyMishka.KeePass.Core
{
    public static class KeePassEntryTests
    {
        [Fact]
        public static void Ctor()
        {
            var entry = new KeePassEntry();

            Assert.NotNull(entry.AuditFields);
            Assert.NotNull(entry.Uuid);
            Assert.NotEmpty(entry.Uuid);
            Assert.Null(entry.AutoType);
            Assert.Null(entry.BackgroundColor);
            Assert.Null(entry.ForegroundColor);
            Assert.Null(entry.CustomIconUuid);
            Assert.Null(entry.UserName);
            Assert.Null(entry.Url);
            Assert.Empty(entry.Tags);
            Assert.Null(entry.OverrideUrl);
            Assert.Null(entry.Notes);
            Assert.Null(entry.Name);
            Assert.Empty(entry.Binaries);
            Assert.Empty(entry.Strings);
        }

        [Fact]
        public static void Ctor_Create()
        {
            var entry = new KeePassEntry(true);

            Assert.NotNull(entry.AuditFields);
            Assert.NotNull(entry.Uuid);
            Assert.NotEmpty(entry.Uuid);
            Assert.Null(entry.AutoType);
            Assert.Null(entry.BackgroundColor);
            Assert.Null(entry.ForegroundColor);
            Assert.Null(entry.CustomIconUuid);
            Assert.Null(entry.UserName);
            Assert.Null(entry.Url);
            Assert.Empty(entry.Tags);
            Assert.Null(entry.OverrideUrl);
            Assert.Null(entry.Notes);
            Assert.Null(entry.Name);
            Assert.Empty(entry.Binaries);
            Assert.NotEmpty(entry.Strings);
        }


        [Fact]
        public static void SetValues()
        {
            var entry = new KeePassEntry(true);
            var name = "MyEntry";
            var url = "https://google.com";
            var tags = new List<string>() {"google"};
            var notes = "Check. Check. 1. 2.";
            var username = "nobody@gmail.com";
            var pw = "great-and-terrible-password";
            var pw2 = "surely, I am securely stored";
            var pwBytes = System.Text.Encoding.UTF8.GetBytes("Mus3$2");
            var ss = new System.Security.SecureString();

            foreach(var c in pw2)
                ss.AppendChar(c);

            entry.Name = name;
            entry.Url = url;
            entry.Tags = tags;
            entry.Notes = notes;
            entry.UserName = username;
            entry.SetPassword(pw);

            Assert.Equal(name, entry.Name);
            Assert.Equal(url, entry.Url);
            Assert.Equal(tags, entry.Tags);
            Assert.Equal(notes, entry.Notes);
            Assert.Equal(username, entry.UserName);
            Assert.Equal(pw, entry.UnprotectPassword());

            entry.SetPassword(ss);
            Assert.Equal(pw2, entry.UnprotectPassword());

            entry.SetPassword(pwBytes);
            Assert.Equal("Mus3$2", entry.UnprotectPassword());
        }
    }
}
