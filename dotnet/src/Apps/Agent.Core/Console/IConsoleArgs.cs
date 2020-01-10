using System.Collections.Generic;

namespace NerdyMishka.Console
{
    /// <summary>
    /// Allows console parameters/arguments to added as a service for
    /// Dependency Injection purposes.
    /// </summary>
    public interface IConsoleArgs
    {
        /// <summary>
        /// Gets or sets the console arguments.
        /// </summary>
        /// <value></value>
        IReadOnlyCollection<string> Args { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConsoleArgs 
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ConsoleArgs" />
        /// </summary>
        /// <param name="args">the arguments</param>
        public ConsoleArgs(IReadOnlyCollection<string> args)
        {
            this.Args = args;
        }


        /// <summary>
        /// Gets or sets the console arguments.
        /// </summary>
        /// <value></value>
        public IReadOnlyCollection<string> Args { get; private set; }
    }
}

