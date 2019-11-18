using System.Text;

namespace NerdyMishka.Text
{
    public static class Encodings
    {
        public static readonly Encoding Utf8NoBom = new UTF8Encoding(false);

        public static readonly Encoding Utf8 = Encoding.UTF8;

        public static readonly Encoding Utf7 = Encoding.UTF7;

        public static readonly Encoding Utf32 = Encoding.UTF32;

        public static readonly Encoding Ascii = Encoding.ASCII;

        public static readonly Encoding Unicode = Encoding.Unicode;

        
    }
}