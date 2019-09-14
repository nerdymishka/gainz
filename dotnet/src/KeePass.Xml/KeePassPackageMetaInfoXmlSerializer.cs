using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassPackageMetaInfoXmlSerializer : XmlSerializable<KeePassPackageMetaInfo>, IXmlSerializable
    {

        public override string Name
        {
            get
            {
                return "Meta";
            }
        }

        public KeePassPackageMetaInfoXmlSerializer(KeePassPackageMetaInfo meta, SerializerContext context)
        {
            this.Model = meta;
            this.Context = context;
        }

        

        protected override void WriteProperties(XmlWriter writer)
        {
            writer.WriteElement(KeePassPackageMetaInfoProperties.Generator, this.Model.Generator);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DatabaseName, this.Model.DatabaseName);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DatabaseNameChanged, this.Model.DatabaseNameChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DatabaseDescription, this.Model.DatabaseDescription);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DatabaseDescriptionChanged, this.Model.DatabaseDescriptionChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DefaultUserName, this.Model.DefaultUserName);
            writer.WriteElement(KeePassPackageMetaInfoProperties.DefaultUserNameChanged, this.Model.DefaultUserNameChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.MaintenanceHistoryDays, this.Model.MaintenanceHistoryDays);
            writer.WriteElement(KeePassPackageMetaInfoProperties.Color, this.Model.Color);
            writer.WriteElement(KeePassPackageMetaInfoProperties.MasterKeyChanged, this.Model.MasterKeyChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.MasterKeyChangeRec, this.Model.MasterKeyChangeRec);
            writer.WriteElement(KeePassPackageMetaInfoProperties.MasterKeyChangeForce, this.Model.MasterKeyChangeForce);

            var serializer = new MemoryProtectionXmlSerializer(this.Model.MemoryProtection, this.Context);
            serializer.WriteXml(writer);

            var customIcons = this.Model.CustomIcons;

            if(customIcons != null && customIcons.Count > 0)
            {
                writer.WriteStartElement("CustomIcons");

                foreach(var icon in customIcons)
                {
                    writer.WriteStartElement("Icon");

                    writer.WriteStartElement("UUID");
                    writer.WriteString(Convert.ToBase64String(icon.Uuid));
                    writer.WriteEndElement();

                    writer.WriteStartElement("Data");
                    writer.WriteString(Convert.ToBase64String(icon.Data));
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            writer.WriteElement(KeePassPackageMetaInfoProperties.RecycleBinEnabled, this.Model.RecycleBinEnabled);
            writer.WriteElement(KeePassPackageMetaInfoProperties.RecycleBinUUID,this.Model.RecycleBinUUID);
            writer.WriteElement(KeePassPackageMetaInfoProperties.RecycleBinChanged,this.Model.RecycleBinChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.EntryTemplatesGroup,this.Model.EntryTemplatesGroup);
            writer.WriteElement(KeePassPackageMetaInfoProperties.EntryTemplatesGroupChanged,this.Model.EntryTemplatesGroupChanged);
            writer.WriteElement(KeePassPackageMetaInfoProperties.HistoryMaxItems,this.Model.HistoryMaxItems);
            writer.WriteElement(KeePassPackageMetaInfoProperties.HistoryMaxSize,this.Model.HistoryMaxSize);
            writer.WriteElement(KeePassPackageMetaInfoProperties.LastSelectedGroup,this.Model.LastSelectedGroup);
            writer.WriteElement(KeePassPackageMetaInfoProperties.LastTopVisibleGroup,this.Model.LastTopVisibleGroup);

            var binaries = this.Context.Binaries;
            var customData = this.Model.CustomData;
            writer.WriteStartElement("Binaries");

            if (binaries != null && binaries.Count > 0)
            {
                foreach (var mapping in binaries)
                {
                    writer.WriteStartElement("Binary");
                    writer.WriteAttributeString("ID", mapping.Id.ToString());
                    writer.WriteAttributeString("Protected", "True");


                    var raw = mapping.Value.UnprotectAsBytes();
                    var pad = this.Context.RandomByteGenerator.NextBytes(raw.Length);
                    var bytes = new byte[raw.Length];

                    for (var i = 0; i < raw.Length; i++)
                        bytes[i] = (byte)(raw[i] ^ pad[i]);

                    if (this.Context.DatabaseCompression == (byte)1)
                    {
                        writer.WriteAttributeString("Compressed", "True");
                        bytes = Compress(bytes);
                    }

                    writer.WriteString(Convert.ToBase64String(bytes));
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.WriteStartElement("CustomData");

            if(customData != null && customData.Count > 0)
            {
                foreach(var kp in customData)
                {
                    writer.WriteStartElement("Item");

                    writer.WriteStartElement("Key");
                    writer.WriteString(kp.Key);
                    writer.WriteEndElement();

                    writer.WriteStartElement("Data");
                    writer.WriteString(kp.Value);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }


        private byte[] Compress(byte[] bytes)
        {
            using (var source = new MemoryStream(bytes, false))
            using (var dest = new MemoryStream())
            using (var gz = new GZipStream(dest, CompressionMode.Compress))
            {
                source.CopyTo(gz);
                return dest.ToArray();
            }
        }

        private byte[] Decompress(byte[] bytes)
        {
            using (var source = new MemoryStream(bytes, false))
            using (var gz = new GZipStream(source, CompressionMode.Decompress))
            using (var dest = new MemoryStream())
            {
                gz.CopyTo(dest);
                return dest.ToArray();
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            switch (reader.Name)
            {
                case KeePassPackageMetaInfoProperties.Generator:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.Generator = reader.ReadContentAsString();
                    }
                    break;

                case KeePassPackageMetaInfoProperties.DatabaseName:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DatabaseName = reader.ReadContentAsString();
                    }
                    
                    break;
                  
                case KeePassPackageMetaInfoProperties.DatabaseNameChanged:
                    reader.Read();
                    this.Model.DatabaseNameChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassPackageMetaInfoProperties.DatabaseDescription:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DatabaseDescription = reader.ReadContentAsString();
                    }
                    break;

                case KeePassPackageMetaInfoProperties.DefaultUserName:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.DefaultUserName = reader.ReadContentAsString();
                    }
                    break;

                case KeePassPackageMetaInfoProperties.DefaultUserNameChanged:
                    reader.Read();
                    this.Model.DefaultUserNameChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassPackageMetaInfoProperties.MaintenanceHistoryDays:
                    reader.Read();
                    this.Model.MaintenanceHistoryDays = reader.ReadContentAsInt();
                    break;

                case KeePassPackageMetaInfoProperties.Color:
                    if (!reader.IsEmptyElement)
                    {
                        reader.Read();
                        this.Model.Color = reader.ReadContentAsString();  
                    }
                    break;

                case KeePassPackageMetaInfoProperties.MasterKeyChanged:
                    reader.Read();
                    this.Model.MasterKeyChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassPackageMetaInfoProperties.MasterKeyChangeRec:
                    reader.Read();
                    this.Model.MasterKeyChangeRec = reader.ReadContentAsInt();
                    break;

                case KeePassPackageMetaInfoProperties.MasterKeyChangeForce:
                    reader.Read();
                    this.Model.MasterKeyChangeForce = reader.ReadContentAsInt();
                    break;

                case KeePassPackageMetaInfoProperties.MemoryProtection:
                    var serializer = new MemoryProtectionXmlSerializer(this.Model.MemoryProtection, this.Context);
                    serializer.ReadXml(reader);
                    this.Model.MemoryProtection = serializer.Model;
                    break;

                case KeePassPackageMetaInfoProperties.RecycleBinEnabled:
                    reader.Read();
                    this.Model.RecycleBinEnabled = reader.ReadContentAsStringToBoolean();
                    break;

                case KeePassPackageMetaInfoProperties.RecycleBinUUID:
                    reader.Read();
                    this.Model.RecycleBinUUID = reader.ReadContentAsBytes();
                    break;

                case KeePassPackageMetaInfoProperties.RecycleBinChanged:
                    reader.Read();
                    this.Model.RecycleBinChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassPackageMetaInfoProperties.EntryTemplatesGroup:
                    reader.Read();
                    this.Model.EntryTemplatesGroup = reader.ReadContentAsBytes();
                    break;

                case KeePassPackageMetaInfoProperties.EntryTemplatesGroupChanged:
                    reader.Read();
                    this.Model.EntryTemplatesGroupChanged = reader.ReadContentAsStringToDateTime();
                    break;

                case KeePassPackageMetaInfoProperties.HistoryMaxItems:
                    reader.Read();
                    this.Model.HistoryMaxItems = reader.ReadContentAsInt();
                    break;

                case KeePassPackageMetaInfoProperties.HistoryMaxSize:
                    reader.Read();
                    this.Model.HistoryMaxSize = reader.ReadContentAsInt();
                    break;

                case KeePassPackageMetaInfoProperties.LastSelectedGroup:
                    reader.Read();
                    this.Model.LastSelectedGroup = reader.ReadContentAsBytes();
                    break;

                case KeePassPackageMetaInfoProperties.LastTopVisibleGroup:
                    reader.Read();
                    this.Model.LastSelectedGroup = reader.ReadContentAsBytes();
                   
                    break;

                case KeePassPackageMetaInfoProperties.Binaries:
                    if (reader.IsEmptyElement)
                        return;


                    this.ReadBinaries(reader);
                    break;
                case KeePassPackageMetaInfoProperties.CustomIcons:
                    if (reader.IsEmptyElement)
                        return;

                    this.ReadCustomIcons(reader);

                    break;
                case KeePassPackageMetaInfoProperties.CustomData:
                    if (reader.IsEmptyElement)
                        return;

                    this.ReadCustomData(reader);

                    break;
                default:
                    break;
            }
        }

        private void ReadCustomIcons(XmlReader reader)
        {
            while(reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Icon")
                    this.ReadCustomIcon(reader);

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "CustomIcons")
                {
                    return;
                }
            }
        }

        private void ReadCustomIcon(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            string name = null;
            byte[] uuid = null;
            byte[] data = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {

                    switch (reader.Name)
                    {
                        case "UUID":
                            reader.Read();
                            uuid = Convert.FromBase64String(reader.ReadContentAsString());
                            break;

                        case "Name":
                            reader.Read();
                            name = reader.ReadContentAsString();
                            break;

                        case "Data":
                            reader.Read();
                            data = Convert.FromBase64String(reader.ReadContentAsString());
                            
                            break;

                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Icon")
                {
                    if (this.Model.CustomIcons == null)
                        this.Model.CustomIcons = new List<IKeePassIcon>();
                    this.Model.CustomIcons.Add(new KeePassIcon() { Uuid = uuid, Name = name, Data = data });
                    uuid = null;
                    name = null;
                    data = null;

                    return;
                }
            }
        }

        private void ReadCustomData(XmlReader reader)
        {
            string key = null, value = null;

            while(reader.Read())
            {
                if(reader.NodeType == XmlNodeType.Element)
                {

                    switch(reader.Name)
                    {
                        case "Key":
                            key = reader.ReadContentAsString();
                            break;

                        case "Value":
                            value = reader.ReadContentAsString();
                            this.Model.CustomData.Add(key, value);
                            break;

                    }
                }

                if(reader.NodeType == XmlNodeType.EndElement && reader.Name == "CustomData")
                {
                    return;
                }
            }
            
        }

        private void ReadBinaries(XmlReader reader)
        {
            while(reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Binary")
                    this.ReadBinary(reader);

                if(reader.NodeType == XmlNodeType.EndElement && reader.Name == KeePassPackageMetaInfoProperties.Binaries)
                {
                    return;
                }
            }
        }

        private void ReadBinary(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            bool compressed = false,
                 isProtected = false;

            string id = null,
                   compressedString = null;
            var mapping = new BinaryMapping();
            mapping.Id = this.Model.Binaries.Count;
           

            if(reader.HasAttributes)
            {
                id = reader.GetAttribute("ID");
                compressedString = reader.GetAttribute("Compressed");
                var isProtectedString = reader.GetAttribute("Protected");

                if (compressedString != null && compressedString == "True")
                    compressed = true;

                if (isProtectedString != null & isProtectedString == "True")
                    isProtected = true;
            }

            

            reader.Read();
            var content = reader.ReadContentAsString();
            var bytes = Convert.FromBase64String(content);

            ProtectedMemoryBinary binary = null;

            if (isProtected)
            {
                var pad = this.Context.RandomByteGenerator.NextBytes(bytes.Length);
                var raw = new byte[bytes.Length];

                for (var i = 0; i < bytes.Length; i++)
                    raw[i] = (byte)(bytes[i] ^ pad[i]);

                binary = new ProtectedMemoryBinary(raw, true);
            }
            else
            {
                if (compressed)
                    bytes = Decompress(bytes);

                binary = new ProtectedMemoryBinary(bytes, false);
            }
           
            mapping.Value = binary;
            mapping.Key = mapping.Id.ToString();
            this.Context.Binaries.Add(mapping);
            this.Model.Binaries.Add(new BinaryInfo() { Id = mapping.Id, Compressed = compressed });
        }
    }
}
