namespace NerdyMishka.Search
{
    public static class StringExtensions
    {
        /// <summary> Compares two strings, character by character, and returns the
		/// first position where the two strings differ from one another.
		/// 
		/// </summary>
		/// <param name="s1">The first string to compare
		/// </param>
		/// <param name="other">The second string to compare
		/// </param>
		/// <returns> The first position where the two strings differ.
		/// </returns>
		public static int DivergentIndex(this string s1, string other)
        {
            int len1 = s1.Length;
            int len2 = other.Length;
            int len = len1 < len2 ? len1 : len2;
            for (int i = 0; i < len; i++)
            {
                if (s1[i] != other[i])
                {
                    return i;
                }
            }
            return len;
        }
    }
}