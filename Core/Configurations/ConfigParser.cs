using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Optional;
using Optional.Collections;
using Optional.Linq;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Core.Configurations
{
    public class ConfigParser
    {
        private readonly Dictionary<string, Type> _configsTypesByNames;
        private readonly IDeserializer _deserializer;

        public ConfigParser()
        {
            _configsTypesByNames = new Dictionary<string, Type>();
            _deserializer = new DeserializerBuilder()
                .WithTypeConverter(new ModifierKeysConverter())
                .Build();
        }

        public void RegisterConfig(string blockName, Type configBlock)
        {
            _configsTypesByNames[blockName] = configBlock;
        }

        public SwimConfig Parse(Stream stream)
        {
            var reader = new StreamReader(stream);
            var yamlStream = new YamlStream();

            yamlStream.Load(reader);

            var configs = yamlStream.Documents
                .Select(d => d.RootNode)
                .OfType<YamlMappingNode>()
                .SelectMany(n => n.Children)
                .Select(child => ParseBlock(child.Key, child.Value))
                .Values()
                .ToDictionary(o => o.GetType());

            return new SwimConfig(configs);
        }

        private Option<object> ParseBlock(YamlNode keyNode, YamlNode valueNode)
        {
            if (keyNode is YamlScalarNode scalarNode)
            {
                return
                    _configsTypesByNames
                        .GetValueOrNone(scalarNode.Value ?? string.Empty)
                        .Select(configType =>
                        {
                            var serializer = new Serializer();
                            var valueNodeYaml = new StringWriter();
                            serializer.Serialize(valueNodeYaml, valueNode);

                            return _deserializer.Deserialize(valueNodeYaml.ToString(), configType);
                        });
            }
            
            return Option.None<object>();
        }
    }
}