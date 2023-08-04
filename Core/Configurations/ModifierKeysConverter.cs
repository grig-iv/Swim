using System;
using System.Windows.Input;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace Core.Configurations;

/// <summary>
/// Converts a pipe-delimited string of modifier keys into a <see cref="ModifierKeys"/> enumeration.
/// This class enables the deserialization of modifier keys from a YAML format where individual keys are separated by the '|' character.
/// </summary>
/// <example>
/// For example, the YAML string "Alt|Control" would be converted into the corresponding <see cref="ModifierKeys"/> value of ModifierKeys.Alt | ModifierKeys.Control.
/// </example>
public class ModifierKeysConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(ModifierKeys);
    }

    public object ReadYaml(IParser parser, Type type)
    {
        var value = parser.Consume<Scalar>().Value;
        return Enum.Parse(typeof(ModifierKeys), value.Replace("|", ","));
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        emitter.Emit(new Scalar(null, value.ToString().Replace(",", "|")));
    }
}