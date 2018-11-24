using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace NerdyMishka
{
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

                if(IsWindows())
                {
                    // dotnet core's Environment.OsVersion returns 6x for windows
                    // ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯
                    var v = GetOsVersion();
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
                            color = 2;
                            return (ColorSupport)color;
                        }
                    }

                    foreach(var s in match)
                    {
                        if(term == s)
                        {
                            color = 2;
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

    

            if(!IsWindows())
            {
                s_isStdOutSet = true;
                return true;
            }

            // dotnet core's Environment.OsVersion returns 6x for windows
            // ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯ ¯\_(ツ)_/¯

            var v = GetOsVersion();
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
              
            return EnableWindowsStdOut();
        }

        private static bool IsWindows()
        {
#if NETSTANDARD2_0
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else 
            var plat = Environment.OSVersion.Platform;
            if(plat == PlatformID.Win32NT || 
                plat == PlatformID.Win32S ||
                plat == PlatformID.Win32Windows) {
                    return true;
            }

            return false;
#endif

        }

        private static Version GetOsVersion()
        {
            #if NETSTANDARD2_0
                var parts = RuntimeInformation.OSDescription.Trim().Split(' ');
                return new Version(parts[parts.Length -1]);
            #else 
                return Environment.OSVersion.Version;
            #endif 

        }


        public static bool EnableVirtualTerminalStdIn()
        {
            if(s_isStdInSet.HasValue)
                return s_isStdInSet.Value;


            if(!IsWindows())
            {
                s_isStdInSet = true;
                return true;
            }

            var v = GetOsVersion();
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
              
            return EnableWindowsStdIn();
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

       
    }
}