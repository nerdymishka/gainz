using System;
using System.Linq;
using Xunit;
using NerdyMishka.Security.Cryptography;
using System.Security.Cryptography;

namespace NerdyMishka.Windows.Vault.Tests
{
    public class IntegrationTests
    {
        [Fact]
        public void DataProtection_WithCompositeKey()
        {
            var pw = "my-great-and-terrible-pw";
            var key = new CompositeKey();
            var key2 = new CompositeKey();
            key.AddPassword("shuckera");
            key.AddDerivedPassword("nonsense", 1081);
            key2.AddPassword("shuckera");
            key2.AddDerivedPassword("nonsense", 1081);

            var d = key.First().UnprotectAndCopyData();
            var e = key2.First().UnprotectAndCopyData();
            var h = key.ElementAt(1).UnprotectAndCopyData();
            var i = key2.ElementAt(1).UnprotectAndCopyData();
            Assert.Equal(e, d);
            Assert.Equal(h, i);

            var x = CompositeKey.UnprotectAndConcatData(key, SHA256.Create());
            var y = CompositeKey.UnprotectAndConcatData(key2, SHA256.Create());

            Assert.Equal(y, x);

            var symKey = PasswordGenerator.GenerateAsBytes(32);
            var first = key.AssembleKey(symKey);
            var second = key.AssembleKey(symKey);

            Assert.Equal(first, second);

            var encryptedData = CompositeDataProtection.EncryptString(pw, key);
            Assert.NotEqual(pw, encryptedData);

            var decryptedData = CompositeDataProtection.DecryptString(encryptedData, key);
            Assert.Equal(pw, decryptedData);
        }

        [Fact]
        public void CrudTests()
        {
            var items = VaultManager.List();
            Assert.NotNull(items);

            // may not be true on an empty system
            // Assert.True(items.Length > 0);
            int count = items.Length;
            var next = VaultManager.Create();
            next.Key = "gainz/test";
            var pw = "my-great-and-terrible-password";
            next.SetBlob(pw);
            Assert.Equal(pw, next.GetBlobAsString());
            
            VaultManager.Write(next);
            
            items = VaultManager.List();
            Assert.True(count + 1 == items.Length);

            var findOne = VaultManager.Read("gainz/test");
            Assert.NotNull(findOne);
            Assert.Equal(pw, findOne.GetBlobAsString());
            
            VaultManager.Delete("gainz/test");
            items = VaultManager.List();
            Assert.True(count  == items.Length);
        }
    }
}
