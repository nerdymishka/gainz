using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.EfCore
{

    internal static class StringBuilderExtensions
    {

        public static StringBuilder AppendJoin(this StringBuilder sb, IEnumerable<string> source, string delimiter)
        {
            sb.Append(string.Join(delimiter, source));
            return sb;
        }
    }
}