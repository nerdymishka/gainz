using System;

namespace NerdyMishka.Extensions.Console.Options
{
    [AttributeUsage(AttributeTargets.Class, 
        Inherited = true, 
        AllowMultiple = false)]
    public class NounAttribute : ConsoleOptionAttribute
    {

        public NounAttribute(string name)
        {
            this.Name = name;
        }

        public bool IsDefault { get; set;} 
    }
}
