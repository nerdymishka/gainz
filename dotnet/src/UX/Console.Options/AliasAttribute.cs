using System;

namespace NerdyMishka.Extensions.Console.Options
{
    public abstract class AliasAttribute : ConsoleOptionAttribute
    {

        public string Alias { get; set; }
    }
}
