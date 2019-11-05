using System;

namespace NerdyMishka.Validation 
{

   [System.Serializable]
   public class ArgumentNullOrEmptyException : System.Exception
   {
        public string ParameterName { get; private set; }

        public ArgumentNullOrEmptyException() { }


        public ArgumentNullOrEmptyException(string parameterName) 
            :base($"Argument {parameterName} must not be null or empty")
        { 
           this.ParameterName = parameterName;
        }


        public ArgumentNullOrEmptyException(string parameterName, string message) 
            :base(message)
        { 
           this.ParameterName = parameterName;
        }

        public ArgumentNullOrEmptyException(string parameterName, string message, System.Exception innerException) 
            :base(message, innerException)
        { 
           this.ParameterName = parameterName;
        }

       public ArgumentNullOrEmptyException(string parameterName, System.Exception innerException) 
            :base($"Argument {parameterName} must not be null or empty", innerException)
       { 
           this.ParameterName = parameterName;
       }
     
       protected ArgumentNullOrEmptyException(
           System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}
