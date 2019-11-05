using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{

    [Serializable]
    public class ArgumentEmptyException : Exception
    {
        public ArgumentEmptyException() { }
        public ArgumentEmptyException(string parameterName) : base($"{parameterName} must not be null or empty") { }
        public ArgumentEmptyException(string parameterName, Exception inner) : base($"{parameterName} must not be null or empty", inner) { }
        protected ArgumentEmptyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
