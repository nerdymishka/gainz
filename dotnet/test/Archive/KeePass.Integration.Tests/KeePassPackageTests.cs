﻿using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using NerdyMishka.KeePass.Xml;
using NerdyMishka.KeePass.Cryptography;
using NerdyMishka.KeePass;

public class KeePass_Package_Theories
{
    public string Title = "sample entry";


    [Fact] 
    public void Scenario_Multiple_Edits_And_Saves()
    {
        var path = Env.ResolvePath("~/Resources/MultipleSaves.kdbx");
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        var key = new MasterKey() {
            new MasterKeyPassword("my-great-and-terrible-password")
        };

        var pw1 = "my-bad-pws";
        var pw2 = "my-next-pw@#42sdw";

        var cpw1 = "next-bad-pw";
        var cpw2 = "my-plus-plus-A2342d1#";


        try {
            byte[] xorKey = null;
            byte[] pad = null;
            using (var package = new KeePassPackage(key, "root"))
            {
                xorKey = package.HeaderInfo.RandomByteGeneratorCryptoKey;
                var entry1 = package.CreateEntry($"root/my-cert", System.Text.Encoding.UTF8.GetBytes(pw1));
                entry1.AttachFile(Env.ResolvePath("~/Resources/my-cert.pfx"));
                entry1.SetPassword(pw1);

                package.CreateEntry("root/my-test-pass", System.Text.Encoding.UTF8.GetBytes(pw2));
                package.SaveKdbx(key, path);

                Assert.Equal(pw1, entry1.UnprotectPassword());
                Assert.Equal(xorKey, package.HeaderInfo.RandomByteGeneratorCryptoKey);
                var x = new Salsa20RandomByteGenerator();
                x.Initialize(xorKey);
                pad = x.NextBytes(pw1.Length);
            }

            using(var package = new KeePassPackage())
            {
                package.OpenKdbx(key, path);
                Assert.Equal(xorKey, package.HeaderInfo.RandomByteGeneratorCryptoKey);
                
                
                var x = new Salsa20RandomByteGenerator();
                x.Initialize(xorKey);
                var pad2 = x.NextBytes(pw1.Length);
                Assert.Equal(pad, pad2);

                var root = package.Document.RootGroup;
                var entry = package.FindEntry($"root/my-cert");

                Assert.NotNull(entry);
                Assert.Equal(pw1, entry.UnprotectPassword());

                entry.SetPassword(cpw1);
                Assert.Equal(cpw1, entry.UnprotectPassword());
                package.SaveKdbx(key, path);
            }

            using(var package = new KeePassPackage())
            {
                package.OpenKdbx(key, path);
                var root = package.Document.RootGroup;
                var entry = package.FindEntry($"root/my-cert");

                Assert.NotNull(entry);
                Assert.Equal(cpw1, entry.UnprotectPassword());

                var entry2 = package.FindEntry("root/my-test-pass");
                Assert.Equal(pw2, entry2.UnprotectPassword());

                entry2.SetPassword(cpw2);
                Assert.Equal(cpw2, entry2.UnprotectPassword());
                package.SaveKdbx(key, path);
            }

            using(var package = new KeePassPackage())
            {
                package.OpenKdbx(key, path);
                var root = package.Document.RootGroup;
                var entry1 = package.FindEntry($"root/my-cert");

                Assert.Equal(cpw1, entry1.UnprotectPassword());

                var entry2 = package.FindEntry("root/my-test-pass");
                Assert.Equal(cpw2, entry2.UnprotectPassword());
               
            }

        } finally {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }

      

    }


    [Fact]
    public void Open_With_Password()
    {
        var pw = new System.Security.SecureString();
        Array.ForEach("password".ToCharArray(), pw.AppendChar);

        var kdbx = Env.ResolvePath("~/Resources/NewDatabase.kdbx");
        var key = new MasterKey() {
            new MasterKeyPassword(pw),
        };
       
     
        using (var package = KeePassPackage.Open(key, kdbx, new KeePassPackageXmlSerializer()))
        {
            var matches = package.FindEntriesByTitle(this.Title);
            Assert.NotNull(matches);
            Assert.True(matches.Length > 0);
            Assert.NotNull(matches[0]);
            Assert.Equal("Password", matches[0].UnprotectPassword());

            var entry = package.FindEntry("NewDatabase/Sample Entry #2");

            Assert.NotNull(entry);
            Assert.Equal("Michael321", entry.UserName);
        }
            

    }

