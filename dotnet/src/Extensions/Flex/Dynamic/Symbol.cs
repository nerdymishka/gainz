using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace NerdyMishka
{



    public class Symbol 
    {
        private readonly string value; 
        private static readonly List<Symbol> symbols = new List<Symbol>();
        private static readonly object s_lock = new object();
       

        public static Symbol For(string symbol)
        {
            foreach(var s in symbols)
                if(s.value == symbol)
                    return s;
            
            lock(s_lock)
            {
                var n = new Symbol(symbol);
                symbols.Add(n);
                return n;
            }
        }

        internal Symbol(string value)
        {
            var interned = string.IsInterned(value);
            if(interned == null)
                interned = string.Intern(value);

            this.value = interned;
        }


        public static implicit operator Symbol(string value)
        {
            return Symbol.For(value);
        }

        public static implicit operator string(Symbol symbol)
        {
            return symbol.value;
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
             return left.value != right.value;
        }

        public override string ToString()
        {
            return this.value;
        }

        public override bool Equals(object obj)
        {
            return obj is Symbol symbol &&
                   value == symbol.value;
        }

        public override int GetHashCode()
        {
            return -1584136870 + EqualityComparer<string>.Default.GetHashCode(value);
        }
    }

}
