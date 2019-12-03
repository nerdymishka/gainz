using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Properties = NerdyMishka.KeePass.MemoryProtectionProperties;

namespace NerdyMishka.KeePass.Xml
{
    public class MemoryProtectionXmlSerializer : XmlSerializable<MemoryProtection>, IXmlSerializable
    {

        public override string Name
        {
            get
            {
                return "MemoryProtection";
            }
        }

        public MemoryProtectionXmlSerializer(MemoryProtection protection, SerializerContext context)
        {
            this.Model = protection;
            this.Context = context;
        }

       

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;

            if (reader.NodeType != XmlNodeType.Element)
                return;

            switch (reader.Name)
            {
                case Properties.ProtectTitle:
                    reader.Read();
                    m.ProtectedTitle = reader.ReadContentAsStringToBoolean();
                    break;

                case Properties.ProtectUserName:
                    reader.Read();
                    m.ProtectPassword = reader.ReadContentAsStringToBoolean();
                    break;

                case Properties.ProtectPassword:
                    reader.Read();
                    m.ProtectPassword = reader.ReadContentAsStringToBoolean();
                    break;

                case Properties.ProtectUrl:
                    reader.Read();
                    m.ProtectUrl = reader.ReadContentAsStringToBoolean();
                    break;

                case Properties.ProtectNotes:
                    reader.Read();
                    m.ProtectNotes = reader.ReadContentAsStringToBoolean();
                    break;

                default:
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement(Properties.ProtectTitle, m.ProtectedTitle);
            writer.WriteElement(Properties.ProtectUserName, m.ProtectUserName);
            writer.WriteElement(Properties.ProtectPassword, m.ProtectPassword);
            writer.WriteElement(Properties.ProtectUrl, m.ProtectUrl);
            writer.WriteElement(Properties.ProtectNotes, m.ProtectNotes);
        }

      
    }
}
