using System;

namespace Nexus.Data
{


    public class InvalidValueException : Exception 
    {
        public string Kind { get; private set; }

        public string Name { get; private set; }

        public string Contraint { get; private set; }

        public object Value {get; private set; }

        private static string TrimValue(object value)
        {
            if(value == null)
                return "NULL";

            if(value is bool)
                return (bool)value ? "True" : "False";

            var v = value.ToString();
            if(v.Length > 50)
                v = v.Substring(0, 50) + "...";

            return v;
        }

        public InvalidValueException(string name, object value, string constraint) 
            : base (string.Format($"{name} could not accept the value \"{TrimValue(value)}`\" because it must {constraint}")) 
        {

        }

        public static InvalidValueException NotEmpty(string name, string value) {
            return new InvalidValueException(name, value, " not be null or empty");
        }

        public static InvalidValueException Unique(string name, string value) {
            return new InvalidValueException(name, value, $" be unique. {TrimValue(value)} already exists");
        }

        public static InvalidValueException LessThan(string name, long value, long constraint = 0) {
            return new InvalidValueException(name, value, $"not be less than {constraint}");
        }
    }
}