
using System;
using System.IO;

namespace NerdyMishka.Extensions.Flex 
{

    public static class YamlExtensions
    {

        public static T FromYamlFile<T>(this FlexSerializationBuilder builder, string path)
        {
            var visitor = new YamlObjectReader(builder.Settings);
            
            using(var fs = File.OpenRead(builder.ResolvePath(path)))
            using(var sr = new StreamReader(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream();
                ymlStream.Load(sr);
                if(ymlStream.Documents == null || ymlStream.Documents.Count == 0)
                    throw new System.Exception($"No yaml documents found in ${path}");

                var doc = ymlStream.Documents[0];
                return (T)visitor.VisitDocument(doc, typeof(T));
            }
        }

        public static object FromYamlFile(this FlexSerializationBuilder builder, string path, Type type)
        {
            var visitor = new YamlObjectReader(builder.Settings);
            
            using(var fs = File.OpenRead(builder.ResolvePath(path)))
            using(var sr = new StreamReader(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream();
                ymlStream.Load(sr);
                if(ymlStream.Documents == null || ymlStream.Documents.Count == 0)
                    throw new System.Exception($"No yaml documents found in ${path}");

                var doc = ymlStream.Documents[0];
                return visitor.VisitDocument(doc, type);
            }
        }

        public static FlexSerializationBuilder ToYamlFile(
            this FlexSerializationBuilder builder, string file, object @object)
        {
            var visitor = new YamlObjectWriter(builder.Settings);
            var node = visitor.Visit(@object);
            var doc = new YamlDotNet.RepresentationModel.YamlDocument(node);
            
            using(var fs = File.OpenWrite(builder.ResolvePath(file)))
            using(var sw = new StreamWriter(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream(doc);
                ymlStream.Save(sw);
            }

            return builder;
        } 


        public static FlexSerializationBuilder ToYamlFile<T>(
            this FlexSerializationBuilder builder, string file, T @object)
        {
            var visitor = new YamlObjectWriter(builder.Settings);
            var node = visitor.Visit(@object);
            var doc = new YamlDotNet.RepresentationModel.YamlDocument(node);
            
            using(var fs = File.OpenWrite(builder.ResolvePath(file)))
            using(var sw = new StreamWriter(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream(doc);
                ymlStream.Save(sw);
            }

            return builder;
        } 

    }
}