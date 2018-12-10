using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BadMishka.DocumentFormat.LuceneIndex.Analysis
{
    public abstract class Analyzer : IAnalyzer
    {
        public abstract ITokenStream CreateTokenStream(TextReader reader, string fieldName = null);
    }
}
