/**
 * Copyright 2016 Nerdy Mishka
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
using System.Collections.Generic;

namespace NerdyMishka
{
    internal class Check
    {
        internal static T NotNull<T>(string parameterName, T value)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }

        internal static void Range<T>(string parameterName, IList<T> value, int start, int count)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start", "The parameter, start, must be 0 or greater.");

            if (start >= value.Count)
                throw new ArgumentOutOfRangeException("start", string.Format("The parameter, start, must not be equal or greater than {0}'s length.", parameterName));

            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "The parameter, count, must be 0 or greater.");

            if (count > value.Count)
                throw new ArgumentOutOfRangeException("count", string.Format("The parameter, count, must not be greater than {0}'s length.", parameterName));

            if (start + count > value.Count)
                throw new ArgumentOutOfRangeException("value,count", string.Format("The sum of start and count must not be greater than {0}'s length", parameterName));
        }
    }
}