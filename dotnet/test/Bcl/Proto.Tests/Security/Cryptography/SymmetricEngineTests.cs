
using System;
using System.IO;
using NerdyMishka.Security.Cryptography;
using Xunit;

namespace Tests
{
    public class SymmetricEngineTests
    {

        [Fact]
        public static void GenerateHeader_WithPrivateKey()
        {
            using(var engine = new SymmetricEngine())
            {
                var privateKey = PasswordGenerator.GenerateAsBytes(20);
                using(var header = engine.GenerateHeader(null, privateKey))
                {
                    Assert.NotNull(header);
                    Assert.Equal(1, header.Version);
                    Assert.Equal(SymmetricAlgorithmTypes.AES, header.SymmetricAlgorithmType);
                    Assert.Equal(KeyedHashAlgorithmTypes.HMACSHA256, header.KeyedHashAlgorithmType);
                    Assert.Equal(0, header.MetaDataSize);
                    Assert.NotEqual(0, header.SigningSaltSize);
                    Assert.NotEqual(0, header.SymmetricSaltSize);
                    Assert.Equal(8, header.SigningSaltSize);
                    Assert.Equal(8, header.SymmetricSaltSize);
                    Assert.NotEqual(0, header.IvSize);
                    Assert.NotEqual(0, header.HashSize);       
                    Assert.NotEqual(0, header.Iterations);
                    Assert.NotNull(header.SymmetricKey);
                    Assert.NotNull(header.IV);
                    Assert.NotNull(header.SigningKey);
                    Assert.NotNull(header.Bytes);
                    
                    Assert.NotEmpty(header.SymmetricKey);
                    Assert.NotEmpty(header.IV);
                    Assert.NotEmpty(header.SigningKey);
                    Assert.NotEqual(privateKey, header.SymmetricKey);
                    Assert.NotEmpty(header.Bytes);

                    using(var ms = new MemoryStream(header.Bytes))
                    using(var br = new BinaryReader(ms))
                    {
                        Assert.Equal(1, br.ReadInt16());
                        Assert.Equal((short)SymmetricAlgorithmTypes.AES, br.ReadInt16());
                        Assert.Equal((short)KeyedHashAlgorithmTypes.HMACSHA256, br.ReadInt16());
                        Assert.Equal(header.MetaDataSize, br.ReadInt32());
                        Assert.Equal(header.Iterations, br.ReadInt32());
                        Assert.Equal(header.SymmetricSaltSize, br.ReadInt16());
                        Assert.Equal(header.SigningSaltSize, br.ReadInt16());
                        Assert.Equal(header.IvSize, br.ReadInt16());
                        Assert.Equal(header.SymmetricKeySize, br.ReadInt16());
                        Assert.Equal(header.HashSize, br.ReadInt16());

                        byte[] metadata = null;
                        byte[] symmetricSalt = null;
                        byte[] signingSalt = null;
                        byte[] iv = null;
                        byte[] symmetricKey = null;
                        byte[] hash = null;

                        if(header.MetaDataSize > 0)
                            metadata = br.ReadBytes(header.MetaDataSize);
                        
                        if(header.SymmetricSaltSize > 0)
                            symmetricSalt = br.ReadBytes(header.SymmetricSaltSize);

                        if(header.SigningSaltSize > 0)
                            signingSalt = br.ReadBytes(header.SigningSaltSize);

                        if(header.IvSize > 0)
                            iv = br.ReadBytes(header.IvSize);

                        if(header.SymmetricKeySize > 0)
                            symmetricKey = br.ReadBytes(header.SymmetricKeySize);

                        if(header.HashSize > 0)
                            hash = br.ReadBytes(header.HashSize);

                    
                        Assert.Null(metadata);
                        Assert.NotNull(hash);
                        Assert.NotNull(signingSalt);
                        Assert.NotNull(symmetricSalt);
                        
                        // header property has a copy but does not
                        // write it to the file header when a private key
                        // is provided. 
                        Assert.Null(symmetricKey);
                        Assert.NotNull(iv);
                        Assert.NotEmpty(hash);
                        Assert.NotEmpty(signingSalt);
                        Assert.NotEmpty(symmetricSalt);
                
                        Assert.NotEmpty(iv);
                    }        
                }
            }
        }


