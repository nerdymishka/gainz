using System;
using System.Collections.Generic;


namespace NerdyMishka.Collections.Generic 
{
    public class MergeSort
    {

        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// that implements <see cref="System.IComparable{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable) where T: IComparable<T>
        {
            Sort(sortable, 0, sortable.Count);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparison{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="comparison">The comparison delegate to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable, Comparison<T> comparison) 
        {
            Sort(sortable, 0, sortable.Count, comparison);
        }

        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable, IComparer<T> comparer)
        {
            Sort(sortable, 0, sortable.Count, comparer);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination sortable. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination sortable.
        /// </exception>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown when <paramref name="comparison" /> is null.
        /// </exception>
        public static void Sort<T>(IList<T> sortable, int low, int high, Comparison<T> comparison)
        {
            if(comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            if(low < 0 || low >= sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(high));

            var src = new T[sortable.Count];
            sortable.CopyTo(src, 0);

            Sort(src, sortable, low, high, comparison);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination sortable. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination sortable.
        /// </exception>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown when <paramref name="comparer" /> is null.
        /// </exception>
        public static void Sort<T>(IList<T> sortable, int low, int high, IComparer<T> comparer)
        {
            if(comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if(low < 0 || low >= sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(high));

            var src = new T[sortable.Count];
            sortable.CopyTo(src, 0);

            Sort(src, sortable, low, high, comparer);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// that implements <see cref="System.Collections.Generic.IComparable{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination sortable. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination sortable.
        /// </exception>
        public static void Sort<T>(IList<T> sortable, int low, int high) where T: IComparable<T>
        {
            if(low < 0 || low >= sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > sortable.Count)
                throw new ArgumentOutOfRangeException(nameof(high));


            var src = new T[sortable.Count];
            sortable.CopyTo(src, 0);

            Sort(src, sortable, low, high);
        }

        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="src">The auxiliary copy of the destination sort.</param>
        /// <param name="dest">The target sortable for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> src, IList<T> dest, int low, int high, IComparer<T> comparer)
        {
            // from lucene 1.0 Util/sortable.java  
            int length = high - low;

            if(length < 7)
            {
                InsertionSort.Sort(dest, low, high, comparer); 
                return;
            }

            int mid = (low + high)/2;
            Sort(dest, src, low, mid, comparer);
            Sort(dest, src, mid, high, comparer);

            // already sorted
            if ((comparer.Compare(src[mid-1], src[mid])) <= 0) {
               if(src is Array && dest is Array) {
                    System.Buffer.BlockCopy((Array)src, low, (Array)dest, low, length);
                    return;
                }

                for(int i = low; i < length; i++)
                {
                    dest[i] = src[i];
                }
            }

            // Merge sorted halves (now in src) into dest
            for(int i = low, p = low, q = mid; i < high; i++) {
                if (q>=high || p < mid && (comparer.Compare(src[p], src[q]) <= 0))
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        
        
        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" />
        /// with the specified comparison delegate.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="src">The auxiliary copy of the destination sort.</param>
        /// <param name="dest">The target sortable for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparison">The comparison method to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> src, IList<T> dest, int low, int high, Comparison<T> comparison)
        {
            // from lucene 1.0 Util/sortable.java  
            int length = high - low;

            if(length < 7)
            {
                InsertionSort.Sort(dest, low, high, comparison);
                return;
            }

            int mid = (low + high)/2;
            Sort(dest, src, low, mid, comparison);
            Sort(dest, src, mid, high, comparison);

            // already sorted
            if (comparison(src[mid-1], src[mid]) <= 0) {
                if(src is Array && dest is Array) {
                    System.Buffer.BlockCopy((Array)src, low, (Array)dest, low, length);
                    return;
                }

                for(int i = low; i < length; i++)
                {
                    dest[i] = src[i];
                }
              
                return;
            }

            // Merge sorted halves (now in src) into dest
            for(int i = low, p = low, q = mid; i < high; i++) {
                if (q>=high || p < mid && (comparison(src[p], src[q]) <= 0))
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }

        /// <summary>
        /// Performs a stable merge sort on an sortable of <typeparamref name="T" /> that
        /// implements <see cref="IComparable" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="src">The auxiliary copy of the destination sort.</param>
        /// <param name="dest">The target sortable for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <typeparam name="T">The type reference for the sortable.</typeparam>
        public static void Sort<T>(IList<T> src, IList<T> dest, int low, int high) where T: IComparable<T>
        {
            // from lucene 1.0 Util/sortable.java  
            int length = high - low;

            if(length < 7)
            {
                InsertionSort.Sort(dest, low, high);
                return;
            }

            int mid = (low + high)/2;
            Sort(dest, src, low, mid);
            Sort(dest, src, mid, high);

            // already sorted
            if ((src[mid-1]).CompareTo(src[mid]) <= 0) {
                if(src is Array && dest is Array) {
                    System.Buffer.BlockCopy((Array)src, low, (Array)dest, low, length);
                    return;
                }

                for(int i = low; i < length; i++)
                {
                    dest[i] = src[i];
                }
            }

            // Merge sorted halves (now in src) into dest
            for(int i = low, p = low, q = mid; i < high; i++) {
                if (q>=high || p<mid && (src[p]).CompareTo(src[q])<=0)
                    dest[i] = src[p++];
                else
                    dest[i] = src[q++];
            }
        }
    }
}