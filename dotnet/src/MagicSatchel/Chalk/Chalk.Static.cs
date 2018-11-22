using System.Drawing;
using System.Text;

namespace NerdyMishka
{
    public partial class Chalk
    {
        private static bool s_enabled = false;
        private static ColorSupport s_colorSupport = ColorSupport.None;
        static Chalk()
        {
            s_enabled = ChalkConsole.EnableVirtualTerminalStdOut();
            s_colorSupport = ChalkConsole.ColorSupport;

            if(!s_enabled) {
                throw new System.Exception("vt is not enabled");
            }

            if(s_colorSupport == ColorSupport.Ansi256)
            {
                throw new System.Exception("color support is none");
            }
        }


        public static IChalk BgBlack() => new Chalk(AnsiCodes.BgBlack);

        public static IChalk BgRed() => new Chalk(AnsiCodes.BgRed);

        public static IChalk BgGreen() => new Chalk(AnsiCodes.BgGreen);

        public static IChalk BgYellow() => new Chalk(AnsiCodes.BgYellow);

        public static IChalk BgBlue() => new Chalk(AnsiCodes.BgBlue);

        public static IChalk BgMagenta() => new Chalk(AnsiCodes.BgMagenta);

        public static IChalk BgCyan() => new Chalk(AnsiCodes.BgCyan);
        
        public static IChalk BgWhite() => new Chalk(AnsiCodes.BgWhite);

        public static IChalk Black() => new Chalk(AnsiCodes.Black);

        public static IChalk Red() => new Chalk(AnsiCodes.Red);

        public static IChalk Green() => new Chalk(AnsiCodes.Green);

        public static IChalk Yellow() => new Chalk(AnsiCodes.Yellow);

        public static IChalk Blue() => new Chalk(AnsiCodes.Blue);

        public static IChalk Magenta() => new Chalk(AnsiCodes.Magenta);

        public static IChalk Cyan() => new Chalk(AnsiCodes.Cyan);
        public static IChalk White() => new Chalk(AnsiCodes.White);

        public static IChalk BrightBlack() => new Chalk(AnsiCodes.BrightBlack);

        public static IChalk BrightRed() => new Chalk(AnsiCodes.BrightRed);

        public static IChalk BrightGreen() => new Chalk(AnsiCodes.BrightGreen);

        public static IChalk BrightYellow() => new Chalk(AnsiCodes.BrightYellow);

        public static IChalk BrightBlue() => new Chalk(AnsiCodes.BrightBlue);

        public static IChalk BrightMagenta() => new Chalk(AnsiCodes.BrightMagenta);

        public static IChalk BrightCyan() => new Chalk(AnsiCodes.BrightCyan);
        public static IChalk BrightWhite() => new Chalk(AnsiCodes.BrightWhite);

        public static IChalk Bold() => new Chalk(AnsiCodes.Bold);

        public static IChalk Underline() => new Chalk(AnsiCodes.Underline);

        public static IChalk Reverse() => new Chalk(AnsiCodes.Reverse);

        public static IChalk StrikeOut() => new Chalk(AnsiCodes.StrikeOut);

        public static IChalk Hide() => new Chalk(AnsiCodes.Hidden);

        public static IChalk Italicize() => new Chalk(AnsiCodes.Italicize);

        public static IChalk Dim() => new Chalk(AnsiCodes.Dim);

        
        public static IChalk Grey() => new Chalk(AnsiCodes.BrightBlack);

        
        public static IChalk BgGrey() => new Chalk(AnsiCodes.BrightBgBlack);
        

        public static IChalk Rgb(Rgb rgb, bool isBgColor = false) => new Chalk(rgb, isBgColor);

        public static IChalk Rgb(int r, int g, int b, bool isBgColor = false) => 
            new Chalk(new Rgb() {
                Red =r,
                Green = g,
                Blue = b
            }, isBgColor);


        public static IChalk Color(Color color, bool isBgColor = false) => 
            new Chalk(color, isBgColor);


        public static IChalk Color(string color, bool isBgColor = false) => 
            new Chalk(System.Drawing.Color.FromName(color), isBgColor);

         public static IChalk Reset() => new Chalk(AnsiCodes.Default);

         public static IChalk Blink() => new Chalk(AnsiCodes.Glitch);


        public static string Black(string value) => new Chalk(AnsiCodes.Black).Draw(value);

        public static string Red(string value) => new Chalk(AnsiCodes.Red).Draw(value);

        public static string Green(string value) => new Chalk(AnsiCodes.Green).Draw(value);

        public static string Yellow(string value) => new Chalk(AnsiCodes.Yellow).Draw(value);

        public static string Blue(string value) => new Chalk(AnsiCodes.Blue).Draw(value);

