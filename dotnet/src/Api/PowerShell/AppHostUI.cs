using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Text;
using Microsoft.Extensions.Logging;

namespace NerdyMishka.Management.Automation
{
    public class AppHostUI : PSHostUserInterface
    {
        private ILogger logger;
        private AppHostRawUI rawUI;

        private AppHostOptions options;

        private IConsole console;

        public AppHostUI(IConsole console, ILogger logger, AppHostOptions options = null)
        {

            if(console == null)
                throw new ArgumentNullException(nameof(console));

            this.Options = options ?? new AppHostOptions();

            this.rawUI = new AppHostRawUI(new Lazy<IConsole>(() => console), logger);
        }

        public AppHostOptions Options { get; private set; }

        public override PSHostRawUserInterface RawUI => throw new NotImplementedException();

        public bool HasStandardErrorData { get; private set; }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions)
        {
            var results = new Dictionary<string, PSObject>();
            foreach (FieldDescription field in descriptions)
            {
                var label = GetLabel(field.Label).Item2 ?? field.Name;

                this.Write(label);

                dynamic selection = string.Empty;

                if (field.ParameterTypeFullName == typeof(SecureString).FullName)
                {
                    selection = ReadLineAsSecureString();
                }
                else
                {
                    selection = ReadLine();
                }
                
                if (selection == null) 
                    return null;

                results[field.Name] = PSObject.AsPSObject(selection);
            }

            return results;
        }

        private static Tuple<string, string> GetLabel(string input)
        {
            if(string.IsNullOrEmpty(input))
                return new Tuple<string, string>(string.Empty, string.Empty);

            string key = string.Empty, label = input;
            //Do not use StringSplitOptions.RemoveEmptyEntries, it causes issues here
            string[] pair = input.Split('&');
            if (pair.Length == 2)
            {
                if (pair[1].Length > 0)
                {
                   key = pair[1][0].ToString().ToUpperInvariant();
                } 
                    
                
                label = (pair[0] + pair[1]).Trim();
            }
            

            return new Tuple<string,string>(key, label);
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            if (!string.IsNullOrWhiteSpace(caption)) 
                this.WriteVerboseLine(caption);

            if (!string.IsNullOrWhiteSpace(message)) 
                this.WriteVerboseLine(message);

            var set = BuildLabels(choices);

            // Format the overall choice prompt string to display.
            var prompt = new StringBuilder();
            for (int j = 0; j < choices.Count; j++)
            {
                var label = set[j];
                prompt.Append(String.Format(
                    CultureInfo.CurrentCulture,
                    "[{0}] {1} ",
                    label.Item1,
                    label.Item2));
            }

            prompt.Append(String.Format(
                CultureInfo.CurrentCulture,
                "(default is \"{0}\")",
                set[defaultChoice].Item2));

            while (true)
            {
                string selection = ReadLine();
                var key = selection.Trim()[0].ToString().ToUpperInvariant();

                if (selection == null || selection.Length == 0) 
                    return defaultChoice;

                for (int i = 0; i < choices.Count; i++)
                {
                    if (set[i].Item1 == key || set[i].Item2 == selection) 
                        return i;
                }

                this.logger?.LogWarning($"Invalid Choice: {selection}");
            }
        }

        private static List<Tuple<string, string>> BuildLabels(Collection<ChoiceDescription> choices)
        {
            var set = new List<Tuple<string, string>>();

            for (int i = 0; i < choices.Count; ++i)
            {
                var label = GetLabel(choices[i].Label);
                set.Add(label);
            }

            return set;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            return this.PromptForCredential(caption, message, userName, targetName, PSCredentialTypes.Default, PSCredentialUIOptions.Default);
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName, PSCredentialTypes allowedCredentialTypes, PSCredentialUIOptions options)
        {
           if (!string.IsNullOrWhiteSpace(caption)) 
                this.WriteVerboseLine(caption);

            if (!string.IsNullOrWhiteSpace(message)) 
                this.WriteVerboseLine(message);

            if (string.IsNullOrWhiteSpace(userName))
            {
                this.WriteLine("Username:");
                string selection = ReadLine();

                if (selection.Length == 0) 
                    selection = targetName;

                if (!string.IsNullOrWhiteSpace(selection)) 
                    userName = selection;
            }

            SecureString ss = null;
            this.WriteLine("Password:");
            ss = this.ReadLineAsSecureString();

            if (string.IsNullOrWhiteSpace(userName) || ss == null)
            {
                this.WriteWarningLine("Missing username or password. This may create issues");
            }

            return new PSCredential(userName, ss);
        }

        public override string ReadLine()
        {
            if(!options.Interactive)
                return null;

            return this.console.ReadLine();
        }

        public override SecureString ReadLineAsSecureString()
        {
            var ss = new SecureString();
            if(!options.Interactive)
                return ss;

            var key = this.console.ReadKey(true);
            var chars = new List<char>();
            while(key.Key != ConsoleKey.Enter)
            {
                if(key.Key != ConsoleKey.Backspace)
                {
                    Console.Write("*");
                    chars.Add(key.KeyChar);
                    key = Console.ReadKey(true);
                    continue;
                }
                if(key.Key == ConsoleKey.Backspace)
                {
                    if(chars.Count > 0)
                    {
                        chars.RemoveAt(chars.Count - 1);
                        int pos = this.console.CursorLeft;
                        this.console.SetCursorPosition(pos -1, this.console.CursorTop);
                        this.console.Write(" ");
                        this.console.SetCursorPosition(pos -1, this.console.CursorTop);
                    }
                    key = Console.ReadKey(true);
                    continue;
                }
            }

            foreach(var c in chars)
                ss.AppendChar(c);

            chars.Clear();
            chars.TrimExcess();
            chars = null;

            return ss;
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value)
        {
            var fgColor = this.console.ForegroundColor;
            var bgColor = this.console.BackgroundColor;
            this.console.ForegroundColor = foregroundColor;
            this.console.BackgroundColor = backgroundColor;

            this.console.Write(value);
            this.logger?.LogInformation(value);

            this.console.ForegroundColor = fgColor;
            this.console.BackgroundColor = bgColor;
        }

        public override void Write(string value)
        {
            this.console.Write(value);
            this.logger?.LogInformation(value);
        }

        public override void WriteDebugLine(string message)
        {
            this.console.WriteLine(this.options.DebugFormatter.Format(message));
        }

        public override void WriteErrorLine(string value)
        {
            this.HasStandardErrorData = true;
            this.console.Error.WriteLine(this.options.ErrorFormatter.Format(value));
            this.logger?.LogError(value);
        }

        public override void WriteLine(string value)
        {
            this.console.WriteLine(value);
            this.logger?.LogInformation(value);
        }

        public override void WriteProgress(long sourceId, ProgressRecord record)
        {
            this.console.Write($"\rProgress: {record.PercentComplete}% - {record.StatusDescription}".PadRight(Console.WindowWidth));
        }

        public override void WriteVerboseLine(string message)
        {
            this.console.WriteLine(this.options.VerboseFormatter.Format(message));
            this.logger?.LogTrace(message);
        }

        public override void WriteWarningLine(string message)
        {
            this.console.WriteLine(this.options.WarningFormatter.Format(message));
            this.logger?.LogWarning(message);
        }
    }
}