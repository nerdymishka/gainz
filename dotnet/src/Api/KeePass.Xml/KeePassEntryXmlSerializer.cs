using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassEntryXmlSerializer : XmlSerializable<IKeePassEntry>, IXmlSerializable
    {
        private Type entryType;
         

        public KeePassEntryXmlSerializer(IKeePassEntry entry, SerializerContext context)
        {
            this.Model = entry;
            this.Context = context;
            this.entryType = context.Mappings[typeof(IKeePassEntry)];
        }

        public override string Name
        {
            get
            {
                return "Entry";
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
         
            writer.WriteElement(KeePassEntryProperties.Uuid, m.Uuid);
            writer.WriteElement(KeePassEntryProperties.IconId, m.IconId);

            if (m.CustomIconUuid != null && m.CustomIconUuid.Length > 0)
                writer.WriteElement(KeePassEntryProperties.CustomIconUuid, Convert.ToBase64String(m.CustomIconUuid));
            
            writer.WriteElement(KeePassEntryProperties.ForegroundColor, m.ForegroundColor);
            writer.WriteElement(KeePassEntryProperties.BackgroundColor, m.BackgroundColor);
            writer.WriteElement(KeePassEntryProperties.OverrideUrl, m.OverrideUrl);

            var tags = "";
            if (m.Tags.Count > 0)
                tags = string.Join(";", m.Tags);

            writer.WriteElement(KeePassEntryProperties.Tags, tags);

            var tx = new AuditFieldsXmlSerializer(m.AuditFields, this.Context);
            tx.WriteXml(writer);
           

            foreach(var ps in m.Strings)
            {
                var psx = new ProtectedStringXmlSerializer(ps, this.Context);
                psx.WriteXml(writer);
            }

            if(m.AutoType != null)
            {
                var axs = new KeePassAutoTypeXmlSerializer(m.AutoType, this.Context);
                axs.WriteXml(writer);
            }

            if(m.History != null && m.History.Count > 0)
            {
                writer.WriteStartElement("History");

                foreach(var entry in m.History)
                {
                    var ex = new KeePassEntryXmlSerializer(entry, this.Context);
                    ex.WriteXml(writer);
                }

                writer.WriteEndElement();
            }

            if(m.Binaries != null && m.Binaries.Count > 0)
            {
                foreach(var binary in m.Binaries)
                {
                    writer.WriteStartElement("Binary");
                    writer.WriteStartElement("Key");
                    writer.WriteString(binary.Key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Value");
                    writer.WriteAttributeString("Ref", binary.Ref.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
               
            }
        }

      

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            IXmlSerializable serializer;
            switch(reader.Name)
            {
                case KeePassEntryProperties.Uuid:
                    reader.Read();
                    m.Uuid = reader.ReadContentAsBytes();
                    break;

                case KeePassEntryProperties.IconId:
                    reader.Read();
                    m.IconId = reader.ReadContentAsInt();
                    break;

                case KeePassEntryProperties.CustomIconUuid:
                    reader.Read();
                    m.CustomIconUuid = Convert.FromBase64String(reader.ReadContentAsString());
                    break;

                case KeePassEntryProperties.ForegroundColor:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.ForegroundColor = reader.ReadContentAsString();
                    }
                    break;

                case KeePassEntryProperties.BackgroundColor:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.BackgroundColor = reader.ReadContentAsString();
                    }
                    break;

                case KeePassEntryProperties.OverrideUrl:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        m.OverrideUrl = reader.ReadContentAsString();
                    }
                    break;

                case KeePassEntryProperties.Tags:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        var tags = reader.ReadContentAsString();
                        if (tags != null)
                        {
                            foreach(var tag in tags.Split(';'))
                            {
                                m.Tags.Add(tag);
                            }
                        }
                    }
                    break;

                case KeePassEntryProperties.Times:
                    serializer = new AuditFieldsXmlSerializer(m.AuditFields, this.Context);
                    serializer.ReadXml(reader);

                    //reader.Read();
                    break;

                case KeePassEntryProperties.String:
                    var protectedString = new ProtectedString();
                    m.Strings.Add(protectedString);
                    serializer = new ProtectedStringXmlSerializer(protectedString, this.Context);
                    serializer.ReadXml(reader);
                    //reader.Read();
                    break;

                case KeePassEntryProperties.Binary:
                    var binary = new ProtectedBinary();
                    m.Binaries.Add(binary);

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            switch (reader.Name)
                            {
                                case "Key":
                                    if (reader.IsEmptyElement)
                                        continue;
                                    reader.Read();
                                    binary.Key = reader.ReadContentAsString();
                                    var name = reader.Name;
                                    break;
                                case "Value":
                                    
                                    if (reader.HasAttributes)
                                    {
                                        var refValue = reader.GetAttribute("Ref");
                                        binary.Ref = int.Parse(refValue);
                                    }

                                    



                                    break;
                            }
                        }

                        if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Binary")
                            break;
                    }

                    var mapping = this.Context.Binaries.SingleOrDefault(o => o.Id == binary.Ref);
                    if (mapping != null)
                    {
                        mapping.Key = binary.Key;
                        binary.Value = mapping.Value;
                    }


                    break;

                case KeePassEntryProperties.AutoType:
                    serializer = new KeePassAutoTypeXmlSerializer(m.AutoType, this.Context);
                    serializer.ReadXml(reader);
                    //reader.Read();
                    break;

                   
                case KeePassEntryProperties.History:
                    if (reader.IsEmptyElement)
                        return;
                    while(reader.Read())
                    {
                        if(reader.NodeType == XmlNodeType.Element && reader.Name == "Entry")
                        {
                            if (reader.IsEmptyElement)
                                continue;


                            var entry = (IKeePassEntry)Activator.CreateInstance(this.entryType);
                            entry.IsHistorical = true;
                            serializer = new KeePassEntryXmlSerializer(entry, this.Context);
                            serializer.ReadXml(reader);
                            m.History.Add(entry);
                        }

                        if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "History")
                        {
                            return;
                        }
                    }
                   
                        
                   
                   
                   
                    break; 
            }
        }
    }
}
