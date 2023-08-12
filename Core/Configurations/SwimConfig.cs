using System;
using System.Collections.Generic;
using Optional;
using Optional.Collections;
using Optional.Linq;

namespace Core.Configurations;

public class SwimConfig
{
    public readonly static SwimConfig Empty = new SwimConfig(new Dictionary<Type, object>());

    private readonly Dictionary<Type, object> _configs;

    public SwimConfig(Dictionary<Type, object> configs)
    {
        _configs = configs;
    }

    public IEnumerable<object> Configs => _configs.Values;

    public Option<TConfig> GetConfig<TConfig>()
    {
        return _configs
            .GetValueOrNone(typeof(TConfig))
            .Select(o => (TConfig) o);
    }
}