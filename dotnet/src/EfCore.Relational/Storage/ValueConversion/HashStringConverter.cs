
using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using NerdyMishka.Flex;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace NerdyMishka.EfCore.Storage.ValueConversion
{
    
    public class HashStringConverter : ValueConverter<string, string>
    {
        public HashStringConverter(
             IFlexHashProvider provider, int iterations = 64000
            ) : base(
                v => provider.ComputeHash(v, iterations),
                v => provider.ComputeHash(v, iterations))
        {

        }
    }
}