

using System.Text;
using System;
using System.Collections.Generic;

namespace NerdyMishka.Text 
{

    public interface ITextTransform
    {

       

        ReadOnlySpan<char> Transform(ReadOnlySpan<char> identifier);

        ReadOnlySpan<char> Transform(IEnumerable<char> identifier);

       
    }
}