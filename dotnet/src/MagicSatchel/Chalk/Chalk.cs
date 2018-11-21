using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NerdyMishka
{

    public partial class Chalk : IChalk
    {
        private const string Default = "\u001b[0m";

        private const string MarkerStart = "\u001b[";

        private const char MarkerEnd = 'm';

        private const string BrightMarkerEnd = ";1m";

        private string styling = null;

        private StringBuilder sb = null;

        internal Chalk(byte code, bool bright = false)
        {
            
            this.sb = new StringBuilder();
            if(s_enabled)
            {
                this.sb.Append(MarkerStart);
                this.sb.Append(code);
                if(bright)
                    this.sb.Append(BrightMarkerEnd);
                else 
                    this.sb.Append(MarkerEnd);
            }
         
        }

        internal IChalk Chain(byte code) 
        {
            if(s_enabled)
            {
                this.sb.Append(MarkerStart)
                    .Append(code)
                    .Append(MarkerEnd);
            }
         

            return this;
        }

        internal IChalk BrightChain(byte code) 
        {
            if(s_enabled)
            {
                this.sb.Append(MarkerStart)
                    .Append(code)
                    .Append(BrightMarkerEnd);
            }

            return this;
        }

        public string Draw(string value)
        {
            if(!s_enabled)
                return value;
            

            this.styling = this.sb.ToString();
            var sb2 = new StringBuilder();
            sb2.Append(this.sb);
            if(!value.Contains(Default))
            {
                sb2.Append(value);
                sb2.Append(Default);
                return sb2.ToString();
            }

            sb2.Append(value.Replace(Default, Default + styling));
            sb2.Append(styling);

            return sb2.ToString();
        }

        IChalk IChalk.BgBlack() => this.Chain(Colors.Black);

        IChalk IChalk.BgRed() => this.Chain(Colors.Red);

        IChalk IChalk.BgGreen() => this.Chain(Colors.Green);

        IChalk IChalk.BgYellow() => this.Chain(Colors.Yellow);

        IChalk IChalk.BgBlue() => this.Chain(Colors.Blue);

        IChalk IChalk.BgMagenta() => this.Chain(Colors.Magenta);

        IChalk IChalk.BgCyan() => this.Chain(Colors.Cyan);
        IChalk IChalk.BgWhite() => this.Chain(Colors.White);

        IChalk IChalk.Black() => this.Chain(Colors.Black);

        IChalk IChalk.Red() => this.Chain(Colors.Red);

        IChalk IChalk.Green() => this.Chain(Colors.Green);

        IChalk IChalk.Yellow() => this.Chain(Colors.Yellow);

        IChalk IChalk.Blue() => this.Chain(Colors.Blue);

        IChalk IChalk.Magenta() => this.Chain(Colors.Magenta);

        IChalk IChalk.Cyan() => this.Chain(Colors.Cyan);
        IChalk IChalk.White() => this.Chain(Colors.White);

        IChalk IChalk.BrightBlack() => this.BrightChain(Colors.Black);

        IChalk IChalk.BrightRed() => this.BrightChain(Colors.Red);

        IChalk IChalk.BrightGreen() => this.BrightChain(Colors.Green);

        IChalk IChalk.BrightYellow() => this.BrightChain(Colors.Yellow);

        IChalk IChalk.BrightBlue() => this.BrightChain(Colors.Blue);

        IChalk IChalk.BrightMagenta() => this.BrightChain(Colors.Magenta);

        IChalk IChalk.BrightCyan() => this.BrightChain(Colors.Cyan);
        IChalk IChalk.BrightWhite() => this.BrightChain(Colors.White);

        IChalk IChalk.Bold() => this.Chain(Style.Bold);

        IChalk IChalk.Underline() => this.Chain(Style.Underline);

        IChalk IChalk.Reverse() => this.Chain(Style.Reversed);

        internal struct Colors {

            
            public const byte Black = 30;
            public const byte Red = 31;
                
            public const byte Green = 32;
            public const byte Yellow = 33;

            public const byte Blue =34;
            public const byte Magenta = 35;
            public const byte Cyan = 36;
            public const byte White = 37;
            public const byte BgBlack = 40;

            public const byte BgRed = 41;
            public const byte BgGreen = 42;
            public const byte BgYellow = 43;

            public const byte BgBlue = 44;
            public const byte BgMagenta = 45;
            public const byte BgCyan = 46;

            public const byte BgWhite = 47;
        } 

        internal struct Style
        {
            public const byte Bold = 1;
            public const byte Underline = 4;
            public const byte Reversed = 7;

        }   
    }

}