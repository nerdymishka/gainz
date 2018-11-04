namespace NerdyMishka.Search.Util
{
    public class ArrayUtil
    {

        /// <summary>
        /// Swaps the values at the specified indexes.
        /// </summary>
        /// <param name="set">An array of strings.</param>
        /// <param name="left">index on the left side, that is replaced with the value from the rightIndex.</param>
        /// <param name="right">index on the right side, that is replaced with the value from the leftIndex.</param>
        public static void Swap(string[] set, int leftIndex, int rightIndex)
        {
            string pointer = set[leftIndex];
            set[leftIndex] = set[rightIndex];
            set[rightIndex] = pointer;
        }
    }
}