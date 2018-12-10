using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Analysis
{
    public interface IAnalyzer
    { 

        ITokenStream CreateTokenStream(TextReader reader, string fieldName = null);
    }

}
