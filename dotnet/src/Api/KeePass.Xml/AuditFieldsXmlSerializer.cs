using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class AuditFieldsXmlSerializer : XmlSerializable<IKeePassAuditFields>, IXmlSerializable
    {

        public AuditFieldsXmlSerializer(IKeePassAuditFields time, SerializerContext context)
        {
            this.Model = time;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "Times";
            }
        }


        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            switch (reader.Name)
            {
                case KeePassAuditFieldProperties.CreationTime:
                    reader.Read();
                    m.CreationTime = reader.ReadContentAsStringToDateTime();
                    break;
                case KeePassAuditFieldProperties.LastModificationTime:
                    reader.Read();
                    m.LastModificationTime = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassAuditFieldProperties.LastAccessTime:
                    reader.Read();
                    m.LastAccessTime = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassAuditFieldProperties.ExpiryTime:
                    reader.Read();
                    m.ExpiryTime = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassAuditFieldProperties.Expires:
                    reader.Read();
                    m.Expires = reader.ReadContentAsStringToBoolean();
                    break;

                case KeePassAuditFieldProperties.UsageCount:
                    reader.Read();
                    m.UsageCount = reader.ReadContentAsInt();
                    break;

                case KeePassAuditFieldProperties.LocationChanged:
                    reader.Read();
                    m.LocationChanged = reader.ReadContentAsStringToDateTime();
                    break;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement(KeePassAuditFieldProperties.CreationTime, m.CreationTime);
            writer.WriteElement(KeePassAuditFieldProperties.LastModificationTime, m.LastModificationTime);
            writer.WriteElement(KeePassAuditFieldProperties.LastAccessTime, m.LastAccessTime);
            writer.WriteElement(KeePassAuditFieldProperties.ExpiryTime, m.ExpiryTime);
            writer.WriteElement(KeePassAuditFieldProperties.Expires, m.Expires);
            writer.WriteElement(KeePassAuditFieldProperties.UsageCount, m.UsageCount);
            writer.WriteElement(KeePassAuditFieldProperties.LocationChanged, m.LocationChanged);
        }
    }
}
