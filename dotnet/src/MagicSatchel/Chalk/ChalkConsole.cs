using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace NerdyMishka
{
    
    public enum ColorSupport
    {
        None = 0,
        Ansi16 = 1,
        Ansi256 = 2,
        TrueColor = 3
    }

   

    public partial class ChalkConsole
    {
        private static bool? s_isStdOutSet = null;
        private static bool? s_isStdInSet = null;
        private const int StdOutputHandle = -11;
        private const int StdInputHandle = -10;
        private const int StdErrorHandle = -12;

        private static readonly string[] s_Map =
            Enumerable.Range(0, 256).Select(s => s.ToString()).ToArray();

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
                        color = 3;
                        return ColorSupport.TrueColor;
                    }

                    if(varColor == "256")
                    {
                        color = (int)ColorSupport.Ansi256;
                        return ColorSupport.Ansi256;
                    }
                }

                if(!Force && (Console.IsOutputRedirected || Console.IsErrorRedirected))
                {
                    color = 0;
                    return ColorSupport.None;
                }

                int min = Force ? 1 : 0;
                
                // if not forced and not enabled which means on non-ansi version of windows..
                if(!Force && !EnableVirtualTerminalStdOut())
                {
                    color = 0;
                    return ColorSupport.None;
                }

                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // dotnet core's Environment.OsVersion returns 6x for windows
                    // ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯
                    var parts = RuntimeInformation.OSDescription.Trim().Split(' ');
                    var v = new Version(parts[parts.Length -1]);
                    if(v.Major > 10 || v.Major == 10 && v.Minor > 0)
                    {
                        color = 3;
                        return ColorSupport.TrueColor;
                    }
                    if(v.Major == 10 && v.Build >= 10586)
                    {
                        color = 3;
                        return ColorSupport.TrueColor;
                    }
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


     

        public static bool EnableVirtualTerminalStdOut()
        {
            if(s_isStdOutSet.HasValue)
                return s_isStdOutSet.Value;

    

            if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                
               s_isStdOutSet = true;
                return true;
            }

            // dotnet core's Environment.OsVersion returns 6x for windows
            // ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯

            var parts = RuntimeInformation.OSDescription.Trim().Split(' ');
            var v = new Version(parts[parts.Length -1]);
            if(v.Major < 10)
            {
                s_isStdOutSet = false;
                return false;
            }
                
            

            if(v.Major == 10 && v.Minor == 0 && v.Build <  10586)
            {
                s_isStdOutSet = false;
                return false;
            }

            if(!Force && (Console.IsOutputRedirected || Console.IsErrorRedirected))
            {
                s_isStdOutSet = false;
                return false;
            }
              
            var stdOutHandle = GetStdHandle(StdOutputHandle);
            var stdInHandle = GetStdHandle(StdInputHandle);

            var iStdOut = GetStdHandle(StdOutputHandle);
            s_isStdOutSet = GetConsoleMode(iStdOut, out uint outConsoleMode) &&
                SetConsoleMode(iStdOut, 
                    outConsoleMode | 
                    (uint)ConsoleModeOutput.EnableVirtualTerminalProcessing | 
                    (uint)ConsoleModeOutput.DisableNewlineAutoReturn);

            return s_isStdOutSet.Value;
        }


        public static bool EnableVirtualTerminalStdIn()
        {
            if(s_isStdInSet.HasValue)
                return s_isStdInSet.Value;


            if(Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                s_isStdInSet = true;
                return true;
            }

            var v = Environment.OSVersion.Version;
            if(v.Major < 10)
            {
                s_isStdInSet = false;
                return false;
            }
                
            

            if(v.Major == 10 && v.Minor == 0 && v.Build <  10586)
            {
                s_isStdInSet = false;
                return false;
            }

            if(!Force && (Console.IsOutputRedirected || Console.IsErrorRedirected))
            {
                s_isStdInSet = false;
                return false;
            }
              
            var stdOutHandle = GetStdHandle(StdOutputHandle);
            var stdInHandle = GetStdHandle(StdInputHandle);

            var stdIn = GetStdHandle(StdInputHandle);
            s_isStdInSet = GetConsoleMode(stdIn, out uint outConsoleMode) &&
                SetConsoleMode(stdIn, 
                    outConsoleMode | 
                    (uint)ConsoleModeInput.EnableVirtualTerminalInput);

            return s_isStdInSet.Value;
        }
        
        private static ConsoleModeOutput GetConsoleModeOutput(IntPtr stdOutHandle)
        {
            if(GetConsoleMode(stdOutHandle, out uint lpMode))
            {
                return (ConsoleModeOutput)lpMode;
            }

            return ConsoleModeOutput.Unknown;
        }

        private static ConsoleModeInput GetConsoleModeInput(IntPtr stdInHandle)
        {
             if(GetConsoleMode(stdInHandle, out uint lpMode))
            {
                return (ConsoleModeInput)lpMode;
            }

            return ConsoleModeInput.Unknown;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
         [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();


        // https://docs.microsoft.com/en-us/windows/console/setconsolemode
        [Flags]
        internal enum ConsoleModeOutput : uint
        {
            Unknown = 0x0,
            EnableProcessedOutput = 0x1,
            EnableWrapAtEolOutput = 0x2,
            EnableVirtualTerminalProcessing = 0x4,
            DisableNewlineAutoReturn = 0x8,
            EnableLvbGridWorldwide = 0x10
        }

        internal enum ConsoleModeInput : uint 
        {
            Unknown = 0x0,

            EnableProcessedInput = 0x1,
            EnableLineInput = 0x2,
            EnableEchoInput = 0x4,
            EnableWindowInput = 0x8,
            EnableMouseInput = 0x10,
            EnableInsertMode = 0x20,
            EnableQuickEditMode = 0x40,
            EnableExtendedFlags = 0x80,
            EnableAutoPosition = 0x100,
            EnableVirtualTerminalInput = 0x200

        }
    }
}