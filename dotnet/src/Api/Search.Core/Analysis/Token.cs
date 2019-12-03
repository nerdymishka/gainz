using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Analysis
{
    public class Token
    {
        private int startOffset;
        private int endOffset;
        private string type = null;


        public Token(string termText, int startOffset, int endOffset, string type = "word")
        {
            this.TermText = termText;
            this.startOffset = startOffset;
            this.endOffset = endOffset;
            this.type = type;
        }

        public int IncrementPosition { get; set; }

        public string TermText { get; set; }

        public int StartOffset => this.startOffset;

        public int EndOffset => this.endOffset;

        public string Type => this.type;
    }
}
