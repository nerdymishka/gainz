using System;
using System.Drawing;

namespace NerdyMishka
{
    public partial class ChalkConsole
    {
        
        public static void Write(string value, Color color)
        {
            Console.Write(Chalk.Color(value, color));
        }

        public static void Write(decimal value, params int[] codes)
        {
            Write(value.ToString(), codes);
        }

        public static void Write(bool value, params int[] codes)
        {
            Write(value == true ? bool.TrueString : bool.FalseString, codes);
        }

        public static void Write(int value, params int[] codes)
        {
            Write(value.ToString(), codes);
        }

        public static void Write(long value, params int[] codes)
        {
            Write(value.ToString(), codes);
        }

        public static void Write(float value, params int[] codes)
        {
            Write(value.ToString(), codes);
        }

        public static void Write(Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
        }

        public static void Write(string value, params int[] codes)
        {
            Console.Write(Chalk.Styles(value, codes));
        }

        public static void Write(string value, params AnsiCodes[] codes)
        {
            Console.Write(Chalk.Styles(value, codes));
        }

        public static void WriteLine(string value, Color color)
        {
            Console.WriteLine(Chalk.Color(value, color));
        }


        public static void WriteLine(string value, params int[] codes)
        {
            Console.WriteLine(Chalk.Styles(value, codes));
        }

        public static void WriteLine(string value, params AnsiCodes[] codes)
        {
            Console.WriteLine(Chalk.Styles(value, codes));
        }

       
    }
}