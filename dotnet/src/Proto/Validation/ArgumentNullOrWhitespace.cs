
namespace NerdyMishka.Validation 
{

   [System.Serializable]
   public class ArgumentNullOrWhiteSpaceException : System.Exception
   {
        public string ParameterName { get; private set; }

        public ArgumentNullOrWhiteSpaceException() { }


        public ArgumentNullOrWhiteSpaceException(string parameterName) 
            :base($"Argument {parameterName} must not be null and must not be empty WhiteSpace")
        { 
           this.ParameterName = parameterName;
        }


        public ArgumentNullOrWhiteSpaceException(string parameterName, string message) 
            :base(message)
        { 
           this.ParameterName = parameterName;
        }

        public ArgumentNullOrWhiteSpaceException(string parameterName, string message, System.Exception innerException) 
            :base(message, innerException)
        { 
           this.ParameterName = parameterName;
        }

       public ArgumentNullOrWhiteSpaceException(string parameterName, System.Exception innerException) 
            :base($"Argument {parameterName} must not be null and must not be empty WhiteSpace", innerException)
       { 
           this.ParameterName = parameterName;
       }
     
       protected ArgumentNullOrWhiteSpaceException(
           System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}
