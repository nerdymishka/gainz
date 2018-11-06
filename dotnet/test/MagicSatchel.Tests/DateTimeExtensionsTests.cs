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
        public void Epoc_GetDatePart()
        {
            var dt = new DateTime(2018, 12, 09, 11, 51, 33);
            Epoc epoc = dt;
            int year = 0;
            int month = 0;
            int day = 0;
            epoc.GetDatePart(out year, out month, out day);

            Assert.Equal(2018, year);
            Assert.Equal(12, month);
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
