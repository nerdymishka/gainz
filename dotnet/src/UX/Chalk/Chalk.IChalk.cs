using System.Drawing;

namespace NerdyMishka
{
    public partial class Chalk: IChalk
    {
        
        IChalk IChalk.BgBlack() => this.Chain(AnsiCodes.BgBlack);

        IChalk IChalk.BgRed() => this.Chain(AnsiCodes.BgRed);

        IChalk IChalk.BgGreen() => this.Chain(AnsiCodes.BgGreen);

        IChalk IChalk.BgYellow() => this.Chain(AnsiCodes.BgYellow);

        IChalk IChalk.BgBlue() => this.Chain(AnsiCodes.BgBlue);

        IChalk IChalk.BgMagenta() => this.Chain(AnsiCodes.BgMagenta);

        IChalk IChalk.BgCyan() => this.Chain(AnsiCodes.BgCyan);

        IChalk IChalk.BgWhite() => this.Chain(AnsiCodes.BgWhite);

        IChalk IChalk.Black() => this.Chain(AnsiCodes.Black);

        IChalk IChalk.Red() => this.Chain(AnsiCodes.Red);

        IChalk IChalk.Green() => this.Chain(AnsiCodes.Green);

        IChalk IChalk.Yellow() => this.Chain(AnsiCodes.Yellow);

        IChalk IChalk.Blue() => this.Chain(AnsiCodes.Blue);

        IChalk IChalk.Magenta() => this.Chain(AnsiCodes.Magenta);

        IChalk IChalk.Cyan() => this.Chain(AnsiCodes.Cyan);
        IChalk IChalk.White() => this.Chain(AnsiCodes.White);

        IChalk IChalk.BrightBlack() => this.Chain(AnsiCodes.BrightBlack);

        IChalk IChalk.BrightRed() => this.Chain(AnsiCodes.BrightRed);

        IChalk IChalk.BrightGreen() => this.Chain(AnsiCodes.BrightGreen);

        IChalk IChalk.BrightYellow() => this.Chain(AnsiCodes.BrightYellow);

        IChalk IChalk.BrightBlue() => this.Chain(AnsiCodes.BrightBlue);

        IChalk IChalk.BrightMagenta() => this.Chain(AnsiCodes.BrightMagenta);

        IChalk IChalk.BrightCyan() => this.Chain(AnsiCodes.BrightCyan);
        IChalk IChalk.BrightWhite() => this.Chain(AnsiCodes.BrightWhite);

        IChalk IChalk.Bold() => this.Chain(AnsiCodes.Bold);

        IChalk IChalk.Underline() => this.Chain(AnsiCodes.Underline);

        IChalk IChalk.Reverse() => this.Chain(AnsiCodes.Reverse);

        IChalk IChalk.StrikeOut() => this.Chain(AnsiCodes.StrikeOut);

        IChalk IChalk.Hide() => this.Chain(AnsiCodes.Hidden);

        IChalk IChalk.Italicize() => this.Chain(AnsiCodes.Italicize);

        IChalk IChalk.Dim() => this.Chain(AnsiCodes.Dim);

        
        IChalk IChalk.Grey() => this.Chain(AnsiCodes.BrightBlack);

        
        IChalk IChalk.BgGrey() => this.Chain(AnsiCodes.BrightBgBlack);
        

        IChalk IChalk.Rgb(Rgb rgb, bool isBgColor) => this.Chain(rgb, isBgColor);

        IChalk IChalk.Rgb(int r, int g, int b, bool isBgColor) => 
            this.Chain(new Rgb() {
                Red =r,
                Green = g,
                Blue = b
            }, isBgColor);


        IChalk IChalk.Color(Color color, bool isBgColor) => 
            this.Chain(color, isBgColor);


        IChalk IChalk.Color(string color, bool isBgColor) => 
            this.Chain(System.Drawing.Color.FromName(color), isBgColor);

        IChalk IChalk.Reset() => this.Chain(AnsiCodes.Default);
    }
}