using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NerdyMishka
{
    public static class ArrayExtensions
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        

       

    }
}