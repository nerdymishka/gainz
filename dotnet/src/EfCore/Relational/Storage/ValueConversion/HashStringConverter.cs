
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class HashStringConverter : ValueConverter<string, string>
    {
        public HashStringConverter(
             IHashProvider provider
            ) : base(
                v => 
                   Convert.ToBase64String(
                       provider.ComputeHash(NerdyMishka.Text.Encodings.Utf8NoBom.GetBytes(v))),
                v => v)
        {

        }
    }
}