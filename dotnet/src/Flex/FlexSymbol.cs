using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace NerdyMishka.Flex
{
    public class FlexSymbol : FObject, IEquatable<FlexSymbol>,
        IEquatable<string>
    {
        private string symbol = null;
        private int hashCode;
        private static readonly ConcurrentBag<WeakReference<FlexSymbol>> flexSymbols = 
            new ConcurrentBag<WeakReference<FlexSymbol>>();

        private static readonly object s_lock = new object();

        public FlexSymbol(string value)
        {
            this.symbol = value;
            this.hashCode = value.GetHashCode() + 31;
        }

        public static FlexSymbol For(string name)
        {
            lock(s_lock)
            {
                FlexSymbol result = null;
                var remove = new List<WeakReference<FlexSymbol>>();
                foreach(var reference in flexSymbols)
                {
                    if(reference.TryGetTarget(out FlexSymbol target))
                    {
                        if (target.symbol == name)
                        {
                            result = target;
                           
                        }

                        continue;
                    }

                    remove.Add(reference);
                }

                foreach(var next in remove)
                {
                    WeakReference<FlexSymbol> target = next;
                    flexSymbols.TryTake(out target);
                }
                

                if (result != null)
                    return result;

                result = new FlexSymbol(name);
                flexSymbols.Add(new WeakReference<FlexSymbol>(result));

                return result;
            }
        }

        public override FlexType FlexType => FlexType.FlexSymbol;

        public static implicit operator string(FlexSymbol value)
        {
            return value.symbol;
        }

        public static implicit operator FlexSymbol(string value)
        {
            if (value == null)
                return null;

            return For(value);
        }

       

        public static bool operator ==(FlexSymbol left, FlexSymbol right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (ReferenceEquals(left, null))
                return false;

            if (ReferenceEquals(right, null))
                return false;


            return left.symbol == right.symbol;
        }

        public static bool operator !=(FlexSymbol left, FlexSymbol right)
        {
            return !(left == right);
        }



        public bool Equals(string other)
        {
            if (other == null)
                return false;

            return this.symbol == other;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is FlexSymbol)
                this.Equals((FlexSymbol)obj);

            if (obj is string)
                this.Equals((string)obj);

            return false;
        }

        public override string ToString()
        {
            return this.symbol;
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override object Unbox()
        {
            return this.symbol;
        }

        public bool Equals(FlexSymbol other)
        {
            return this == other;
        }
    }
}
