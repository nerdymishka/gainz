using System.Drawing;

namespace NerdyMishka 
{
    public interface IChalk 
    {
        IChalk Black ();

        IChalk Red ();

        IChalk Green ();

        IChalk Yellow ();

        IChalk Blue ();

        IChalk Magenta ();
        IChalk Cyan ();

        IChalk White ();

        IChalk BrightBlack ();

        IChalk BrightRed ();

        IChalk BrightGreen ();

        IChalk BrightYellow ();

        IChalk BrightBlue ();

        IChalk BrightMagenta ();

        IChalk BrightCyan ();
        IChalk BrightWhite ();

        IChalk BgBlack ();

        IChalk BgRed ();

        IChalk BgGreen ();

        IChalk BgYellow ();

        IChalk BgBlue ();

        IChalk BgMagenta ();

        IChalk BgCyan ();
        IChalk BgWhite ();

        IChalk Bold ();

        IChalk Underline ();

        IChalk Reverse ();

        IChalk StrikeOut();

        IChalk Italicize();

        IChalk Hide();

        IChalk Dim();

        IChalk Grey();

        IChalk BgGrey();

        IChalk Rgb(Rgb rgb, bool isBbColor = false);

        IChalk Color(Color color, bool isBbColor = false);

        IChalk Color(string color, bool isBbColor = false);

        IChalk Rgb(int r, int g, int b, bool isBbColor = false);

        IChalk Reset();

        string Draw(string value);
    }
}