namespace NerdyMishka.Search
{
     public static class NumberExtensions
    {
        public const int MinRadix = 2;
        public const int MaxRadix = 36;
        private const char charZero = '0';
        private const char charA = 'a';

        private const string digits = "0123456789abcdefghijklmnopqrstuvwxyz";


        public static char ToChar(this char c, int radix)
        {
            return ToChar((int)c, radix);
        }
        public static char ToChar(this int digit, int radix)
        {
            // if radix or digit is out of range,
            // return the null character.
            if (radix < MinRadix)
                return Char.MinValue;
            if (radix > MaxRadix)
                return Char.MinValue;
            if (digit < 0)
                return Char.MinValue;
            if (digit >= radix)
                return Char.MinValue;

            // if digit is less than 10,
            // return '0' plus digit
            if (digit < 10)
                return (char)(charZero + digit);

            // otherwise, return 'a' plus digit.
            return (char)(charA + digit - 10);
        }

        public static string EncodeToString(this int value, int radix = MaxRadix)
        {
            return EncodeToString((long)value, radix);
        }

        /// <summary>
        /// Encodes the <paramref name="value"/> into a <see cref="string"/> for indexing.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="radix">The base number to use for conversion.</param>
        /// <returns>The string value.</returns>
        public static string EncodeToString(this long value, int radix = MaxRadix)
        {
            if (radix < MinRadix || radix > MaxRadix)
                radix = 10;

            var buffer = new char[65];
            var position = 64;
            bool isNegative = (value < 0);

            if (!isNegative)
                value = -value;

            while(value <= -radix)
            {
                buffer[position--] = digits[(int)(-(value % radix))];
                value = value / radix;
            }
            buffer[position] = digits[(int)(-value)];

            if (isNegative)
                buffer[--position] = '-';


            return new string(buffer, position, (65 - position));
        }

        /// <summary>
        /// Decodes a <see cref="string"/> to an <see cref="System.Int64"/>.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Number.parse(str, int) in Lucene.Net
        ///     </para>
        /// </remarks>
        /// <param name="value">The string value</param>
        /// <param name="radix">The base number to use for conversion.</param>
        /// <returns>The int64 value.</returns>
        public static long DecodeInt64FromString(this string value, int radix = MaxRadix)
        {
            value = Check.NullParamenter(nameof(value), value);

            if (radix < MinRadix || radix > MaxRadix)
                throw new ArgumentOutOfRangeException("radix", $"radix must be between {MinRadix} and {MaxRadix}");


            long result = 0,
                 multiplier = 1;

            int i = 0,
                weight = 0;

            var normalized = value.ToLower();
            for(i = normalized.Length -1; i >= 0; i--)
            {
                char character = normalized[i];
                weight = digits.IndexOf(character);
                if (weight == -1)
                    throw new FormatException($"Invalid character, {character}, for the given radix {radix}");

                result += (weight * multiplier);
                multiplier *= radix;
            }

            return result;
        }
    }
}