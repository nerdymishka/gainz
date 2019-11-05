using System;

namespace NerdyMishka.Extensions.Console.Options
{
    [AttributeUsage(AttributeTargets.Class, 
        Inherited = true, 
        AllowMultiple = false)]
    public class RemainingArgsAttribute : ConsoleOptionAttribute
    {

        public RemainingArgsAttribute()
        {
            this.Name = "_RemainingArgs";
        }
    }
}
