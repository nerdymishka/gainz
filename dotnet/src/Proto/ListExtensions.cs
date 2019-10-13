using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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
        
        /// <summary>
        /// Swaps the values for the left and right indexes provided. This swap method
        /// does not perform error checking.
        /// </summary>
        /// <param name="list">A set of objects.</param>
        /// <param name="leftIndex">The left index.</param>
        /// <param name="rightIndex">The right index.</param>
        /// <typeparam name="T">The of type of items in the set.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Swap<T>(this IList<T> list, int leftIndex, int rightIndex)
        {
            T leftValue = list[leftIndex];
            list[leftIndex] = list[rightIndex];
            list[rightIndex] = leftValue;
        }
    }
}