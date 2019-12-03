using System.Runtime.InteropServices;
using System;

namespace NerdyMishka
{
    public partial class ChalkConsole
    {
        

        protected static bool EnableWindowsStdOut()
        {
            var stdOut = GetStdHandle(StdOutputHandle);
            
            s_isStdOutSet = GetConsoleMode(stdOut, out uint outConsoleMode) &&
                SetConsoleMode(stdOut, 
                    outConsoleMode | 
                    (uint)ConsoleModeOutput.EnableVirtualTerminalProcessing | 
                    (uint)ConsoleModeOutput.DisableNewlineAutoReturn);

            return s_isStdOutSet.Value;
        }

        protected static bool EnableWindowsStdIn()
        {

            var stdIn = GetStdHandle(StdInputHandle);
            s_isStdInSet = GetConsoleMode(stdIn, out uint intConsoleMode) &&
                SetConsoleMode(stdIn, 
                    intConsoleMode | 
                    (uint)ConsoleModeInput.EnableVirtualTerminalInput);

            return s_isStdInSet.Value;
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
        internal static extern uint GetLastError();


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

         // https://docs.microsoft.com/en-us/windows/console/setconsolemode
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