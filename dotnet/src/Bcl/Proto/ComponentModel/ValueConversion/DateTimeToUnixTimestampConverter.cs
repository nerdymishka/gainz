

using System;

namespace NerdyMishka.ComponentModel.ValueConversion
{
    public class DateTimeToUnixTimestampConverter : ValueConverter<DateTime, long>
    {
        

        public DateTimeToUnixTimestampConverter() : 
            base((from) => from.ToUnixTimeStamp(), (to) => to.FromUnixTimeStamp())
        {
            
        }
    }
}