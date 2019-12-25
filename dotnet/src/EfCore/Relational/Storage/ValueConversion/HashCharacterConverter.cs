
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class HashCharacterConverter : ValueConverter<char[], char[]>
    {
        public HashCharacterConverter(
             IHashProvider provider
            ) : base(
                v => Convert.ToBase64String(
                        provider.ComputeHash(NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes(v)))
                        .ToCharArray(),
                v => v)
        {
            
        }
    }
}