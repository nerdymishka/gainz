using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassGroupXmlSerializer : XmlSerializable<IKeePassGroup>, IXmlSerializable
    {
        private Type entryType;
        private Type groupType;

        public KeePassGroupXmlSerializer(IKeePassGroup group, SerializerContext context)
        {
            this.Model = group;
            this.Context = context;
            this.entryType = context.Mappings[typeof(IKeePassEntry)];
            this.groupType = context.Mappings[typeof(IKeePassGroup)];
        }

        public override string Name
        {
            get
            {
                return "Group";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            IXmlSerializable serializer = null;
            switch(reader.Name)
            {
                case KeePassGroupProperties.Uuid:
                   reader.Read();
                    m.Uuid = reader.ReadContentAsBytes();
                    
                    break;

                case KeePassGroupProperties.Name:
                    if(!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Name = reader.ReadContentAsString();
                    }
                 
                    break;

                case KeePassGroupProperties.Notes:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.Notes = reader.ReadContentAsString();
                    }
                    break;

                case KeePassGroupProperties.IconId:
                   reader.Read();
                    m.IconId = reader.ReadContentAsInt();
                    break;

                case KeePassGroupProperties.CustomIconUuid:
                    reader.Read();
                    m.CustomIconUuid = Convert.FromBase64String(reader.ReadContentAsString());
                    break;

                case KeePassGroupProperties.Times:
                    serializer = new AuditFieldsXmlSerializer(m.AuditFields, this.Context);
                    serializer.ReadXml(reader);
                    break;

                case KeePassGroupProperties.IsExpanded:
                   reader.Read();
                    m.IsExpanded = reader.ReadContentAsStringToBoolean();
                    break;

                case KeePassGroupProperties.DefaultAutoTypeSequence:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.DefaultAutoTypeSequence = reader.ReadContentAsString();
                    }
                    break;

                case KeePassGroupProperties.EnableAutoType:
                   reader.Read();
                    m.EnableAutoType = reader.ReadContentAsNullableBoolean();
                    break;

                case KeePassGroupProperties.EnableSearching:
                   reader.Read();
                    m.EnableSearching = reader.ReadContentAsNullableBoolean();
                    break;

                case KeePassGroupProperties.LastTopVisibleEntry:
                    reader.Read();
                    m.LastTopVisibleEntry = reader.ReadContentAsBytes();
                    break;


                case KeePassGroupProperties.Entry:
                    var entry = (IKeePassEntry)Activator.CreateInstance(this.entryType);
                    m.Add(entry);
                    serializer = new KeePassEntryXmlSerializer(entry, this.Context);
                    serializer.ReadXml(reader);
                    // reader.Read();
                    break;

                case KeePassGroupProperties.Group:
                    var group = (IKeePassGroup)Activator.CreateInstance(this.groupType);
                    m.Add(group);
                    serializer = new KeePassGroupXmlSerializer(group, this.Context);
                    serializer.ReadXml(reader);
                    // reader.Read();
                    break;

            }
        }

     

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement(KeePassGroupProperties.Uuid, m.Uuid);


            writer.WriteElement(KeePassGroupProperties.Name, m.Name);
            writer.WriteElement(KeePassGroupProperties.Notes, m.Notes);
            writer.WriteElement(KeePassGroupProperties.IconId, m.IconId);

            if (m.CustomIconUuid != null && m.CustomIconUuid.Length > 0)
                writer.WriteElement(KeePassGroupProperties.CustomIconUuid, Convert.ToBase64String(m.CustomIconUuid));

            var tx = new AuditFieldsXmlSerializer(m.AuditFields, this.Context);
            tx.WriteXml(writer);

            writer.WriteElement(KeePassGroupProperties.IsExpanded, m.IsExpanded);
            writer.WriteElement(KeePassGroupProperties.DefaultAutoTypeSequence, m.DefaultAutoTypeSequence);
            writer.WriteElement(KeePassGroupProperties.EnableAutoType, m.EnableAutoType);
            writer.WriteElement(KeePassGroupProperties.EnableSearching, m.EnableSearching);
            writer.WriteElement(KeePassGroupProperties.LastTopVisibleEntry, m.LastTopVisibleEntry);

            if(m.Entries != null)
            {
                foreach(var entry in m.Entries)
                {
                    var ex = new KeePassEntryXmlSerializer(entry, this.Context);
                    ex.WriteXml(writer);
                }
            }


            if(m.Groups != null)
            {
                foreach(var group in m.Groups)
                {
                    var gx = new KeePassGroupXmlSerializer(group, this.Context);
                    gx.WriteXml(writer);
                }
            }
        }
    }
}
