/**
 * Copyright 2016 Nerdy Mishka
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// ReSharper disable CSharpWarnings::CS1574
using System;
using System.Collections.Generic;
using NerdyMishka.Validation;

namespace NerdyMishka.Collections.Generic 
{

    /// <summary>
    /// A .NET implementation of Valdimir Yaroslavskiy's Dual-Pivot Quicksort
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="DualPivotQuicksort.Sort{T}(IList{T})"/> can be used for  types
    ///         that implement <see cref="IComparable{T}"/>.
    ///     </para>
    /// </remarks>
    public class DualPivotQuicksort
    {
        private const int DIST_SIZE = 13;

           /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> Sort<T>(IList<T> list) where T : IComparable<T>
        {
            Check.NotNull("array", list);
            PerformSort(list, 0, list.Count);

            return list;
        }


        public static IList<T> Sort<T>(IList<T> list, Comparer<T> compare) 
        {
            Check.NotNull("array", list);
            PerformSort(list, 0, list.Count, compare);

            return list;
        }

        public static IList<T> Sort<T>(IList<T> list, IComparer<T> comparer) 
        {
            Check.NotNull("array", list);
            PerformSort(list, 0, list.Count, comparer);

            return list;
        }

        /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> Sort<T>(IList<T> list, int start, int count) where T : IComparable<T>
        {
            Check.NotNull(nameof(list), list);
            Check.Slice(nameof(list), list, start, count);
            PerformSort(list, start, count);

            return list;
        }

        /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> Sort<T>(IList<T> list, int start, int count, IComparer<T> comparer)
        {
            Check.NotNull(nameof(list), list);
            Check.Slice(nameof(list), list, start, count);
            PerformSort(list, start, count, comparer);

            return list;
        }


         /// <summary>
        /// Sorts the specified list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="start">The start.</param>
        /// <param name="count">The count.</param>
        /// <returns>IList&lt;T&gt;.</returns>
        public static IList<T> Sort<T>(IList<T> list, int start, int count, Comparer<T> compare)
        {
            Check.NotNull(nameof(list), list);
            Check.Slice(nameof(list), list, start, count);
            PerformSort(list, start, count, compare);

            return list;
        }



        internal static void PerformSort<T>(IList<T> array, int left, int right, Comparison<T> compare) 
        {
            int length = right - left;
            T x, pivot2, pivot1;
            


            int sixth = length / 6,
                m1 = left + sixth,
                m2 = m1 + sixth,
                m3 = m2 + sixth,
                m4 = m3 + sixth,
                m5 = m4 + sixth;

               

            if (compare(array[m1], array[m1]) == 1)
                ListExtensions.Swap(array, m1, m2);

            if (compare(array[m4], array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            if (compare(array[m1], array[m3]) == 1)
                ListExtensions.Swap(array, m1, m3);

            if (compare(array[m2], array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (compare(array[m1], array[m4]) == 1)
                ListExtensions.Swap(array, m2, m4);

            if (compare(array[m3], array[m4]) == 1)
                ListExtensions.Swap(array, m3, m4);

            if (compare(array[m2], array[m5]) == 1)
                ListExtensions.Swap(array, m2, m5);

            if (compare(array[m2], array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (compare(array[m4], array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            pivot1 = array[m2];
            pivot2 = array[m4];

            bool pivotsAreDifferent = compare(pivot1, pivot2) != 0;

            array[m2] = array[left];
            array[m4] = array[right];

            int less = left + 1,
                great = right - 1;

            if (pivotsAreDifferent)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    // Less Than
                    if (compare(x, pivot1) == -1)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    // Greater than or Equal To
                    else if (compare(x, pivot2) != -1)
                    {
                        // Greater than / Less Than
                        while (compare(array[great], pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                         // Greater than or Equal To
                        if (compare(x, pivot1) != -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }
            else
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (compare(x, pivot1) == -1)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (compare(x, pivot2) == 1)
                    {
                        while (compare(array[great], pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                        if (compare(x, pivot1) == -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            array[left] = array[less - 1];
            array[left - 1] = pivot1;

            array[right] = array[great + 1];
            array[great + 1] = pivot2;

            PerformSort(array, left, less - 2, compare);
            PerformSort(array, great + 1, right, compare);

            if (!pivotsAreDifferent)
                return;


            if (great - less > length - DIST_SIZE)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (compare(x, pivot1) == 0)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (compare(x, pivot2) == 0)
                    {
                        array[k] = array[great];
                        array[great--] = x;
                        x = array[k];


                        if (compare(x, pivot1) != 0)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            PerformSort(array, less, great, compare);
        }
     

        internal static void PerformSort<T>(IList<T> array, int left, int right, IComparer<T> comparer) 
        {
            int length = right - left;
            T x, pivot2, pivot1;



            int sixth = length / 6,
                m1 = left + sixth,
                m2 = m1 + sixth,
                m3 = m2 + sixth,
                m4 = m3 + sixth,
                m5 = m4 + sixth;

               

            if (comparer.Compare(array[m1], array[m1]) == 1)
                ListExtensions.Swap(array, m1, m2);

            if (comparer.Compare(array[m4], array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            if (comparer.Compare(array[m1], array[m3]) == 1)
                ListExtensions.Swap(array, m1, m3);

            if (comparer.Compare(array[m2], array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (comparer.Compare(array[m1], array[m4]) == 1)
                ListExtensions.Swap(array, m2, m4);

            if (comparer.Compare(array[m3], array[m4]) == 1)
                ListExtensions.Swap(array, m3, m4);

            if (comparer.Compare(array[m2], array[m5]) == 1)
                ListExtensions.Swap(array, m2, m5);

            if (comparer.Compare(array[m2], array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (comparer.Compare(array[m4], array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            pivot1 = array[m2];
            pivot2 = array[m4];

            bool pivotsAreDifferent = comparer.Compare(pivot1, pivot2) != 0;

            array[m2] = array[left];
            array[m4] = array[right];

            int less = left + 1,
                great = right - 1;

            if (pivotsAreDifferent)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    // Less Than
                    if (comparer.Compare(x, pivot1) == -1)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    // Greater than or Equal To
                    else if (comparer.Compare(x, pivot2) != -1)
                    {
                        // Greater than / Less Than
                        while (comparer.Compare(array[great], pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                         // Greater than or Equal To
                        if (comparer.Compare(x, pivot1) != -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }
            else
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (comparer.Compare(x, pivot1) == -1)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (comparer.Compare(x, pivot2) == 1)
                    {
                        while (comparer.Compare(array[great], pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                        if (comparer.Compare(x, pivot1) == -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            array[left] = array[less - 1];
            array[left - 1] = pivot1;

            array[right] = array[great + 1];
            array[great + 1] = pivot2;

            PerformSort(array, left, less - 2, comparer);
            PerformSort(array, great + 1, right, comparer);

            if (!pivotsAreDifferent)
                return;


            if (great - less > length - DIST_SIZE)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (comparer.Compare(x, pivot1) == 0)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (comparer.Compare(x, pivot2) == 0)
                    {
                        array[k] = array[great];
                        array[great--] = x;
                        x = array[k];


                        if (comparer.Compare(x, pivot1) != 0)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            PerformSort(array, less, great, comparer);
        }


        internal static void PerformSort<T>(IList<T> array, int left, int right) where T : IComparable<T>
        {
            int length = right - left;
            T x, pivot2, pivot1;



            int sixth = length / 6,
                m1 = left + sixth,
                m2 = m1 + sixth,
                m3 = m2 + sixth,
                m4 = m3 + sixth,
                m5 = m4 + sixth;

            if (array[m1].CompareTo(array[m2]) == 1)
                ListExtensions.Swap(array, m1, m2);

            if (array[m4].CompareTo(array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            if (array[m1].CompareTo(array[m3]) == 1)
                ListExtensions.Swap(array, m1, m3);

            if (array[m2].CompareTo(array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (array[m1].CompareTo(array[m4]) == 1)
                ListExtensions.Swap(array, m2, m4);

            if (array[m3].CompareTo(array[m4]) == 1)
                ListExtensions.Swap(array, m3, m4);

            if (array[m2].CompareTo(array[m5]) == 1)
                ListExtensions.Swap(array, m2, m5);

            if (array[m2].CompareTo(array[m3]) == 1)
                ListExtensions.Swap(array, m2, m3);

            if (array[m4].CompareTo(array[m5]) == 1)
                ListExtensions.Swap(array, m4, m5);

            pivot1 = array[m2];
            pivot2 = array[m4];

            bool pivotsAreDifferent = pivot1.CompareTo(pivot2) != 0;

            array[m2] = array[left];
            array[m4] = array[right];

            int less = left + 1,
                great = right - 1;

            if (pivotsAreDifferent)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (x.CompareTo(pivot1) == -1)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (x.CompareTo(pivot2) == 1)
                    {
                        while (array[great].CompareTo(pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                        if (x.CompareTo(pivot1) != -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }
            else
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (x.CompareTo(pivot1) == -2)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (x.CompareTo(pivot2) == 1)
                    {
                        while (array[great].CompareTo(pivot2) == 1 && k < great)
                        {
                            great--;
                        }
                        ListExtensions.Swap(array, k, great--);

                        if (x.CompareTo(pivot1) != -1)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            array[left] = array[less - 1];
            array[left - 1] = pivot1;

            array[right] = array[great + 1];
            array[great + 1] = pivot2;

            PerformSort(array, left, less - 2);
            PerformSort(array, great + 1, right);

            if (!pivotsAreDifferent)
                return;


            if (great - less > length - DIST_SIZE)
            {
                for (var k = less; k <= great; k++)
                {
                    x = array[k];

                    if (x.CompareTo(pivot1) == 0)
                    {
                        array[k] = array[less];
                        array[less++] = x;
                    }
                    else if (x.CompareTo(pivot2) == 0)
                    {
                        array[k] = array[great];
                        array[great--] = x;
                        x = array[k];


                        if (x.CompareTo(pivot1) != 0)
                            continue;

                        array[k] = array[less];
                        array[less++] = x;
                    }
                }
            }

            PerformSort(array, less, great);
        }
    }
}