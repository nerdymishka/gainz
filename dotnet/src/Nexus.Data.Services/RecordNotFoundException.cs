namespace Nexus.Services
{

    public class RecordNotFoundException : System.Exception
    {
        public RecordNotFoundException(string type, long id): 
            this($"Record {type}({id}) does not exist.") {

        }

        public RecordNotFoundException(string type, int id): 
            this($"Record {type}({id}) does not exist.") {

        }
       
        public RecordNotFoundException(string message) : base(message) { }
        public RecordNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected RecordNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}