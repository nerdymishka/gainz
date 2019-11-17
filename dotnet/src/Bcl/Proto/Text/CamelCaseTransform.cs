

using System.Collections.Generic;
using System.Text;
using System;

namespace NerdyMishka.Text
{
    public class CamelCaseTransform : ITextTransform
    {

       
        public ReadOnlySpan<char> Transform(IEnumerable<char> identifier)
        {
            var sb = new StringBuilder();
            var transform = false;
            var i = 0;
            foreach(var c in identifier)
            {
                if(i == 0 && char.IsUpper(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                if(c == '-' || c == '_' || c == ' ')
                {
                    transform = true;
                    i++;
                    continue;
                }

                if(transform)
                {
                    sb.Append(char.ToUpperInvariant(c));
                    transform = false;
                    i++;
                    continue;
                }
              
                sb.Append(c);
                i++;
            }

            var set = new char[sb.Length];
            sb.CopyTo(0, set, 0, set.Length);
            return set.AsSpan();            
        }

        public ReadOnlySpan<char> Transform(ReadOnlySpan<char> identifier)
        {
            var sb = new StringBuilder();
            var transform = false;
            var i = 0;
            foreach(var c in identifier)
            {
                if(i == 0 && char.IsUpper(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    i++;
                    continue;
                }

                if(c == '-' || c == '_')
                {
                    transform = true;
                    i++;
                    continue;
                }

                if(transform)
                {
                    sb.Append(char.ToUpperInvariant(c));
                    i++;
                    continue;
                }
              
                sb.Append(c);
                i++;
            }

            var set = new char[sb.Length];
            sb.CopyTo(0, set, 0, set.Length);
            return set.AsSpan();    
        }

      
    }
}