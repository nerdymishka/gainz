using System.Text;

namespace NerdyMishka
{
    public partial class Chalk
    {
        private static bool s_enabled = false;
        private static ColorSupport s_colorSupport = ColorSupport.None;
        static Chalk()
        {
            s_enabled = ChalkAnsiConsole.Enable();
            s_colorSupport = ChalkAnsiConsole.ColorSupport;
        }

        private static string DrawBright(string value, byte color)
        {
            if(!s_enabled)
                return value;

            var sb = new StringBuilder();
            sb.Append(MarkerStart);
            sb.Append(color);
            sb.Append(BrightMarkerEnd);
            if(value.Contains(Default))
            {
                // TODO: parse and use string builder;
                sb.Append(value.Replace(Default, Default + MarkerStart + color + BrightMarkerEnd));
            } else {
                sb.Append(value);
            }

            sb.Append(Default);
            
            return sb.ToString();
        }

        private static string Draw(string value, byte color)
        {
            if(!s_enabled)
                return value;

            var sb = new StringBuilder();

            // TODO: keep consts around for common colors;
            sb.Append("\u001b[");
            sb.Append(color);
            sb.Append("m");
            if(value.Contains(Default))
            {
                // TODO: parse and use string builder;
                sb.Append(value.Replace(Default,  Default + MarkerStart + color + MarkerEnd));
            } else {
                sb.Append(value);
            }

            sb.Append(Default);
            
            return sb.ToString();
        }
        public static IChalk Black() => new Chalk(Colors.Black);

        public static IChalk Red() =>new Chalk( Colors.Red);

        public static IChalk Green() => new Chalk( Colors.Green);

        public static IChalk Yellow() => new Chalk( Colors.Yellow);

        public static IChalk Blue() => new Chalk( Colors.Blue);

        public static IChalk Cyan() => new Chalk( Colors.Cyan);

        public static IChalk Magenta() => new Chalk( Colors.Magenta);

        public static IChalk White() => new Chalk(Colors.White);


        public static IChalk BrightBlack() => new Chalk(Colors.Black, true);

        public static IChalk BrightRed() => new Chalk(Colors.Red, true);

        public static IChalk BrightGreen() => new Chalk(Colors.Green, true);

        public static IChalk BrightYellow() => new Chalk( Colors.Yellow, true);

        public static IChalk BrightBlue() => new Chalk( Colors.Blue, true);

        public static IChalk BrightCyan() => new Chalk( Colors.Cyan, true);

        public static IChalk BrightMagenta() => new Chalk( Colors.Magenta, true);

        public static IChalk BrightWhite() => new Chalk(Colors.White, true);

        public static IChalk Bold() => new Chalk(Style.Bold);

        public static IChalk Underline() => new Chalk(Style.Bold);

        public static IChalk Reverse() => new Chalk(Style.Bold);

        public static string Black(string value) => Draw(value, Colors.Black);

        public static string Red(string value) => Draw(value, Colors.Red);

        public static string Green(string value) => Draw(value, Colors.Green);

        public static string Yellow(string value) => Draw(value, Colors.Yellow);

        public static string Blue(string value) => Draw(value, Colors.Blue);

        public static string Cyan(string value) => Draw(value, Colors.Cyan);

        public static string Magenta(string value) => Draw(value, Colors.Magenta);

        public static string White(string value) => Draw(value, Colors.White);


        public static string BrightBlack(string value) => DrawBright(value, Colors.Black);

        public static string BrightRed(string value) => DrawBright(value, Colors.Red);

        public static string BrightGreen(string value) => DrawBright(value, Colors.Green);

        public static string BrightYellow(string value) => DrawBright(value, Colors.Yellow);

        public static string BrightBlue(string value) => DrawBright(value, Colors.Blue);

        public static string BrightCyan(string value) => DrawBright(value, Colors.Cyan);

        public static string BrightMagenta(string value) => DrawBright(value, Colors.Magenta);

        public static string BrightWhite(string value) => DrawBright(value, Colors.White);
    }
}