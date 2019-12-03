using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public static class Check
    {

        public static string NotEmpty(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new NullReferenceException($"{name} must not be null or empty.");

            return value;
        }

        public static bool Equal<T>(ICollection<T> left, ICollection<T> right)
        {
            if (left == null && right != null)
                return false;

            if (left != null && right == null)
                return false;

            if (left.Count != right.Count)
                return false;

            var leftEnumerator = left.GetEnumerator();
            var rightEnumerator = right.GetEnumerator();

            while(leftEnumerator.MoveNext() && rightEnumerator.MoveNext())
            {
                if (!leftEnumerator.Current.Equals(rightEnumerator.Current))
                    return false;
            }

            return true;
        }
    }
}
