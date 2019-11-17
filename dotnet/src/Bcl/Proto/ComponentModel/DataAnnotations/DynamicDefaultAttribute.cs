using System;

namespace NerdyMishka.ComponentModel.DataAnnotations
{
    /// <summary>
    /// Notates a property that should be used as the default value set if a element
    /// is a data type instead of an object. 
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class DynamicDefaultAttribute : Attribute
    {
       

        // This is a positional argument
        public DynamicDefaultAttribute()
        {
             
        }
    }
}