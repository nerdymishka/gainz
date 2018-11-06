using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// List extensions should provide some kind of Sort overload for lists that 
    /// take the sort type, startIndex, endIndex, and overloads for the 
    /// <see cref="System.Collections.Generic.Comparison{T}" /> delegate.sealed 
    /// </remarks>
    public static class ListExtensions
    {

        private static void InsertionSort<T>(IList<T> list, int low, int high) where T : System.IComparable
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (list[j - 1]).CompareTo(list[j]) > 0; j--) {
                    list.QuickSwap(j, j-1);
                }        
            }
        }

        private static void InsertionSort<T>(IList<T> list) where T: System.IComparable
        {
            InsertionSort(list, 0, list.Count);
        } 

        private static void MergeSort<T>(IList<T> list, int low, int high) where T: System.IComparable
        {
            var cp = list.ToArray();
            MergeSort(cp, list, low, high);
        }

        private static void MergeSort<T>(IList<T> src, IList<T> dest, int low, int high) where T : System.IComparable
        {
            int length = high - low;

            if(length < 7)
            {
                InsertionSort(dest);
                return;
            }

            int mid = (low + high) / 2;
            MergeSort(dest, src, low, mid);
            MergeSort(dest, src, mid, high);

            if ((src[mid - 1]).CompareTo(src[mid]) <= 0) {
            
                for(int k = low; k < length; k++) {
                    dest[k] = src[k];
                }
                return;
            }
            var  x = new List<T>();
        }
        
        /// <summary>
        /// Swaps the values for the left and right indexes provided. This swap method
        /// does not perform error checking.
        /// </summary>
        /// <param name="list">A set of objects.</param>
        /// <param name="leftIndex">The left index.</param>
        /// <param name="rightIndex">The right index.</param>
        /// <typeparam name="T">The of type of items in the set.</typeparam>
        public static void QuickSwap<T>(this IList<T> list, int leftIndex, int rightIndex)
        {
            T leftValue = list[leftIndex];
            list[leftIndex] = list[rightIndex];
            list[rightIndex] = leftValue;
        }
    }
}