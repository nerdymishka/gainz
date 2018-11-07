using System;
using Xunit;

namespace NerdyMishka.MagicSatchel.Tests
{
    public class DateTimeExtensionTests
    {
        [Fact]
        public void ToUnixTimeStamp_Test()
        {
            // November 4, 2018 6:38:49
            var date = new DateTime(2018, 11, 4, 6, 38, 49, DateTimeKind.Utc);
            var dt2 = date.ToLocalTime();

            // should enforce convertsion to UTC.
            var epoc1 = dt2.ToUnixTimeStamp();
            
            Assert.Equal(1541313529000, epoc1);
        }

        

         
    }
}
