

using System;

namespace NerdyMishka.ComponentModel.ValueConversion
{
    public class DateTimeToTicksConverter : ValueConverter<DateTime, long>
    {
        

        public DateTimeToTicksConverter() :base((from) => from.Ticks, (to) => new DateTime(to))
        {
            
        }
    }
}