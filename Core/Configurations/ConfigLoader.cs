using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using Utils;

namespace Core.Configurations;

public class ConfigLoader : IConfigProvider
{
    private readonly ReplaySubject<SwimConfig> _whenConfigChanged;
    private readonly ConfigParser _parser;

    public ConfigLoader(IConfigLocator configLocator)
    {
        _parser = new ConfigParser();
        _whenConfigChanged = new ReplaySubject<SwimConfig>(1);
        _whenConfigChanged.Subscribe(c => Config = c);

        InitParser();

        configLocator
            .FindConfig()
            .MatchSome(stream =>
            {
                using (stream)
                {
                    var config = _parser.Parse(stream);
                    _whenConfigChanged.OnNext(config);
                }
            });
    }

    public IObservable<SwimConfig> WhenConfigChanged => _whenConfigChanged.AsObservable();
    public SwimConfig Config { get; private set; }

    private void InitParser()
    {
        Assembly
            .GetAssembly(typeof(Swim))
            .GetTypes()
            .Where(type => type
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IKeyBindingConfig<>)))
            .ForEach(t => _parser.RegisterConfig(t.Name.Replace("Config", string.Empty), t));
    }
}