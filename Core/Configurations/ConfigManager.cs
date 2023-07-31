using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Security;
using Core.Services;
using Optional.Linq;
using Utils;

namespace Core.Configurations
{
    public class ConfigManager : IConfigProvider
    {
        private readonly ReplaySubject<SwimConfig> _whenConfigChanged;
        private readonly ConfigParser _parser;

        public ConfigManager(
            IConfigLocator configLocator,
            IUserEventPublisher userEventPublisher)
        {
            _parser = new ConfigParser();
            _whenConfigChanged = new ReplaySubject<SwimConfig>(1);
            _whenConfigChanged.Subscribe(c => Config = c);

            InitParser();

            var config = configLocator
                .FindConfig()
                .Select(_parser.Parse)
                .ValueOr(SwimConfig.Empty);

            _whenConfigChanged.OnNext(config);
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
                    .Contains(typeof(IModuleConfig<>)))
                .ForEach(_parser.RegisterConfig<>());

            _parser.RegisterConfig<>();
        }
    }
}