using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadMishka.DocumentFormat.LuceneIndex.Analysis
{
    public interface ITokenStream : IDisposable
    {
        Token Read();
    }
}
