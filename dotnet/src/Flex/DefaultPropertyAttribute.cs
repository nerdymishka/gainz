using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DefaultPropertyAttribute : Attribute
    {
       

        // This is a positional argument
        public DefaultPropertyAttribute()
        {
             
        }

        public string PropertyName { get; set; }
    }
}
