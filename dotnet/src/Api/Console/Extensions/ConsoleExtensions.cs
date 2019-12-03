
using System;

namespace NerdyMishka.Extensions
{
    public static class ConsoleExtensions
    {
        
        [System.CLSCompliant(false)]
        public static void Write(this IConsole console, ulong value)
        {
            console.Out.Write(value);
        }

        public static void Write(this IConsole console, long value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, string format, object arg0, object arg1)
        {
            console.Out.Write(format, arg0, arg1);
        }
        public static void Write(this IConsole console, int value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, string format, object arg0)
        {
            console.Out.Write(format, arg0);
        }

        [CLSCompliant(false)]
        public static void Write(this IConsole console, uint value)
        {
            console.Out.Write(value);
        }


        [CLSCompliant(false)]
        public static void Write(this IConsole console, string format, object arg0, object arg1, object arg2, object arg3)
        {
            console.Out.Write(format, arg0, arg1, arg2, arg3);
        }

        public static void Write(this IConsole console, string format, params object[] arg)
        {
            console.Out.Write(format, arg);
        }

        public static void Write(this IConsole console, bool value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, char value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, char[] buffer)
        {
            console.Out.Write(buffer);
        }
        public static void Write(this IConsole console, char[] buffer, int index, int count)
        {
            console.Out.Write(buffer, index, count);
        }

        public static void Write(this IConsole console, string format, object arg0, object arg1, object arg2)
        {
            console.Out.Write(format, arg0, arg1, arg2);
        }
        public static void Write(this IConsole console, decimal value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, float value)
        {
            console.Out.Write(value);
        }
        public static void Write(this IConsole console, double value)
        {
            console.Out.Write(value);
        }




        [System.CLSCompliant(false)]
        public static void WriteLine(this IConsole console, ulong value)
        {
            console.Out.WriteLine(value);
        }

        public static void WriteLine(this IConsole console, long value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, string format, object arg0, object arg1)
        {
            console.Out.WriteLine(format, arg0, arg1);
        }
        public static void WriteLine(this IConsole console, int value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, string format, object arg0)
        {
            console.Out.WriteLine(format, arg0);
        }

        [CLSCompliant(false)]
        public static void WriteLine(this IConsole console, uint value)
        {
            console.Out.WriteLine(value);
        }


        [CLSCompliant(false)]
        public static void WriteLine(this IConsole console, string format, object arg0, object arg1, object arg2, object arg3)
        {
            console.Out.WriteLine(format, arg0, arg1, arg2, arg3);
        }

        public static void WriteLine(this IConsole console, string format, params object[] arg)
        {
            console.Out.WriteLine(format, arg);
        }

        public static void WriteLine(this IConsole console, bool value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, char value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, char[] buffer)
        {
            console.Out.WriteLine(buffer);
        }
        public static void WriteLine(this IConsole console, char[] buffer, int index, int count)
        {
            console.Out.WriteLine(buffer, index, count);
        }

        public static void WriteLine(this IConsole console, string format, object arg0, object arg1, object arg2)
        {
            console.Out.WriteLine(format, arg0, arg1, arg2);
        }
        public static void WriteLine(this IConsole console, decimal value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, float value)
        {
            console.Out.WriteLine(value);
        }
        public static void WriteLine(this IConsole console, double value)
        {
            console.Out.WriteLine(value);
        }
    }
}