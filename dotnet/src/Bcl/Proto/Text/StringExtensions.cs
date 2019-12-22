

using System.Text.RegularExpressions;

namespace NerdyMishka.Text
{

    public static class StringExtensions
    {

        public static bool Match(this string left, string pattern, bool ignoreCase =true, bool isRegex = false)
        {
            if(!isRegex)
            {
                if(ignoreCase)
                    return string.Equals(left, pattern, System.StringComparison.OrdinalIgnoreCase);

                return left == pattern;
            }
            var options = RegexOptions.None;
            if(ignoreCase)
                options = RegexOptions.IgnoreCase;

            return Regex.IsMatch(left, pattern, options);
        }
    }
    
}