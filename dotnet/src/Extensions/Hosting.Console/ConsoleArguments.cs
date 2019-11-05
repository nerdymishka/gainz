

using System.Collections.Generic;

namespace NerdyMishka.Extensions.Hosting.Console
{

    public interface IConsoleArguments
    {
        IList<string> Arguments {get; }
    }

    internal class ConsoleArguments : IConsoleArguments
    {
        public ConsoleArguments() {

             this.Arguments = new List<string>();
        }

        public ConsoleArguments(string[] arguments)
        {
            this.Arguments = new List<string>(arguments);
        }

        public IList<string> Arguments { get; }
    }
}