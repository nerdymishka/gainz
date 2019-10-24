
namespace NerdyMishka.Validation 
{

   [System.Serializable]
   public class ArgumentNullOrWhitespaceException : System.Exception
   {
        public string ParameterName { get; private set; }

        public ArgumentNullOrWhitespaceException() { }


        public ArgumentNullOrWhitespaceException(string parameterName) 
            :base($"Argument {parameterName} must not be null and must not be empty whitespace")
        { 
           this.ParameterName = parameterName;
        }


        public ArgumentNullOrWhitespaceException(string parameterName, string message) 
            :base(message)
        { 
           this.ParameterName = parameterName;
        }

        public ArgumentNullOrWhitespaceException(string parameterName, string message, System.Exception innerException) 
            :base(message, innerException)
        { 
           this.ParameterName = parameterName;
        }

       public ArgumentNullOrWhitespaceException(string parameterName, System.Exception innerException) 
            :base($"Argument {parameterName} must not be null and must not be empty whitespace", innerException)
       { 
           this.ParameterName = parameterName;
       }
     
       protected ArgumentNullOrWhitespaceException(
           System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
   }
}
