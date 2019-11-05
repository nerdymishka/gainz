
using System;
using System.Collections.Generic;

namespace NerdyMishka.Extensions.Console.Options.Parser
{

    public class ArgParser
    {
        private List<ReadOnlySpan<char>> parameterPrefixes = new List<ReadOnlySpan<char>>() {
                "-".AsReadOnlySpan(),
                "--".AsReadOnlySpan(),
                "/".AsReadOnlySpan()
            };
        
        public void Parse(IList<string> args, Directory directory)
        {
            var tokens = new List<Token>();

            foreach(var segment in args)
            {
                ReadOnlySpan<char> span = segment.AsReadOnlySpan();
                if(span.Length == 0)
                {
                    tokens.Add(new Token(){
                        Type = TokenTypes.Empty,
                        Value = span 
                    });
                    continue;
                }
                bool isParameter = false;
                int prefixEnd = 0;
                foreach(var prefix in this.parameterPrefixes) 
                {
                
                    if(prefix.Length == 1)
                    {
                        if(span[0] == prefix[0]) {
                            prefixEnd = 1;
                            isParameter = true;
                            break;
                        }
                    }

                    for(int i = 0; i < prefix.Length; i++)
                    {
                        if(prefix[i] != span[i])
                        {
                            isParameter = false;
                            break;
                        }

                        prefixEnd = i;
                    }

                    isParameter = true;
                }

                if(isParameter) {
                    for(int i = prefixEnd; i < span.Length; i++)
                    {
                        var c = span[i];
                        if(!(Char.IsLetterOrDigit(c) || c == '_' || c == '-')) {
                            isParameter = false;
                            break;
                        }
                    }

                    if(isParameter) {
                        var token = new Token() {
                            Type = TokenTypes.ArgName,
                            Value = span 
                        };
                        
                        tokens.Add(token);
                        continue;
                    }
                }


                tokens.Add(new Token(){
                    Value = span,
                    Type = TokenTypes.Value
                });
            }

            // TODO: finish parser


        }
    }
}