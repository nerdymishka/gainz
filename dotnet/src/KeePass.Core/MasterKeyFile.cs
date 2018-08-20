using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace NerdyMishka.KeePass
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;KeyFile&gt;
    ///     &lt;Meta&gt;
    ///         &lt;Version&gt;1.00&gt;/Version&gt;
    ///     &lt;/Meta&gt;
    ///     &lt;Key&gt;
    ///         &lt;Data&gt;sdfadf==&lt;/Data&gt;
    ///     &lt;/Key&gt;
    /// &lt;/KeyFile&gt;
    /// </code>
    /// 
    /// </remarks>
    //
    public class MasterKeyFile : MasterKeyFragment
    {
        public MasterKeyFile(string path)
        {
            Check.NotEmpty(nameof(path), path);

            this.Path = path;

            if (!System.IO.File.Exists(path))
                throw new System.IO.FileNotFoundException(path);

            var data = GetData(path);
            this.SetData(data);

            data.Clear();
        }

        public string Path { get; private set; }

        

        private static byte[] GetData(string path)
        {
            var bytes = GetDataFromFile(path);

            var utf8 = new System.Text.UTF8Encoding(false, false);
            var value = utf8.GetString(bytes);

            var hex = bytes.ToHexString();
            var key = Enumerable.Range(0, hex.Length / 2)
                                .Select(x => Convert.ToByte(hex.Substring(x * 2, 2), 16))
                                .ToArray();
            //if (key == null || key.Length != 32)
            //    return null;

            return key;
        }

        

        private static byte[] GetDataFromFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var doc = new XmlDocument();
                doc.Load(fs);

                var el = doc.DocumentElement;
                if (el == null)
                    return null;

                if (!el.Name.Equals("KeyFile"))
                    return null;

                foreach(XmlNode child in el.ChildNodes)
                {
                    if(child.Name == "Key")
                    {
                        foreach(XmlNode subChild in child.ChildNodes)
                        {
                            if(subChild.Name == "Data")
                            {
                                return Convert.FromBase64String(subChild.InnerText);
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static void Generate(string path, byte[] entropy)
        {
            var generator = Cryptography.RandomByteGeneratorFactory.GetGenerator(2);
            var key = generator.NextBytes(32);
            byte[] hash = null;
            

            using (var ms = new MemoryStream())
            {
                ms.Write(entropy);
                ms.Write(key);

                hash = ms.ToArray().ToSHA256Hash();
            }

            CreateFile(path, hash);
        }

        private static void CreateFile(string path, byte[] data)
        {
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t",
                Encoding = new UTF8Encoding(false, false)
            };

            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            using (var xmlWriter = XmlWriter.Create(fs, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("KeyFile");
                xmlWriter.WriteStartElement("Meta");
                xmlWriter.WriteStartElement("Version");
                xmlWriter.WriteString("1.00");
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Data");
                xmlWriter.WriteString(Convert.ToBase64String(data));
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                
            }
               
        }
             

        private byte[] Copy(byte[] source, int offset, int length = 4)
        {
            byte[] copy = new byte[length];
            Array.Copy(source, offset, copy, 0, length);
            return copy;
        }
    }
}
