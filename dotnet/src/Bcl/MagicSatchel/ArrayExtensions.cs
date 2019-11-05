using System;
using System.Collections;
using System.Collections.Generic;

namespace NerdyMishka
{
    public static class ArrayExtensions
    {
        
        public static void Clear<T>(this T[] array, int index = 0, int? length = null)
        {
            if(array == null)
                throw new ArgumentNullException(nameof(array));

            if(index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            if(length == null)
                length = array.Length;

            Array.Clear(array, index, length.Value);
        }

        public static T[] Grow<T>(this T[] array, int growthRate = 1)
        {
            if(array == null)
                throw new ArgumentNullException(nameof(array));

            if(growthRate < 1)
                throw new ArgumentOutOfRangeException(nameof(growthRate));

            var next = new T[array.Length + growthRate];
            Array.Copy(array, next, array.Length);

            return next;
        }

        public static T[] Shrink<T>(this T[] array, int shrinkRate = 1)
        {
            if(array == null)
                throw new ArgumentNullException(nameof(array));

            if(shrinkRate < 1)
                throw new ArgumentOutOfRangeException(nameof(shrinkRate));

            var next = new T[array.Length - shrinkRate];
            Array.Copy(array, next, next.Length);

            return next;
        }

        
        /// <summary>
        /// Performs a stable insertion sort using <see cref="System.Collections.Generic.Comparer{T}" />
        /// </summary>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <param name="comparer">The comparer used to compare two objects.</param>
        /// <typeparam name="T">The type parameter of the array.</typeparam>
        public static void InsertionSort<T>(this T[] array, int low, int high, IComparer<T> comparer)
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (comparer.Compare(array[j - 1], array[j]) > 0); j--) {
                    array.QuickSwap(j, j-1);
                }        
            }
        }
        
        /// <summary>
        /// Performs a stable insertion sort using <see cref="System.Collections.Generic.Comparison{T}" />
        /// </summary>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <param name="comparer">The comparison delegate used to compare two objects.</param>
        /// <typeparam name="T">The type parameter of the array.</typeparam>
        public static void InsertionSort<T>(this T[] array, int low, int high, Comparison<T> comparison)
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (comparison(array[j - 1], array[j]) > 0); j--) {
                    array.QuickSwap(j, j-1);
                }        
            }
        }

        /// <summary>
        /// Performs a stable insertion sort where <typeparamref name="T" /> 
        /// implements <see cref="System.IComparable{T}" /> 
        /// </summary>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <typeparam name="T">The type parameter of the array.</typeparam>
        public static void InsertionSort<T>(this T[] array, int low, int high) where T : System.IComparable<T>
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (array[j - 1]).CompareTo(array[j]) > 0; j--) {
                    array.QuickSwap(j, j-1);
                }        
            }
        }

        public static void InsertionSort<T>(this T[] array, Comparison<T> comparison)
        {
            InsertionSort(array, 0, array.Length, comparison);
        }

        
        public static void InsertionSort<T>(this T[] array, IComparer<T> comparable)
        {
            InsertionSort(array, 0, array.Length, comparable);
        }

        public static void InsertionSort<T>(this T[] array) where T: System.IComparable<T>
        {
            InsertionSort(array, 0, array.Length);
        } 

        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// that implements <see cref="System.IComparable{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] array) where T: IComparable<T>
        {
            MergeSort(array, 0, array.Length);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparison{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparison">The comparison delegate to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] array, Comparison<T> comparison) 
        {
            MergeSort(array, 0, array.Length, comparison);
        }

        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] array, IComparer<T> comparer)
        {
            MergeSort(array, 0, array.Length, comparer);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination array. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination array.
        /// </exception>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown when <paramref name="comparison" /> is null.
        /// </exception>
        public static void MergeSort<T>(T[] array, int low, int high, Comparison<T> comparison)
        {
            if(comparison == null)
                throw new ArgumentNullException(nameof(comparison));

            if(low < 0 || low >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > array.Length)
                throw new ArgumentOutOfRangeException(nameof(high));

            var src = new T[array.Length];
            Array.Copy(array, src, src.Length);

            MergeSort(src, array, low, high, comparison);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// with the specified <see cref="System.Collections.Generic.IComparer{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination array. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination array.
        /// </exception>
        /// <exception cref="System.NullReferenceException">
        ///     Thrown when <paramref name="comparer" /> is null.
        /// </exception>
        public static void MergeSort<T>(T[] array, int low, int high, IComparer<T> comparer)
        {
            if(comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if(low < 0 || low >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > array.Length)
                throw new ArgumentOutOfRangeException(nameof(high));

            var src = new T[array.Length];
            Array.Copy(array, src, src.Length);

            MergeSort(src, array, low, high, comparer);
        }

        
        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
        /// that implements <see cref="System.Collections.Generic.IComparable{T}" />
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Merge sort performs O(n log n) for the worst case. If the length
        ///         of the sort is less than 7, an <see cref="InsertionSort" /> is 
        ///         performed.
        ///     </para>
        /// </remarks>
        /// <param name="array">The array to sort.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///     The exception is thrown when <c>low</c> or <c>high</c> is out of range. 
        ///     Low must be equal to or greater than zero and must be less than the length
        ///     of the destination array. High must be greater than <c>low</c> and less than
        ///     or equal to the length of the destination array.
        /// </exception>
        public static void MergeSort<T>(T[] array, int low, int high) where T: IComparable<T>
        {
            if(low < 0 || low >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(low));

            if(high <= low || high > array.Length)
                throw new ArgumentOutOfRangeException(nameof(high));


            var src = new T[array.Length];
            Array.Copy(array, src, src.Length);

            MergeSort(src, array, low, high);
        }

        /// <summary>
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
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
        /// <param name="dest">The target array for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparer">The comparer to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] src, T[] dest, int low, int high, IComparer<T> comparer)
        {
            // from lucene 1.0 Util/Array.java  
            int length = high - low;

            if(length < 7)
            {
                dest.InsertionSort(low, high, comparer);
                return;
            }

            int mid = (low + high)/2;
            MergeSort(dest, src, low, mid, comparer);
            MergeSort(dest, src, mid, high, comparer);

            // already sorted
            if ((comparer.Compare(src[mid-1], src[mid])) <= 0) {
                System.Buffer.BlockCopy(src, low, dest, low, length);
                return;
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
        /// Performs a stable merge sort on an array of <typeparamref name="T" />
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
        /// <param name="dest">The target array for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <param name="comparison">The comparison method to use for sorting purposes.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] src, T[] dest, int low, int high, Comparison<T> comparison)
        {
            // from lucene 1.0 Util/Array.java  
            int length = high - low;

            if(length < 7)
            {
                dest.InsertionSort(low, high, comparison);
                return;
            }

            int mid = (low + high)/2;
            MergeSort(dest, src, low, mid, comparison);
            MergeSort(dest, src, mid, high, comparison);

            // already sorted
            if (comparison(src[mid-1], src[mid]) <= 0) {
                System.Buffer.BlockCopy(src, low, dest, low, length);
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
        /// Performs a stable merge sort on an array of <typeparamref name="T" /> that
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
        /// <param name="dest">The target array for sorting.</param>
        /// <param name="low">The starting index of the sort.</param>
        /// <param name="high">The ending index of the sort.</param>
        /// <typeparam name="T">The type reference for the array.</typeparam>
        public static void MergeSort<T>(T[] src, T[] dest, int low, int high) where T: IComparable<T>
        {
            // from lucene 1.0 Util/Array.java  
            int length = high - low;

            if(length < 7)
            {
                dest.InsertionSort(low, high);
                return;
            }

            int mid = (low + high)/2;
            MergeSort(dest, src, low, mid);
            MergeSort(dest, src, mid, high);

            // already sorted
            if ((src[mid-1]).CompareTo(src[mid]) <= 0) {
                System.Buffer.BlockCopy(src, low, dest, low, length);
                return;
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