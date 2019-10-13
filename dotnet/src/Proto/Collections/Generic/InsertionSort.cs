using System;
using System.Collections.Generic;

namespace NerdyMishka.Collections.Generic
{
    public class InsertionSort
    {
         
        /// <summary>
        /// Performs a stable insertion sort using <see cref="System.Collections.Generic.Comparer{T}" />
        /// </summary>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <param name="comparer">The comparer used to compare two objects.</param>
        /// <typeparam name="T">The type parameter of the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable, int low, int high, IComparer<T> comparer)
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (comparer.Compare(sortable[j - 1], sortable[j]) > 0); j--) {
                    sortable.Swap(j, j-1);
                }        
            }
        }
        
        /// <summary>
        /// Performs a stable insertion sort using <see cref="System.Collections.Generic.Comparison{T}" />
        /// </summary>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <param name="comparer">The comparison delegate used to compare two objects.</param>
        /// <typeparam name="T">The type parameter of the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable, int low, int high, Comparison<T> comparison)
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (comparison(sortable[j - 1], sortable[j]) > 0); j--) {
                    sortable.Swap(j, j-1);
                }        
            }
        }

        /// <summary>
        /// Performs a stable insertion sort where <typeparamref name="T" /> 
        /// implements <see cref="System.IComparable{T}" /> 
        /// </summary>
        /// <param name="sortable">The sortable to sort.</param>
        /// <param name="low">The start index to sort.</param>
        /// <param name="high">The final index to sort.</param>
        /// <typeparam name="T">The type parameter of the sortable.</typeparam>
        public static void Sort<T>(IList<T> sortable, int low, int high) where T : System.IComparable<T>
        {
            for (int i = low; i < high; i++) {
                for(int j = i; j > low && (sortable[j - 1]).CompareTo(sortable[j]) > 0; j--) {
                    sortable.Swap(j, j-1);
                }        
            }
        }

        public static void Sort<T>(IList<T> sortable, Comparison<T> comparison)
        {
            Sort(sortable, 0, sortable.Count, comparison);
        }

        
        public static void Sort<T>(IList<T> sortable, IComparer<T> comparable)
        {
            Sort(sortable, 0, sortable.Count, comparable);
        }

        public static void Sort<T>(IList<T> sortable) where T: System.IComparable<T>
        {
            Sort(sortable, 0, sortable.Count);
        } 
    }
}