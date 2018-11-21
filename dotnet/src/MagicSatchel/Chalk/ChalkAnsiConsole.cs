using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace NerdyMishka
{
    
    public enum ColorSupport
    {
        None = 0,
        Basic = 1,
        Medium256 = 2,

        Full = 3
    }
    public class ChalkAnsiConsole
    {
        private static bool? s_isSet = null;

        private const int StdOutputHandle = -11;
        private const uint EnableVirtualTerminalProcessing = 0x0004;
        private const uint DisableNewlineAutoReturn = 0x0008;

        public static bool Force { get; set; } = false;

        private static int? color = null;

        public static ColorSupport ColorSupport 
        {
            get{
                if(color.HasValue)
                    return (ColorSupport)color.Value;

                var emu = Environment.GetEnvironmentVariable("ConEmuANSI");
                if(!string.IsNullOrWhiteSpace(emu))
                {
                    if(emu.ToLower() == "on")
                    {
                        color = 2;
                        return (ColorSupport)color;
                    }
                }

                var varColor = Environment.GetEnvironmentVariable("color");
                if(!string.IsNullOrWhiteSpace(varColor))
                {
                    var trueColors = new string[] {"16m", "full", "truecolor"};
                    if(trueColors.Contains(varColor))
                    {
                        color = (int)ColorSupport.Full;
                        return ColorSupport.Full;
                    }

                    if(varColor == "256")
                    {
                        color = (int)ColorSupport.Medium256;
                        return ColorSupport.Medium256;
                    }
                }

                if(!Force && (Console.IsOutputRedirected || Console.IsErrorRedirected))
                {
                    color = 0;
                    return ColorSupport.None;
                }

                int min = Force ? 1 : 0;
                
                // if not forced and not enabled which means on non-ansi version of windows..
                if(!Force && !Enable())
                {
                    color = 0;
                    return ColorSupport.None;
                }

                var ciVars = new string[] { "TRAVIS", "CIRCLECI", "APPVEYOR", "GITLAB_CI", "TF_BUILD" };

                foreach(var ci in ciVars)
                {
                    var varCiValue = Environment.GetEnvironmentVariable(ci);
                    if(!String.IsNullOrWhiteSpace(varCiValue))
                    {
                        color = 1;
                        return (ColorSupport)color;
                    }
                }

                var colorTerm = Environment.GetEnvironmentVariable("COLORTERM");
                if(colorTerm != null && colorTerm == "truecolor")
                {
                    color = 3;
                    return (ColorSupport)color;
                }

                var termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM");
                if(!string.IsNullOrWhiteSpace(termProgram))
                {
                    var termProgramVer = Environment.GetEnvironmentVariable("TERM_PROGRAM_VERSION") ?? "";
                    var version = 0;
                    int.TryParse(termProgramVer, out version);

                    switch(termProgram)
                    {
                        case "iTerm.app":
                        {
                            if(version >= 3)
                                color = 3;
                            else 
                                color = 2;

                            return (ColorSupport)color;
                        }
                        case "Apple_Terminal":
				        {
                            color = 2;
                            return (ColorSupport)color;
                        }
                        default: 
                            break;

                        
                    }
                }

                var term = Environment.GetEnvironmentVariable("TERM") ?? "";
                if(!string.IsNullOrWhiteSpace(term))
                {
                    term = term.ToLower();
                    var startsWith = new string[] {"screen", "xterm", "vt100", "v220", "rxvt"};
                    var match = new string[] { "color", "ansi", "cygwin", "linux"};
                    foreach(var s in startsWith)
                    {
                        if(term.StartsWith(s))
                        {
                            color = 1;
                            return (ColorSupport)color;
                        }
                    }

                    foreach(var s in match)
                    {
                        if(term == s)
                        {
                            color = 1;
                            return (ColorSupport)color;
                        }
                    }
                }

                if(string.IsNullOrWhiteSpace(colorTerm))
                {
                    color = 1;
                    return (ColorSupport)color;
                }

                color = min;
                return (ColorSupport)color;
            }
        }

        public static bool Enable()
        {
            if(s_isSet.HasValue)
                return s_isSet.Value;


            if(Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                s_isSet = true;
                return true;
            }

            var v = Environment.OSVersion.Version;
            if(v.Major < 10)
            {
                s_isSet = false;
                return false;
            }
                
            

            if(v.Major == 10 && v.Minor == 0 && v.Build <  10586)
            {
                s_isSet = false;
                return false;
            }

            if(!Force && (Console.IsOutputRedirected || Console.IsErrorRedirected))
            {
                s_isSet = false;
                return false;
            }
              

            var iStdOut = GetStdHandle(StdOutputHandle);
            s_isSet = GetConsoleMode(iStdOut, out uint outConsoleMode) &&
                SetConsoleMode(iStdOut, 
                    outConsoleMode | 
                    EnableVirtualTerminalProcessing | 
                    DisableNewlineAutoReturn);

            return s_isSet.Value;
        }



        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}