    [Fact]
    public void Open_And_Merge()
    {
        var key = new MasterKey
        {
            new MasterKeyPassword("password")
        };

        var package = new KeePassPackage(key).OpenKdbx(Env.ResolvePath("~/Resources/NewDatabase.kdbx"));
        var package2 = new KeePassPackage(key, package.Document.RootGroup.Name);

        package.Merge(package2);

        package2.SaveKdbx(Env.ResolvePath("~/Resources/NewDatabaseCopy.kdbx"));
    }

    [Fact]
    public void Write_With_Password()
    {
        var path = Env.ResolvePath("~/Resources/NewDatabase23.kdbx");
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);

        PortableSalsa20ProtectedDataProvider.Setup();

        var key = new MasterKey().AddPassword("megatron");
       
        var package = new KeePassPackage(
            key, 
            "Sample");

        var pw = System.Text.Encoding.UTF8.GetBytes("Password");
        var entry = package.CreateEntry("Sample/Test", password: pw, force: true);


        package.SaveKdbx(path);



        var package2 = new KeePassPackage(key);
        package2.OpenKdbx(Env.ResolvePath("~/Resources/NewDatabase23.kdbx"));

        Assert.NotNull(package2);
        Assert.NotNull(package2.Document);
        Assert.NotNull(package2.Document.Groups);
        Assert.NotEmpty(package2.Document.Groups);
        Assert.NotNull(package2.Document.Groups.ElementAt(0).Entries);
        Assert.NotEmpty(package2.Document.Groups.ElementAt(0).Entries);

        var entry2 = package2.Document.Groups.ElementAt(0).Entries.ElementAt(0);
        var titleString = entry2.Strings.SingleOrDefault(o => o.Key == "Title");
        var passwordString = entry2.Strings.SingleOrDefault(o => o.Key == "Password");
        Assert.Equal("Test", titleString.Value.UnprotectAsString());
        Assert.Equal("Password", passwordString.Value.UnprotectAsString());

        Assert.Equal("Test", entry2.Name);
        Assert.Equal("Password", entry2.UnprotectPassword());
    }


    private static string GetKeyFilePath()
    {
        var roaming = Environment.GetEnvironmentVariable("APPDATA");
        if (string.IsNullOrWhiteSpace(roaming))
        {
            roaming = Environment.GetEnvironmentVariable("HOME");
        }

        if (string.IsNullOrWhiteSpace(roaming))
            throw new InvalidProgramException("Could not determine home directory");

        roaming = System.IO.Path.Combine(roaming, "KeePass", "ProtectedUserKey.bin");

        return roaming;
    }


    /*
    [Fact]
    public void Open_Save_With_UserAccount()
    {
        var file = Env.ResolvePath("~/Resources/NewDatabase24.bin");
        var path = Env.ResolvePath("~/Resources/NewDatabase24.kdbx");

        if (System.IO.File.Exists(file))
            System.IO.File.Delete(file);


        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);


        KeePassManagedProtectedDataProvider.Setup();
        var key = new MasterKey() {
            new MasterKeyUserAccount(file)
        };
        

        var package = new KeePassPackage(key);
        var group = package.CreateGroup("Sample");


        var entry = new KeePassEntry(true);
        var fields = entry.Fields;
        
        
        fields.Title = "Test";
        fields.Password = "Password";
        group.Add(entry);

        var entry2 = (IKeePassEntry)new KeePassEntry(true);
        entry2.Uuid = entry.Uuid;
        entry2.Name = entry.Name;

        group.Add(entry2);

        Assert.Equal(1, group.Entries.Count());

        package.SaveKdbx(path);

        key = new MasterKey();
        key.Add(new MasterKeyUserAccount(file));
        var package2 = new KeePassPackage(key: key).OpenKdbx(path);


        Assert.NotNull(package2);
        Assert.NotNull(package2.Document);
        Assert.NotNull(package2.Document.Groups);
        Assert.Equal(1, package2.Document.Groups.Count());
        Assert.NotNull(package2.Document.RootGroup.Entries);
        Assert.Equal(1, package2.Document.RootGroup.Entries.Count());

        entry2 = package2.Document.RootGroup.Entry(0);
        var titleString = entry2.Strings.SingleOrDefault(o => o.Key == "Title");
        var passwordString = entry2.Strings.SingleOrDefault(o => o.Key == "Password");
        Assert.Equal("Test", titleString.Value.UnprotectAsString());
        Assert.Equal("Password", entry2.Fields.UnprotectPassword());

        Assert.Equal("Test", entry2.Name);
        Assert.Equal("Password", entry2.UnprotectPassword());
    }
     */


    
}