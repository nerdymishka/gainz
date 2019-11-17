using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.ComponentModel.DataAnnotations
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {
        public IgnoreAttribute()
        {
            
        }
    }
}
