using System;

namespace NerdyMishka.Management.Automation
{
    public interface IMessageFormatter
    {
        string Format(string message);
    }

    public class MessageFormatter : IMessageFormatter
    {
        public Func<string, string> invoke;

        public MessageFormatter(Func<string, string> invoke)
        {
            if(invoke == null)
                throw new ArgumentNullException(nameof(invoke));

            this.invoke = invoke;
        }

        public string Format(string message)
        {
            return this.invoke(message);
        }

        public static IMessageFormatter ErrorFormatter => new MessageFormatter((message) => $"ERROR: {message}");

        public static IMessageFormatter WarningFormatter => new MessageFormatter((message) => $"WARN: {message}");

        public static IMessageFormatter VerboseFormatter => new MessageFormatter((message) => $"VERBOSE: {message}");

        public static IMessageFormatter DebugFormatter => new MessageFormatter((message) => $"DEBUG: {message}");
    }
}