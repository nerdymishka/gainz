using System.Collections;
using NerdyMishka.Flex.Reflection;
using YamlDotNet.RepresentationModel;
using System.Linq;
using System;
using System.Collections.Generic;

namespace NerdyMishka.Flex
{
    public class YamlDotNetFlexVisitor : FlexVisitor<YamlNode, YamlScalarNode, YamlMappingNode, YamlSequenceNode, YamlDocument>
    {
        public override string Name => "YamlDotNet";

        public YamlDotNetFlexVisitor()
        {
            this.FlexSettings = new FlexSettings();
        }

        public YamlDotNetFlexVisitor(FlexSettings settings)
        {
            this.FlexSettings = settings;
        }


        protected override void AddChild(YamlMappingNode documentElement, string name, object value)
            => documentElement.Add(name, (YamlNode)value);

        protected override void AddChild(YamlSequenceNode documentArray, YamlNode value)
            => documentArray.Add(value);

        protected override YamlDocument CreateDocument(YamlNode root)
            => new YamlDocument(root);
        

        protected override IEnumerator<YamlNode> GetEnumerator(YamlSequenceNode array)
            => array.GetEnumerator();

        protected override YamlNode GetRootNode(YamlDocument document)
            => document.RootNode;

        protected override string GetValue(YamlScalarNode documentValue)
            => documentValue.Value;

        protected override void SetValue(YamlScalarNode documentValue, object value)
        {
            documentValue.Value = value as string;
        }

        protected override bool IsNull(YamlScalarNode documentValue)
            => documentValue.Value == null || documentValue.Value == "null";

        protected override YamlScalarNode CreateValue(string value)
        {
            return new YamlScalarNode(value);
        }
    }
}