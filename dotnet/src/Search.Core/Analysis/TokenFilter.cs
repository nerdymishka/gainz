using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Analysis
{
    public abstract class TokenFilter : ITokenStream
    {

        public abstract Token Read();

        protected ITokenStream Stream { get; set; }

        public void Dispose()
        {
            this.Stream.Dispose();
        }
    }
}