        [Fact]
        public static void ReaderHeader_WithPrivateKey()
        {
            using(var engine = new SymmetricEngine())
            {
                var privateKey = PasswordGenerator.GenerateAsBytes(20);
                byte[] data = null;
                using(var header = engine.GenerateHeader(null, privateKey))
                {
                    data = new byte[header.Bytes.Length];
                    Array.Copy(header.Bytes, 0, data, 0, header.Bytes.Length);
                }

                var ms = new MemoryStream(data);
                using(var header = engine.ReadHeader(ms, privateKey))
                {
                    ms.Position = 0;

                    Assert.NotNull(header);
                    Assert.Equal(1, header.Version);
                    Assert.Equal(SymmetricAlgorithmTypes.AES, header.SymmetricAlgorithmType);
                    Assert.Equal(KeyedHashAlgorithmTypes.HMACSHA256, header.KeyedHashAlgorithmType);
                    Assert.Equal(0, header.MetaDataSize);
                    Assert.NotEqual(0, header.SigningSaltSize);
                    Assert.NotEqual(0, header.SymmetricSaltSize);
                    Assert.NotEqual(0, header.IvSize);
                    Assert.NotEqual(0, header.HashSize);       
                    Assert.NotEqual(0, header.Iterations);
                    Assert.NotNull(header.SymmetricKey);
                    Assert.NotNull(header.IV);
                    Assert.NotNull(header.SigningKey);
                    Assert.NotNull(header.Bytes);
                    
                    Assert.NotEmpty(header.SymmetricKey);
                    Assert.NotEmpty(header.IV);
                    Assert.NotEmpty(header.SigningKey);
                    Assert.NotEqual(privateKey, header.SymmetricKey);
                    Assert.NotEmpty(header.Bytes);

                    ms.Position = 0;
                    using(var br = new BinaryReader(ms))
                    {
                        Assert.Equal(1, br.ReadInt16());
                        Assert.Equal((short)SymmetricAlgorithmTypes.AES, br.ReadInt16());
                        Assert.Equal((short)KeyedHashAlgorithmTypes.HMACSHA256, br.ReadInt16());
                        Assert.Equal(header.MetaDataSize, br.ReadInt32());
                        Assert.Equal(header.Iterations, br.ReadInt32());
                        Assert.Equal(header.SymmetricSaltSize, br.ReadInt16());
                        Assert.Equal(header.SigningSaltSize, br.ReadInt16());
                        Assert.Equal(header.IvSize, br.ReadInt16());
                        Assert.Equal(header.SymmetricKeySize, br.ReadInt16());
                        Assert.Equal(header.HashSize, br.ReadInt16());

                        byte[] metadata = null;
                        byte[] symmetricSalt = null;
                        byte[] signingSalt = null;
                        byte[] iv = null;
                        byte[] symmetricKey = null;
                        byte[] hash = null;

                        if(header.MetaDataSize > 0)
                            metadata = br.ReadBytes(header.MetaDataSize);
                        
                        if(header.SymmetricSaltSize > 0)
                            symmetricSalt = br.ReadBytes(header.SymmetricSaltSize);

                        if(header.SigningSaltSize > 0)
                            signingSalt = br.ReadBytes(header.SigningSaltSize);

                        if(header.IvSize > 0)
                            iv = br.ReadBytes(header.IvSize);

                        if(header.SymmetricKeySize > 0)
                            symmetricKey = br.ReadBytes(header.SymmetricKeySize);

                        if(header.HashSize > 0)
                            hash = br.ReadBytes(header.HashSize);

                    
                        Assert.Null(metadata);
                        Assert.NotNull(hash);
                        Assert.NotNull(signingSalt);
                        Assert.NotNull(symmetricSalt);
                        
                        // header property has a copy but does not
                        // write it to the file header when a private key
                        // is provided. 
                        Assert.Null(symmetricKey);
                        Assert.NotNull(iv);
                        Assert.NotEmpty(hash);
                        Assert.NotEmpty(signingSalt);
                        Assert.NotEmpty(symmetricSalt);
                
                        Assert.NotEmpty(iv);
                    }        

                }
            }
        }

        [Fact]
        public void EncryptDecryptBlob_WithPrivateKey()
        {
            using(var engine = new SymmetricEngine())
            {
                var pw = PasswordGenerator.GenerateAsBytes(20);
                var text = NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes("My name Jeff");

                var encryptedBlob = engine.EncryptBlob(text, pw);
                Assert.NotEmpty(encryptedBlob);
                Assert.NotEqual(text, encryptedBlob);
                
                var text2 = engine.DecryptBlob(encryptedBlob, pw);
                Assert.Equal(text, text2);
            }
        }
    }
}