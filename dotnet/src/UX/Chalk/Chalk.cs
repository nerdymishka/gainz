using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NerdyMishka
{

    public partial class Chalk 
    {
        private List<AnsiStyle> styles = new List<AnsiStyle>();

        internal Chalk(AnsiCodes ansiCode) 
        {
            this.styles.Add(new AnsiCode((int)ansiCode));
        }

        internal Chalk(int ansiCode) 
        {
            this.styles.Add(new AnsiCode(ansiCode));
        }

        internal Chalk(AnsiCodes[] ansiCodes) 
        {
            if(ansiCodes != null && ansiCodes.Length > 0)
                this.styles.AddRange(ansiCodes.Select(o => new AnsiCode((int)o)));
        }

        internal Chalk(int[] ansiCodes)
        {
            if(ansiCodes != null && ansiCodes.Length > 0)
                this.styles.AddRange(ansiCodes.Select(o => new AnsiCode(o)));
        }

        internal Chalk(Rgb rgb, bool isBgColor)
        {
            this.styles.Add(new AnsiRgbColor(rgb, isBgColor));
        }

        internal Chalk(Color color, bool isBgColor)
        {
            this.styles.Add(new AnsiRgbColor(color, isBgColor));
        }

        internal IChalk Chain(int code) 
        {
            this.styles.Add(new AnsiCode(code));

            return this;
        }

         internal IChalk Chain(AnsiCodes code) 
        {
            return this.Chain((int)code);
        }


        internal IChalk Chain(Color color, bool isBgColor) 
        {
            this.styles.Add(new AnsiRgbColor(color, isBgColor));

            return this;
        }

        internal IChalk Chain(Rgb color, bool isBgColor) 
        {
            this.styles.Add(new AnsiRgbColor(color, isBgColor));

            return this;
        }
        

        public string Draw(string value)
        {
            if(!ChalkConsole.EnableVirtualTerminalStdOut())
                return value;

            if(this.styles.Count == 0)
                return value;

            var sb = new StringBuilder();

       
            sb.Append(AsciiCodes.Escape)
                .Append('[');
            int joinBreak = this.styles.Count - 1;
            for(var i = 0; i < this.styles.Count; i++)
            {
                
                sb.Append(this.styles[i].ToString());
                if(i < joinBreak)
                    sb.Append(";");
                else 
                    sb.Append("m");
        
            }
            

            sb.Append(value)
                .Append(AsciiCodes.Escape)
                .Append('[');

            for(var i = 0; i < this.styles.Count; i++)
            {
                
                sb.Append(this.styles[i].Reverse());
                if(i < joinBreak)
                    sb.Append(";");
                else 
                    sb.Append("m");
                    
            
            }

            return sb.ToString();
        }



    }

}