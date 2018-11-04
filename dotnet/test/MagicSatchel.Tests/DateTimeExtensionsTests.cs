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

            var epoc1 = date.ToUnixTimeStamp();
            
            Assert.Equal(1541313529000, epoc1);
        }

        [Fact]
        public void ToUtcUnixTimeStamp_Test()
        {
             // November 4, 2018 6:38:49
            var date = new DateTime(2018, 11, 4, 6, 38, 49, DateTimeKind.Utc);
            var local = date.ToLocalTime();

            Assert.NotEqual(local, date);

            var epoc1 = local.ToUtcUnixTimeStamp();
            
            Assert.Equal(1541313529000, epoc1);
            
            var epoc2 = date.ToUtcUnixTimeStamp();

            Assert.Equal(1541313529000, epoc2);
        } 
    }
}
