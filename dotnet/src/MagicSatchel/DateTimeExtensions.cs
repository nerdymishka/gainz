/**
 * Copyright 2016 Nerdy Mishka LLC
 * Based Upon Lucene from The Apache Foundation, Copyright 2004
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Text;

namespace NerdyMishka 
{
    public static class DateTimeExtensions
    {
        

        public static DateTime FromUnixTimeStamp(this long value)
        {
            var ticks = (value * 1000) + 621355968000000000;
            return new DateTime(ticks);
        }

        /// <summary>
        /// Transforms the <paramref name="value"/> to UTC and then converts it into 
        /// the Unix TimeStamp format.
        /// </summary>
        /// <param name="value">the value to be converted.</param>
        /// <returns>The <see cref="Int64"/> representation of time.</returns>
        public static long ToUtcUnixTimeStamp(this DateTime value)
        {
            value = value.ToUniversalTime();
            return (value.Ticks - 621355968000000000) / 10000;
        }

        /// <summary>
        /// Converts the time into the Unix TimeStamp format. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToUnixTimeStamp(this DateTime value)
        {
            return (value.Ticks - 621355968000000000) / 10000;
        }
    }
}
