using System;

namespace NerdyMishka.Extensions.Console.Options
{
    public abstract class ConsoleOptionAttribute : Attribute
    {
        public string Name { get; protected set; }

        public string Description { get; set; }

        public bool IsHidden { get; set; }
    }
}
