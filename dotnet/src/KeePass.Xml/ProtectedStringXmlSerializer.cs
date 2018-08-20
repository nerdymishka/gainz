using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace NerdyMishka.KeePass.Xml
{
    public class ProtectedStringXmlSerializer : XmlSerializable<ProtectedString>, IXmlSerializable
    {


        public ProtectedStringXmlSerializer(ProtectedString model, SerializerContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        public override string Name
        {
            get
            {
                return "String";
            }
        }

        protected override void VisitElement(XmlReader reader)
        {
            var m = this.Model;
            switch (reader.Name)
            {
                case "Key":
                    reader.Read();

                    m.Key = reader.ReadContentAsString();
                    break;

                case "Value":
                    if (reader.IsEmptyElement)
                        return;

                    bool isProtected = false;
                    
                    if (reader.HasAttributes)
                    {
                        var pValue = reader.GetAttribute("Protected");

                        

                        if (pValue != null && pValue == "True")
                            isProtected = true;
                    }



                    reader.Read();


                    if (isProtected)
                    {
                        var content = reader.ReadContentAsString();

                        if(content.Length > 0)
                        {
                            byte[] xorred = Convert.FromBase64String(content);
                            byte[] pad = this.Context.RandomByteGenerator.NextBytes(xorred.Length);
                            byte[] bytes = new byte[xorred.Length];
                            for (var i = 0; i < bytes.Length; ++i)
                                bytes[i] = (byte)(xorred[i] ^ pad[i]);

                            xorred.Clear();
                            pad.Clear();
                            m.Value = new ProtectedMemoryString(bytes, true);
                        }
                        else
                        {
                            m.Value = new ProtectedMemoryString(new byte[] { }, false);
                        }
                    }
                    else
                    {
                        var value = reader.ReadContentAsString();
                        m.Value = new ProtectedMemoryString(value);
                    }
                   

                    break;
            }
        }

        private bool IsProtected(string key)
        {
            switch(key)
            {
                case "Title":
                    return this.Context.MemoryProtection.ProtectedTitle;
                case "UserName":
                    return this.Context.MemoryProtection.ProtectUserName;
                case "Notes":
                    return this.Context.MemoryProtection.ProtectNotes;
                case "Password":
                    return this.Context.MemoryProtection.ProtectPassword;
                case "URL":
                    return this.Context.MemoryProtection.ProtectUrl;

                default:
                    return false;
            }
        }

        protected override void WriteProperties(XmlWriter writer)
        {
            var m = this.Model;
            writer.WriteElement("Key", m.Key);

            writer.WriteStartElement("Value");
           
            if(m.Value == null)
            {
                writer.WriteEndElement();
                return;
            }



            if (!IsProtected(m.Key))
            {
                writer.WriteString(m.Value.UnprotectAsString());
                writer.WriteEndElement();
                return;
            }

            
            writer.WriteAttributeString("Protected", "True");

            byte[] raw = m.Value.UnprotectAsBytes();
            byte[] pad = this.Context.RandomByteGenerator.NextBytes(raw.Length);
            byte[] bytes = new byte[raw.Length];

            for (var i = 0; i < raw.Length; i++)
                bytes[i] = (byte)(raw[i] ^ pad[i]);

            writer.WriteString(Convert.ToBase64String(bytes));
            writer.WriteEndElement();
        }
    }
}
