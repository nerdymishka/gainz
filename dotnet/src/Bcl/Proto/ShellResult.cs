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

        public override string ToString()
        {
            var hasErr = this.StdError != null && this.StdError.Count > 0;
            var hasOut = this.StdOut != null && this.StdOut.Count > 0;
            if(hasOut || hasErr)
            {
                var _out = "";
                if(hasOut)
                    _out += string.Join(System.Environment.NewLine, this.StdOut);

                if(hasErr)
                    _out += string.Join(System.Environment.NewLine, this.StdError);

                return _out;
            }

            return string.Empty;
        }
    }
}

