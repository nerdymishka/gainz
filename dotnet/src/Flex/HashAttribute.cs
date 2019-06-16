using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class HashAttribute : Attribute
    {
        
        public string Algorithm { get; set; }

        // This is a positional argument
        public HashAttribute()
        {
           
        }
    }
}
