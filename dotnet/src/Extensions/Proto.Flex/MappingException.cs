namespace NerdyMishka.Extensions.Flex 
{
    [System.Serializable]
    public class MappingException : System.Exception
    {
        public MappingException() { }
        public MappingException(string message) : base(message) { }
        public MappingException(string message, System.Exception inner) : base(message, inner) { }
        protected MappingException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}