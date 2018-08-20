using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassAssocationXmlSerializer: XmlSerializable<IKeePassAssociation>, IXmlSerializable
    {

        public KeePassAssocationXmlSerializer(IKeePassAssociation association, SerializerContext context)
        {
            this.Model = association;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "Association";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            switch(reader.Name)
            {
                case "Window":
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Window = reader.ReadContentAsString();
                    }
                    break;

                case "KeystrokeSequence":
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.KeystrokeSequence = reader.ReadContentAsString();
                    }
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement("Window", m.Window);
            writer.WriteElement("KeystrokeSequence", m.KeystrokeSequence);
        }
    }
}
