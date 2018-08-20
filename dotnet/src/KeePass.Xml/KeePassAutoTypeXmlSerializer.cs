using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassAutoTypeXmlSerializer : XmlSerializable<IKeePassAutoType>, IXmlSerializable
    {

        public KeePassAutoTypeXmlSerializer(IKeePassAutoType model, SerializerContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "AutoType";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            IXmlSerializable serializer = null;
            switch(reader.Name)
            {
                case "Enabled":
                    reader.Read();
                    m.Enabled = reader.ReadContentAsStringToBoolean();
                    break;
                case "DataTransferObfuscation":
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.DataTransferObfuscation = reader.ReadContentAsInt();
                    }
                    break;

                case "Assocation":
                    serializer = new KeePassAssocationXmlSerializer(m.Association, this.Context);
                    serializer.ReadXml(reader);

                    // reader.Read();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement("Enabled", m.Enabled);
            writer.WriteElement("DataTransferObfuscation", m.DataTransferObfuscation);

            var ax = new KeePassAssocationXmlSerializer(m.Association, this.Context);
            ax.WriteXml(writer);
        }
    }
}
