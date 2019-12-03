using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace NerdyMishka.KeePass
{
    public static class XmlUtil
    {
       

        public static byte[] ReadContentAsBytes(this XmlReader reader)
        {
            var value = reader.ReadContentAsString();
            var bytes = Convert.FromBase64String(value);
            return bytes;
        }

        public static DateTime ReadContentAsStringToDateTime(this XmlReader reader)
        {
            var dateString = reader.ReadContentAsString();
            DateTime datetime;
            if (DateTime.TryParse(dateString, out datetime))
                return datetime;

            return DateTime.MinValue;
        }


        public static bool? ReadContentAsNullableBoolean(this XmlReader reader)
        {
            var boolString = reader.ReadContentAsString();
            boolString = boolString.Trim().ToLower();
            if (boolString == null)
                return null;

            switch (boolString)
            {
                case "yes":
                case "true":
                case "1":
                    return true;
            }

            return false;
        }

        public static bool ReadContentAsStringToBoolean(this XmlReader reader)
        {
            if (!reader.HasValue)
                return false;

            var boolString = reader.ReadContentAsString();
            if (boolString == null)
                return false;

            boolString = boolString.Trim().ToLower();
           

            switch(boolString)
            {
                case "yes":
                case "true":
                case "1":
                    return true;
            }

            return false;
        }

        public static void WriteElement(this XmlWriter writer, string name, byte[] bytes)
        {
            writer.WriteStartElement(name);
            writer.WriteBase64(bytes, 0, bytes.Length);
            writer.WriteEndElement();
        }

        public static void WriteElement(this XmlWriter writer, string name, int value)
        {
            WriteElement(writer, name, value.ToString());
        }

        public static void WriteElement(this XmlWriter writer, string name, bool? value)
        {
            if(!value.HasValue)
            {
                WriteElement(writer, name, "null");
                return;
            }
            WriteElement(writer, name, value.Value);
        }

        public static void WriteElement(this XmlWriter writer, string name, bool value)
        {
            WriteElement(writer, name, value ? "True" : "False");
        }

        public static void WriteElement(this XmlWriter writer, string name, DateTime value)
        {
            var content = value.ToUniversalTime().ToString("s");
            if (!content.EndsWith("Z"))
                content += "Z";

            WriteElement(writer, name, content);
        }

        public static void WriteElement(this XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement(name);
            if(value != null)
                writer.WriteString(value);
            writer.WriteEndElement();
        }
    }
}
