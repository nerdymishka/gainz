
using System;

namespace NerdyMishka
{
    internal static class Check
    {
        public static T NullParamenter<T>(string parameterName, T value)  where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }
    }
}