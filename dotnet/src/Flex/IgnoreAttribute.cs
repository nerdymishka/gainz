using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        

        // This is a positional argument
        public IgnoreAttribute()
        {
            
        }

       
    }
}
