

using NerdyMishka;
using Xunit;

namespace Tests
{
    //[Unit]
    [Trait("tag", "unit")]
    public class DotNetRandomTests : PsuedoRandomTestCase
    {
        protected override IRandom CreateRng(int? seed = null)
        {
            if(seed == null)
                return new DotNetRandom();

            return new DotNetRandom(seed.Value);
        }
    }
}