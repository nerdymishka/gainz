using System.Collections.Generic;

namespace NerdyMishka
{

    public class AnsiCodeMap
    {
        private static Dictionary<int, string> s_map = null;

        static AnsiCodeMap()
        {
            s_map = new Dictionary<int, string>() {
                // 21 is the proper way to remove bold, 
                // but not supported in most terminals

                // 39 / 49 work for bright colors
                { 1, "22" },  { 2, "22" },   { 3, "23" },   { 4, "24" },
                { 5, "25" },    { 6, "26" },     { 7, "27" },   { 8, "28" },
                { 9, "29" },    { 30, "39" },   { 31, "39" },   { 32, "39" },
                { 33, "39" },   { 34, "39" },   { 35, "39" },   { 36, "39" },
                { 37, "39" },   { 38, "39" },   { 40, "49" },   { 41, "49" },
                { 42, "49" },   { 43, "49" },   { 44, "49" },   { 45, "49" },
                { 46, "49" },   { 47, "49" },   { 48, "49" },   { 90, "39" },
                { 91, "39" },   { 92, "39" },   { 93, "39" },   { 94, "39" },
                { 95, "39" },   { 96, "39" },   { 97, "39" },   { 100, "49" },
                { 101, "49" },  { 102, "49" },  { 103, "49" },  { 104, "49" },
                { 105, "49" },  { 106, "49" },  { 107, "49" },  { 108, "49" },
            };
        }

        public static string Reverse(AnsiCodes ansiCode)
        {
            return Reverse((int)ansiCode);
        }
        public static string Reverse(int ansiCode)
        {
            if(s_map.TryGetValue(ansiCode, out string reverse))
                return reverse;

            return "0";
        }
    }

    public enum AnsiCodes : int 
    {
        Default = 0,

        Bold = 1,

        Dim = 2,

        Italicize = 3,

        Underline = 4,

        Blink = 5,
        Glitch = 6,

        Reverse = 7,

        Hidden = 8,

        StrikeOut = 9,

        PrimaryFont = 10,

        AltFont1 = 11,

        AltFont2 = 12,

        AltFont3 = 13,

        AltFont4 = 14, 

        AltFont5 = 15, 


        AltFont6 = 16, 

        AltFont7 = 17, 

        AltFont8 = 18, 

        AltFont9 = 19, 

        FrakturFont = 20,

        BoldOff = 21,

        DimOff = 22,

        ItalicizeOff = 23,

        UnderlineOff = 24,

        BlinkOff = 25,

        GlitchOff = 26,

        ReverseOff = 27,

        HiddenOff = 28,

        StikeOutOff = 29,

        Black = 30,

        Red = 31,

        Green = 32,

        Yellow = 33,

        Blue = 34, 

        Magenta = 35,

        Cyan = 36,

        White = 37,

        Rgb = 38,

        DefaultColor = 39,

        BgBlack = 40,

        BgRed = 41,

        BgGreen = 42,

        BgYellow = 33,

        BgBlue = 34,

        BgMagenta = 35,

        BgCyan = 36,

        BgWhite = 37,

        BgRgb = 38,

        BgDefaultColor = 49,

        Frame = 51,

        Encircled = 52,

        Overlined = 53,

        FrameOrEncircledOff = 54,

        OverlinedOf = 55,


        BrightBlack = 90,

        BrightRed = 91,

        BrightGreen = 92,

        BrightYellow = 93,

        BrightBlue = 94, 

        BrightMagenta = 95,

        BrightCyan = 96,

        BrightWhite = 97,


        BrightBgBlack = 100,

        BrightBgRed = 101,

        BrightBgGreen = 102,

        BrightBgYellow = 103,

        BrightBgBlue = 104,

        BrightBgMagenta = 105,

        BrightBgCyan = 106,

        BrightBgWhite = 107,


    }
}