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
using System.Runtime.CompilerServices;


namespace NerdyMishka.Validation 
{
    public static class Check
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T NotNull<T>(string parameterName, T value)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IList<T> NotNullOrEmpty<T>(string parameterName, IList<T> value)
        {
            NotNull(parameterName, value);

            if(value.Count == 0)
                throw new NerdyMishka.Validation.ArgumentNullOrEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullOrEmpty<T>(string parameterName, string value)
        {
            NotNull(parameterName, value);

            if(value.Length == 0)
                throw new ArgumentNullOrEmptyException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string NotNullOrWhitespace<T>(string parameterName, string value)
        {
            NotNull(parameterName, value);

            if(value.Length == 0)
                throw new ArgumentNullOrWhitespaceException(parameterName);

            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Range<T>(string parameterName, T value, T min, T max) where T:struct, IComparable<T>
        {
            if (value.CompareTo(min) == -1)
                throw new ArgumentOutOfRangeException(parameterName, $"The parameter {parameterName} must not be less than {min}.");

            if (value.CompareTo(max) == 1)
                throw new ArgumentOutOfRangeException(parameterName, $"Argument {parameterName} must not be greater than {max}");

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Range<T>(string parameterName, IList<T> value, int start, int count)
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