using System.IO;
using YamlDotNet.RepresentationModel;

namespace NerdyMishka.Flex.Yaml
{
    public static class YamlFlexBuilderExtensions
    {
        
        public static FlexBuilder ToYamlFile<T>(this FlexBuilder builder, string file, T @object)
        {
            var visitor = new YamlDotNetFlexVisitor(builder.Build());
            var doc = visitor.VisitDocument(@object);

            using(var fs = System.IO.File.OpenWrite(file))
            using(var sw = new StreamWriter(fs))
            {
                var ymlStream = new YamlStream(doc);
                ymlStream.Save(sw, false);
                fs.Flush();
            }
            
            return builder;
        } 

        public static string ToYamlString<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new YamlDotNetFlexVisitor(builder.Build());
            var doc = visitor.VisitDocument(@object);

            using(var sw = new StringWriter())
            {
                var ymlStream = new YamlStream(doc);
            
                ymlStream.Save(sw, false);
                sw.Flush();
                return sw.ToString();
            }
        }

        public static YamlDocument ToYaml<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new YamlDotNetFlexVisitor(builder.Build());
            var doc = visitor.VisitDocument(@object);

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
                var visitor = new YamlDotNetFlexVisitor(builder.Build());
                return visitor.VisitObject<T>(doc);
            }  
        }


        public static T FromYaml<T>(this FlexBuilder builder, 
            YamlDocument document)
        {
            var visitor = new YamlDotNetFlexVisitor(builder.Build());
            return visitor.VisitObject<T>(document);
        }

        public static T FromYamlFile<T>(this FlexBuilder builder, string path)
        {
            var visitor = new YamlDotNetFlexVisitor(builder.Build());
            
            var content = System.IO.File.ReadAllText(path);
            return FromYaml<T>(builder, content);
        }
    }
}