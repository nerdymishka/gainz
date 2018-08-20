using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassDocumentXmlSerializer : XmlSerializable<IKeePassDocument>, IXmlSerializable
    {
        private Type groupType;
        public KeePassDocumentXmlSerializer(IKeePassDocument model, SerializerContext context)
        {
            this.groupType = context.Mappings[typeof(IKeePassGroup)];
            this.Model = model;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "Root";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            IXmlSerializable serializer = null;
            switch (reader.Name)
            {
                case KeePassGroupProperties.Group:

                    var group = (IKeePassGroup)Activator.CreateInstance(this.groupType);
                    m.Add(group);
                    serializer = new KeePassGroupXmlSerializer(group, this.Context);
                    serializer.ReadXml(reader);
                    //reader.Read();
                    break;

                case "DeletedObjects":
                    var deletedObject = new DeletedObjectInfo();
                    m.DeletedObjects.Add(deletedObject);
                    serializer = new DeletedObjectXmlSerializer(deletedObject, this.Context);
                    serializer.ReadXml(reader);
                    //reader.Read();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
          

            if (m.Groups != null)
            {
                foreach (var group in m.Groups)
                {
                    var gx = new KeePassGroupXmlSerializer(group, this.Context);
                    gx.WriteXml(writer);
                }
            }

            if(m.DeletedObjects != null && m.DeletedObjects.Count > 0)
            {
                foreach(var deletedObject in m.DeletedObjects)
                {
                    var dox = new DeletedObjectXmlSerializer(deletedObject, this.Context);
                    dox.WriteXml(writer);
                }
            }
        }
    }
}
