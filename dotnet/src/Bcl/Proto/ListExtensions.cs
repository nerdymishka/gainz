using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using NerdyMishka.Validation;

namespace NerdyMishka
{
    /// <summary>
    /// Extensions for collections that implement <see cref="System.Collections.Generic.IList{T}" />
    /// </summary>
    /// <remarks>
    /// List extensions should provide some kind of Sort overload for lists that 
    /// take the sort type, startIndex, endIndex, and overloads for the 
    /// <see cref="System.Collections.Generic.Comparison{T}" /> delegate.sealed 
    /// </remarks>
    public static class ListExtensions
    {
        
        /// <summary>
        /// Shuffles the items in the collection
        /// </summary>
        /// <param name="list">The list instance.</param>
        /// <param name="random">(Optional) instance of <see cref="IRandom" /> used to shuffle items between indexes. </param>
        /// <typeparam name="T">The item type.</typeparam>
        public static void Shuffle<T>(this IList<T> list, [AllowNull] IRandom random = null)
        {
            Check.NotNull(nameof(list), list);

            random = random ?? new DotNetRandom();
            int n = list.Count;
            while (n > 1)
            {
                int k = random.NextInt32(n--);
                list.Swap(n, k);
            }
        }
        
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