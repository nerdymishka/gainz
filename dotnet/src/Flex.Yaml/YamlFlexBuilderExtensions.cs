using System.IO;
using YamlDotNet.RepresentationModel;

namespace NerdyMishka.Flex.Yaml
{
    public static class YamlFlexBuilderExtensions
    {
        
        public static FlexBuilder ToYamlFile<T>(this FlexBuilder builder, string file, T @object)
        {
            var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
            var doc = visitor.ToDoc(@object);
            
            using(var fs = File.OpenWrite(builder.ResolvePath(file)))
            using(var sw = new StreamWriter(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream(doc);
                ymlStream.Save(sw);
            }

            return builder;
        } 

        public static string ToYamlString<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
            var doc = visitor.ToDoc(@object);

            using(var sw = new StringWriter())
            {
                var ymlStream = new YamlStream(doc);
                ymlStream.Save(sw);
                sw.Flush();
                return sw.ToString();
            }
        }

        public static YamlDocument ToYaml<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
            var doc = visitor.ToDoc(@object);

            return doc;
        }


        public static T FromYaml<T>(this FlexBuilder builder, string yaml)
        {
            var ymlStream = new YamlStream();
            using(var sr = new StringReader(yaml))
            {
                ymlStream.Load(sr);
                if(ymlStream.Documents == null || ymlStream.Documents.Count == 0)
                    throw new System.Exception($"No yaml documents found in yaml string");

                var doc = ymlStream.Documents[0];
                var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
                return visitor.Visit<T>(doc);
            }  
        }


        public static T FromYaml<T>(this FlexBuilder builder, 
            YamlDocument document)
        {
            var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
            return visitor.Visit<T>(document);
        }

        public static T FromYamlFile<T>(this FlexBuilder builder, string path)
        {
            var visitor = new ObjectYamlVisitor(builder.FlexCryptoProvider);
            
            using(var fs = File.OpenRead(builder.ResolvePath(path)))
            using(var sr = new StreamReader(fs))
            {
                var ymlStream = new YamlDotNet.RepresentationModel.YamlStream();
                ymlStream.Load(sr);
                if(ymlStream.Documents == null || ymlStream.Documents.Count == 0)
                    throw new System.Exception($"No yaml documents found in ${path}");

                var doc = ymlStream.Documents[0];
                return visitor.Visit<T>(doc);
            }
        }
    }
}