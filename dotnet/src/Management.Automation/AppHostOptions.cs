namespace NerdyMishka.Management.Automation
{
    public class AppHostOptions
    {
        public int TimeOut { get; set; }

        public bool Interactive { get; set; }

        public string ApplicationName { get; set; }

        public IMessageFormatter ErrorFormatter { get; set; } = MessageFormatter.ErrorFormatter;

        public IMessageFormatter WarningFormatter { get; set; } = MessageFormatter.WarningFormatter;

        public IMessageFormatter VerboseFormatter { get; set; } = MessageFormatter.VerboseFormatter;

        public IMessageFormatter DebugFormatter { get; set; } = MessageFormatter.DebugFormatter;
    }
}