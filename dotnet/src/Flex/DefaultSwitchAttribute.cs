using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class SwitchAttribute : Attribute
    {
        
        public bool IsDefault { get; set; }

        public string Yes { get; set; } = "true";

        public string No { get; set; } = "false";

        // This is a positional argument
        public SwitchAttribute()
        {

        }

    }
}