        public static string Cyan(string value) => new Chalk(AnsiCodes.Cyan).Draw(value);

        public static string Magenta(string value) => new Chalk(AnsiCodes.Magenta).Draw(value);

        public static string White(string value) => new Chalk(AnsiCodes.White).Draw(value);

        public static string BrightBlack(string value) => new Chalk(AnsiCodes.BrightBlack).Draw(value);

        public static string BrightRed(string value) => new Chalk(AnsiCodes.BrightRed).Draw(value);

        public static string BrightGreen(string value) => new Chalk(AnsiCodes.BrightGreen).Draw(value);

        public static string BrightYellow(string value) => new Chalk(AnsiCodes.BrightYellow).Draw(value);

        public static string BrightBlue(string value) => new Chalk(AnsiCodes.BrightBlue).Draw(value);

        public static string BrightCyan(string value) => new Chalk(AnsiCodes.BrightCyan).Draw(value);

        public static string BrightMagenta(string value) =>new Chalk(AnsiCodes.Magenta).Draw(value);

        public static string BrightWhite(string value) => new Chalk(AnsiCodes.White).Draw(value);


        public static string BgBlack(string value) => new Chalk(AnsiCodes.BgBlack).Draw(value);

        public static string BgRed(string value) => new Chalk(AnsiCodes.BgRed).Draw(value);

        public static string BgGreen(string value) => new Chalk(AnsiCodes.BgGreen).Draw(value);

        public static string BgYellow(string value) => new Chalk(AnsiCodes.BgYellow).Draw(value);

        public static string BgBlue(string value) => new Chalk(AnsiCodes.BgBlue).Draw(value);

        public static string BgCyan(string value) => new Chalk(AnsiCodes.BgCyan).Draw(value);

        public static string BgMagenta(string value) => new Chalk(AnsiCodes.BgMagenta).Draw(value);

        public static string BgWhite(string value) => new Chalk(AnsiCodes.BgWhite).Draw(value);

        public static string BrightBgBlack(string value) => new Chalk(AnsiCodes.BrightBgBlack).Draw(value);

        public static string BrightBgRed(string value) => new Chalk(AnsiCodes.BrightBgRed).Draw(value);

        public static string BrightBgGreen(string value) => new Chalk(AnsiCodes.BrightBgGreen).Draw(value);

        public static string BrightBgYellow(string value) => new Chalk(AnsiCodes.BrightBgYellow).Draw(value);

        public static string BrightBgBlue(string value) => new Chalk(AnsiCodes.BrightBgBlue).Draw(value);

        public static string BrightBgCyan(string value) => new Chalk(AnsiCodes.BrightBgCyan).Draw(value);

        public static string BrightBgMagenta(string value) =>new Chalk(AnsiCodes.BrightBgMagenta).Draw(value);

        public static string BrightBgWhite(string value) => new Chalk(AnsiCodes.BrightBgWhite).Draw(value);

        public static string Bold(string value) => new Chalk(AnsiCodes.Bold).Draw(value);

        public static string Underline(string value) =>  new Chalk(AnsiCodes.Underline).Draw(value);

        public static string Reverse(string value) =>  new Chalk(AnsiCodes.Reverse).Draw(value);

        public static string StrikeOut(string value) =>  new Chalk(AnsiCodes.StrikeOut).Draw(value);

        public static string Hide(string value) =>  new Chalk(AnsiCodes.Hidden).Draw(value);

        public static string Italicize(string value) =>  new Chalk(AnsiCodes.Italicize).Draw(value);

        public static string Dim(string value) =>  new Chalk(AnsiCodes.Dim).Draw(value);

       

        
        public static string Grey(string value) =>  new Chalk(AnsiCodes.BrightBlack).Draw(value);

        
        public static string BgGrey(string value) =>  new Chalk(AnsiCodes.BrightBgBlack).Draw(value);
        

        public static string Rgb(string value, Rgb rgb, bool isBgColor = false) => new Chalk(rgb, isBgColor).Draw(value);

        public static string Rgb(string value, int r, int g, int b, bool isBgColor = false) => 
            new Chalk(new Rgb() {
                Red =r,
                Green = g,
                Blue = b
            }, isBgColor).Draw(value);


        public static string Color(string value, Color color, bool isBgColor = false) => 
            new Chalk(color, isBgColor).Draw(value);


        public static string Color(string value, string color, bool isBgColor = false) => 
            new Chalk(System.Drawing.Color.FromName(color), isBgColor).Draw(value);

        public static string Styles(string value, params int[] codes) => new Chalk(codes).Draw(value);

        public static string Styles(string value, params AnsiCodes[] codes) => new Chalk(codes).Draw(value);

         public static string Reset(string value) =>  new Chalk(AnsiCodes.Default).Draw(value);
    }
}