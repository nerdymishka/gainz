

using System.Security;
using NerdyMishka.Security.Cryptography;
using Xunit;

namespace Tests 
{
    //[Unit]
    [Trait("tag", "unit")]
    public class CompositeKeyTests 
    {

        [Fact]
        public void Ctor()
        {
            var compositeKey = new CompositeKey();
            Assert.Equal(0, compositeKey.Count);
        }


        [Fact]
        public void AssemblyKey_CreatesValueWithOptions()
        {
           var compositeKey = new CompositeKey();
           var key = compositeKey.AssembleKey();
           Assert.NotNull(key);
           Assert.NotEmpty(key);
        }

        [Fact]
        public void Add_HashedCharacterKeyFragment()
        {
            var jeffAsString = "My name Jeff";
            var jeffAsBytes = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes(jeffAsString);
            var jeffAsSS = new SecureString();
            foreach(var c in jeffAsString)
                jeffAsSS.AppendChar(c);
            
            var compositeKey0 = new CompositeKey();
            var key0 = compositeKey0.AssembleKey();

            var compositeKeyB = new CompositeKey(){
                new CharacterKeyFragment(jeffAsBytes)
            };
            var keyB = compositeKeyB.AssembleKey();

            // String
            var compositeKey1 = new CompositeKey() {
                new HashedCharacterKeyFragment(jeffAsString)
            };
            var key1 = compositeKey1.AssembleKey();
            Assert.NotNull(key1);
            Assert.NotEqual(key0, key1);
            Assert.NotEqual(keyB, key1);

            // Secure String
            var compositeKey2 = new CompositeKey() {
                new HashedCharacterKeyFragment(jeffAsSS)
            };
            var key2 = compositeKey2.AssembleKey();

            Assert.NotNull(key2);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key2);

            // Bytes
            var compositeKey3 = new CompositeKey() {
                new HashedCharacterKeyFragment(jeffAsBytes)
            };
            var key3 = compositeKey2.AssembleKey();

            Assert.NotNull(key3);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key3);


            // CharArray
            var compositeKey4 = new CompositeKey() {
                new HashedCharacterKeyFragment(jeffAsString.ToCharArray())
            };
            var key4 = compositeKey2.AssembleKey();

            Assert.NotNull(key4);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key4);
        }

        [Fact]
        public void Add_CharacterKeyFragment()
        {
            var jeffAsString = "My name Jeff";
            var jeffAsBytes = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes(jeffAsString);
            var jeffAsSS = new SecureString();
            foreach(var c in jeffAsString)
                jeffAsSS.AppendChar(c);
            
            var compositeKey0 = new CompositeKey();
            var key0 = compositeKey0.AssembleKey();

            // String
            var compositeKey1 = new CompositeKey() {
                new CharacterKeyFragment(jeffAsString)
            };
            var key1 = compositeKey1.AssembleKey();
            Assert.NotNull(key1);
            Assert.NotEqual(key0, key1);

            // Secure String
            var compositeKey2 = new CompositeKey() {
                new CharacterKeyFragment(jeffAsSS)
            };
            var key2 = compositeKey2.AssembleKey();

            Assert.NotNull(key2);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key2);

            // Bytes
            var compositeKey3 = new CompositeKey() {
                new CharacterKeyFragment(jeffAsBytes)
            };
            var key3 = compositeKey2.AssembleKey();

            Assert.NotNull(key3);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key3);


            // CharArray
            var compositeKey4 = new CompositeKey() {
                new CharacterKeyFragment(jeffAsString.ToCharArray())
            };
            var key4 = compositeKey2.AssembleKey();

            Assert.NotNull(key4);
            Assert.NotEqual(key0, key1);
            Assert.Equal(key1, key4);
        }
    }
}