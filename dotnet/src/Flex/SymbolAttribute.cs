using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class SymbolAttribute : Attribute
    {

        public SymbolAttribute()
        {

        }

        public SymbolAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }   

        public int Position { get; set; }
    }
}
