using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{

    [Serializable]
    public class InvalidFlexPrimitiveCastException : Exception
    {
       

        public InvalidFlexPrimitiveCastException() { }
        public InvalidFlexPrimitiveCastException(Type to)
            : base($"Cannot convert NULL to {to.FullName}")
        {

        }

        public InvalidFlexPrimitiveCastException(Type from, Type to)
           : base($"Cannot convert {from.FullName} to {to.FullName}")
        {

        }
       
        protected InvalidFlexPrimitiveCastException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

   
}
