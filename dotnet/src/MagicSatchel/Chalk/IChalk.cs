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

        IChalk Bold ();

        IChalk Underline ();

        IChalk Reverse ();

        IChalk BgBlack ();

        IChalk BgRed ();

        IChalk BgGreen ();

        IChalk BgYellow ();

        IChalk BgBlue ();

        IChalk BgMagenta ();

        IChalk BgCyan ();
        IChalk BgWhite ();

        string Draw(string value);
    }
}