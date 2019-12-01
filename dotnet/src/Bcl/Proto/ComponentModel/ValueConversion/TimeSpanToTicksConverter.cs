

using System;

namespace NerdyMishka.ComponentModel.ValueConversion
{
    public class TimeSpanToTicksConverter : ValueConverter<TimeSpan, long>
    {
        

        public TimeSpanToTicksConverter() :base((from) => from.Ticks, (to) => new TimeSpan(to))
        {
            
        }

        
    }
}