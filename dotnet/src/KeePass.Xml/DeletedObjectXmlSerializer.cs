using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class DeletedObjectXmlSerializer : XmlSerializable<DeletedObjectInfo>, IXmlSerializable
    {

        public DeletedObjectXmlSerializer(DeletedObjectInfo model, SerializerContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "DeletedObject";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            switch(reader.Name)
            {
                case "UUID":
                    reader.Read();
                    m.Uuid = reader.ReadContentAsBytes();
                    break;

                case "DeletionTime":
                    reader.Read();
                    m.DeletionTime = reader.ReadContentAsStringToDateTime();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement("UUID", m.Uuid);
            writer.WriteElement("DeletionTime", m.DeletionTime);
        }
    }
}
