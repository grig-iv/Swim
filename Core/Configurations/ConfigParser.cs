using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utils;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Core.Configurations
{
    public class ConfigParser
    {
        private readonly Dictionary<string, Type> _configsTypesByNames;
        private readonly Deserializer _deserializer;

        public ConfigParser()
        {
            _configsTypesByNames = new Dictionary<string, Type>();
            _deserializer = new Deserializer();
        }

        public void RegisterConfig(string blockName, Type configBlock)
        {
            _configsTypesByNames[blockName] = configBlock;
        }

        public SwimConfig Parse(Stream stream)
        {
            var reader = new StreamReader(stream);
            var yamlStream = new YamlStream();
            
            var configs = new Dictionary<Type, object>();

            yamlStream.Load(reader);
            yamlStream.Documents
                .Select(d => d.RootNode)
                .OfType<YamlMappingNode>()
                .SelectMany(n => n.Children)
                .ForEach(child =>
                {
                    if (child.Key is YamlScalarNode scalarKey &&
                        _configsTypesByNames.TryGetValue(scalarKey.Value, out var configType))
                    {
                        var config = _deserializer.Deserialize(child.Value.ToString(), configType);
                        configs[config.GetType()] = config;
                    }
                });

            return new SwimConfig(configs);
        }
    }
}