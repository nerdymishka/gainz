using NerdyMishka.Security.Cryptography;
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