using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EncryptAttribute : Attribute
    {
        public string Encoding { get; set; }

        public EncryptAttribute()
        {
           
        }
    }
}
