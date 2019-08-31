using System.Collections.Generic;

namespace NerdyMishka
{
    public class ShellResult
    {
        public int ExitCode { get; set; }

        public IList<string> StdOut { get; set; }

        public IList<string> StdError { get; set; }

        public bool TimeoutExpired { get; set; }

        public int Timeout { get; set; }
    }
}

