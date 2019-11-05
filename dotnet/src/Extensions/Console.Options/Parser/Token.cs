

namespace NerdyMishka.Extensions.Console.Options.Parser
{

    public class Token
    {
        
        public TokenTypes Type { get; set; }

        public ReadOnlySpan<char> Value  { get; set; }

        
    }

    public enum TokenTypes: int
    {
        Empty = 0,
        ArgName = 1,

        Value = 2
      
    }
}