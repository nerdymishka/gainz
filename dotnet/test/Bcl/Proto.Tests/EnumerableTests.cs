

using System.Diagnostics.CodeAnalysis;
using NerdyMishka;
using Xunit;

namespace Tests
{
    [Unit]
    [Trait("tag", "unit")]
    public class EnumerableTests
    {

        [Fact]
        public void EqualTo_WithComparer()
        {
            var listA = new [] { "alpha", "beta", "gamma", "delta" };
            var listB = new [] { "apple", "bannana", "grape", "date" };

            var isEqual = listA.EqualTo(listB, (left, right) =>  {
                return left[0].CompareTo(right[0]);
            });

            Assert.True(isEqual);


            isEqual = listA.EqualTo(listB, (left, right) =>  {
                return left.CompareTo(right);
            });

            Assert.False(isEqual);
        }

         [Fact]
        public void EqualTo_WithIComparer()
        {
            var listA = new [] { "alpha", "beta", "gamma", "delta" };
            var listB = new [] { "apple", "bannana", "grape", "date" };

            var isEqual = listA.EqualTo(listB, new FirstLetterComparer());

            Assert.True(isEqual);


            isEqual = listA.EqualTo(listB, new StringComparer());

            Assert.False(isEqual);
        }



        [Fact]
        public void EqualTo()
        {
            var listA = new [] { "alpha", "beta", "gamma", "delta" };
            var listB = new [] { "apple", "bannana", "grape", "date" };
            var listC = new string[4];
            listA.CopyTo(listC, 0);

            var isEqual = listA.EqualTo(listB);
            Assert.False(isEqual);

            isEqual = listA.EqualTo(listC);
            Assert.True(isEqual);  
        }

        public class FirstLetterComparer : System.Collections.Generic.IComparer<string>
        {
            public int Compare([AllowNull] string x, [AllowNull] string y)
            {
                return x[0].CompareTo(y[0]);
            }
        }

        public class StringComparer : System.Collections.Generic.IComparer<string>
        {
            public int Compare([AllowNull] string x, [AllowNull] string y)
            {
                return x.CompareTo(y);
            }
        }
    }
}