using System.Collections.Generic;
using System;

namespace NerdyMishka
{
    public static class EnumerableExtensions
    {
        public static bool EqualTo<T>(this IEnumerable<T> left, IEnumerable<T> right)
        {
            if (left == null && right != null)
                return false;

            if (left != null && right == null)
                return false;

            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();

            while(true)
            {
                var lNext = leftEnumerator.MoveNext();
                var rNext = rightEnumerator.MoveNext();

                if (lNext == false && rNext == false)
                    return true;

                if (lNext != rNext)
                    return false;

                var lValue = leftEnumerator.Current;
                var rValue = rightEnumerator.Current;

                var lEquatable = lValue as IEquatable<T>;
                if (lEquatable != null && !lEquatable.Equals(rValue))
                    return false;

                var lComparable = lValue as IComparable<T>;
                if (lComparable != null && lComparable.CompareTo(rValue) != 0)
                    return false;

                if (!lValue.Equals(rValue))
                    return false;
            }
        }
    }
}