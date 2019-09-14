using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NerdyMishka.Flex.Json
{
    public static class JsonFlexBuilderExtensions
    {
        
        public static FlexBuilder ToJsonFile<T>(this FlexBuilder builder, string file, T @object)
        {
            var visitor = new JsonNetFlexVisitor(builder.Build());
            var doc = visitor.VisitComplexObject(@object);
            
            using(var fs = File.OpenWrite(builder.ResolvePath(file)))
            using(var sr = new StreamWriter(fs))
            using (JsonTextWriter writer = new JsonTextWriter(sr))
            {
                doc.WriteTo(writer);
            }

            return builder;
        } 

        public static string ToJsonString<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new JsonNetFlexVisitor(builder.Build());
            var doc = visitor.VisitComplexObject(@object);

            using(var sw = new StringWriter())
            using (JsonTextWriter writer = new JsonTextWriter(sw))
            {
                doc.WriteTo(writer);
                return sw.ToString();
            }
        }

        public static JContainer ToJson<T>(this FlexBuilder builder, T @object)
        {
            var visitor = new JsonNetFlexVisitor(builder.Build());
            var doc = visitor.VisitComplexObject(@object);

            return doc;
        }


        public static T FromJson<T>(this FlexBuilder builder, string Json)
        {
         
            using (var reader = new StringReader(Json))
            {
                var o = (JContainer)JToken.ReadFrom(new JsonTextReader(reader));
                var visitor = new JsonNetFlexVisitor(builder.Build());
                return visitor.Visit<T>(o);
            }
           
        }


        public static T FromJson<T>(this FlexBuilder builder, 
            JContainer document)
        {
             var visitor = new JsonNetFlexVisitor(builder.Build());
            return visitor.Visit<T>(document);
        }

        public static T FromJsonFile<T>(this FlexBuilder builder, string path)
        {
           
            
            using(var fs = File.OpenRead(builder.ResolvePath(path)))
            using(var sr = new StreamReader(fs))
            {
                var o = (JContainer)JToken.ReadFrom(new JsonTextReader(sr));
                var visitor = new JsonNetFlexVisitor(builder.Build());
                return visitor.Visit<T>(o);
            }
        }
    }
}