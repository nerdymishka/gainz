using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public static class DateTimeUtils
    {


        public static DateTime ToShortDate(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
