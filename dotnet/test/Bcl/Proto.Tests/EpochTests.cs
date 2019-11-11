using System;
using Xunit;
using NerdyMishka;

namespace Tests 
{
    [Unit]
    [Trait("tag", "unit")]
    public class EpochTests
    {
        [Fact]
        public static void Constructor()
        { 
            var dt = new DateTime(2018, 12, 9, 11, 51, 33, 150, DateTimeKind.Utc);
            var epoch = new Epoch(dt);
            Assert.Equal(2018, epoch.Year);
            Assert.Equal(12, epoch.Month);
            Assert.Equal(9, epoch.Day);
            Assert.Equal(150, epoch.Millisecond);
            Assert.Equal(33, epoch.Second);
            Assert.Equal(51, epoch.Minute);
            Assert.Equal(11, epoch.Hour);
        }


        [Fact]
        public static void EpochToDateTime()
        {
            var epoch = new Epoch(1541571473);
            var dt = epoch.ToDateTime();

            // no conversion to datetime.
            Assert.Equal(2018, dt.Year);
            Assert.Equal(11, dt.Month);
            Assert.Equal(7, dt.Day);
            Assert.Equal(2018, dt.Year);
            Assert.Equal(6, dt.Hour);
            Assert.Equal(17, dt.Minute);
            Assert.Equal(53, dt.Second);
            Assert.Equal(0, dt.Millisecond);
        }


        [Fact]
        public static void GetDatePart()
        {
            var dt = new DateTime(2018, 12, 9, 11, 51, 33, 450, DateTimeKind.Utc);
            Epoch epoch = dt;
            Assert.Equal(2018, epoch.Year);
            Assert.Equal(12, epoch.Month);
            Assert.Equal(9, epoch.Day);
        }

        [Fact]
        public static void GetTimePart()
        {
            var dt = new DateTime(2018, 12, 9, 11, 51, 33, 450, DateTimeKind.Utc);
            Epoch epoch = dt;
            Assert.Equal(11, epoch.Hour);
            Assert.Equal(51, epoch.Minute);
            Assert.Equal(33, epoch.Second);
            Assert.Equal(450, epoch.Millisecond);
        }

        [Fact]
        public static void TimestampToEpochObject()
        {
            var epoch = new Epoch(1541571473);

            // no conversion to datetime.
            Assert.Equal(2018, epoch.Year);
            Assert.Equal(11, epoch.Month);
            Assert.Equal(7, epoch.Day);
            Assert.Equal(2018, epoch.Year);
            Assert.Equal(6, epoch.Hour);
            Assert.Equal(17, epoch.Minute);
            Assert.Equal(53, epoch.Second);
            Assert.Equal(0, epoch.Millisecond);
        }
    }
}