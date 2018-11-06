using System;

namespace NerdyMishka
{

    public static class LongUtil
    {
        private const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";

        public const int CharacterMinRadix = 2;

        public const int CharacterMaxRadix = 32;

        public static long Parse(string value, int toBase = 10)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            

            if (toBase < CharacterMinRadix)
            {

                throw new ArgumentOutOfRangeException(
                    $"radix {toBase} less than Number.MIN_RADIX");
            }
            if (toBase > CharacterMaxRadix)
            {
                throw new ArgumentOutOfRangeException(
                    $"radix {toBase} greater than Number.MAX_RADIX");
            }

            long result = 0;
            long multiple = 1;

            value = value.ToLower();

            for (int i = value.Length - 1; i >= 0; i--)
            {
                int weight = digits.IndexOf(value[i]);
                if (weight == -1)
                    throw new FormatException("Invalid number for the specified radix");

                result += (weight * multiple);
                multiple *= toBase;
            }

            return result;
        }

        public static string ToString(this long value, int toBase)
        {
            if (toBase < CharacterMinRadix || toBase > CharacterMaxRadix)
                toBase = 10;

            bool negative = (value < 0);

            // when possible attempt to use .NET implementations.
            if(toBase < 17)
            {
                // to match Long.toString(i, r) in Java
                if(negative)
                {
                    value = -value;
                    return "-" + Convert.ToString(value, toBase);
                }
                    
                return Convert.ToString(value, toBase);
            }
                

            if (!negative)
                value = -value;

            Span<char> span = new char[65];
            int position = 64,
                index = 0;

            while (value <= -toBase)
            {
                index = (int)-(value % toBase);
                span[position--] = digits[index];
                value = value / toBase;
            }

            index = (int)-value;
            span[position] = digits[index];

            if (negative)
                span[--position] = '-';
            

            // TODO: use new String(span)
            return new String(span.ToArray(), position, (65 - position));
        }
    }
}