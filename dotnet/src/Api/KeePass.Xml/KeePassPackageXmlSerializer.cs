using NerdyMishka.KeePass.Cryptography;
using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NerdyMishka.KeePass.Xml
{
    public class KeePassPackageXmlSerializer : IKeePassPackageSerializer
    {
        public KeePassPackageXmlSerializer()
        {
            this.Binaries = new Dictionary<string, ProtectedMemoryBinary>();
            this.Mappings = new Dictionary<Type, Type>()
            {
                { typeof(IKeePassDocument), typeof(KeePassDocument) },
                {typeof(IKeePassAssociation), typeof(KeePassAssocation) },
                {typeof(IKeePassAutoType), typeof(KeePassAutoType) },
                {typeof(IKeePassAuditFields), typeof(KeePassAuditFields) },
                {typeof(IKeePassEntry) , typeof(KeePassEntry) },
                { typeof(IKeePassGroup), typeof(KeePassGroup) },
           
            };
        }

        public string Extension => ".kdbx";

        public IDictionary<string, ProtectedMemoryBinary> Binaries { get; set; }

        public IDictionary<Type, Type> Mappings { get; set; }

        public void Read(IKeePassPackage package,  Stream stream)
        {
            var settings = new XmlReaderSettings()
            {
                CloseInput = true,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true,

                DtdProcessing = DtdProcessing.Prohibit
            };


            settings.ValidationType = ValidationType.None;
            XmlReader reader = XmlReader.Create(stream, settings);

            reader.Read();

          

            var serializerContext = new SerializerContext()
            {
                RandomByteGenerator = package.HeaderInfo.RandomByteGenerator,
                Binaries = package.Binaries.ToList(),
                DatabaseCompression = package.HeaderInfo.DatabaseCompression,
                Mappings = this.Mappings
            };

            var metaSerializer = new KeePassPackageMetaInfoXmlSerializer(package.MetaInfo, serializerContext);
            metaSerializer.ReadXml(reader);

            // set memory protection options. 
            serializerContext.MemoryProtection = package.MetaInfo.MemoryProtection;

            var rootSerializer = new KeePassDocumentXmlSerializer(package.Document, serializerContext);
            rootSerializer.ReadXml(reader);

            foreach(var binary in serializerContext.Binaries)
            {
                package.Binaries.Add(binary);
            }
            
        }

        public void Write(IKeePassPackage package,  Stream stream)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = new UTF8Encoding(false, false)
            };

#if XML_FULL
            settings.ValidationType = ValidationType.None;
#endif 

            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                writer.WriteStartElement("KeePassFile");

                package.HeaderInfo.ClearGeneratorEngine();

                var serializerContext = new SerializerContext()
                {
                    RandomByteGenerator = package.HeaderInfo.RandomByteGenerator,
                    MemoryProtection = package.MetaInfo.MemoryProtection,
                    Binaries = package.Binaries.ToList(),
                    Mappings =this.Mappings
                };

                var metaSerializer = new KeePassPackageMetaInfoXmlSerializer(package.MetaInfo, serializerContext);
                metaSerializer.WriteXml(writer);

                var rootSerializer = new KeePassDocumentXmlSerializer(package.Document, serializerContext);
                rootSerializer.WriteXml(writer);

                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
