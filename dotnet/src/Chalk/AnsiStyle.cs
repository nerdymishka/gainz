using System.Drawing;
using System.Linq;

namespace NerdyMishka
{

    public enum AnsiKind
    {
        Code,
        Rgb
    }

    public abstract class AnsiStyle
    {
        public abstract AnsiKind Kind { get; }

        public abstract object GetValue();

        public abstract string Reverse();
    }

    public class AnsiCode : AnsiStyle
    {
        public AnsiCode(int code)
        {
            this.Code = code;
        }

        public AnsiCode(AnsiCodes code)
        {
            this.Code = (int)code;
        }
        public override AnsiKind Kind => AnsiKind.Code;

        public int Code { get; set; }

        public override object GetValue() => this.Code;

        public override string ToString()
        {
            if(!ChalkConsole.EnableVirtualTerminalStdOut())
                return string.Empty;

            return this.Code.ToString();
        }

        public override string Reverse() {
            if(!ChalkConsole.EnableVirtualTerminalStdOut())
                return string.Empty;
            
            return AnsiCodeMap.Reverse(this.Code);
        }
    }

    public class AnsiRgbColor : AnsiStyle
    {
        private static readonly string[] s_Map =
            Enumerable.Range(0, 256).Select(s => s.ToString()).ToArray();

        public AnsiRgbColor(Rgb rgb, bool isBgColor = false)
        {
            this.Rgb = rgb;
            this.IsBgColor = isBgColor;
        }

        public AnsiRgbColor(Color color, bool isBgColor = false)
        {
            this.Rgb = color.ToRgb();
            this.IsBgColor = isBgColor;
        }

        public AnsiRgbColor(int r, int g, int b, bool isBgColor = false)
        {
            this.Rgb = new Rgb(){
                Red = r,
                Green = g,
                Blue = b
            };
            this.IsBgColor = isBgColor;
        }

        public override AnsiKind Kind => AnsiKind.Rgb;

        public Rgb Rgb { get; private set; }

        public bool IsBgColor { get; private set;} = false;

        public override object GetValue() => this.Rgb;

        public override string ToString()
        {
            return EmitAnsiColor(this.Rgb, this.IsBgColor);
        }


        public override string Reverse() {
            switch(ChalkConsole.ColorSupport)
            {
                case ColorSupport.TrueColor:
                case ColorSupport.Ansi16:
                case ColorSupport.Ansi256:
                    if(this.IsBgColor)
                        return AnsiCodeMap.Reverse(48);
                    return AnsiCodeMap.Reverse(38);
                default: 
                    return string.Empty;
            }
        }


        public static string EmitAnsiColor(Rgb rgb, bool isBgColor = false)
        {
            switch(ChalkConsole.ColorSupport)
            {
                case ColorSupport.TrueColor:
                    return EmitTrueColor(rgb, isBgColor);

                case ColorSupport.Ansi256:
                    return EmitAnsi256(rgb, isBgColor);

                case ColorSupport.Ansi16:
                    return rgb.ToAnsi256().ToString();

                case ColorSupport.None:
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }

        public static string EmitTrueColor(Rgb rgb, bool isBgColor = false)
        {
            return string.Concat(
                isBgColor ? "48;" : "38;",
                "2;",
                s_Map[rgb.Red], ";",
                s_Map[rgb.Green], ";",
                s_Map[rgb.Blue] 
            );
        }

        public static string EmitAnsi256(Rgb rgb, bool isBgColor = false)
        {
            return string.Concat(
                isBgColor ? "48;" : "38;",
                "5;",
                rgb.ToAnsi256() 
            );
        }
    }

}