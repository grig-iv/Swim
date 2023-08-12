using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Core.Configurations;
using Core.Services;
using NHotkey.Wpf;
using Utils;
using KeyBinding = Core.Configurations.KeyBinding;

namespace Core.EventSystem;

public class UserEventPublisher : IUserEventPublisher
{
    private readonly Subject<object> _whenHotKeyFired;
    private readonly HotkeyManager _hotkeyManager;

    public UserEventPublisher(IConfigProvider configProvider)
    {
        _whenHotKeyFired = new Subject<object>();
        _hotkeyManager = HotkeyManager.Current;

        configProvider.WhenConfigChanged.Subscribe(UpdateKeybinding);
    }

    public IDisposable Subscribe(IObserver<object> observer)
    {
        return _whenHotKeyFired.Subscribe(observer);
    }

    private void UpdateKeybinding(SwimConfig swimConfig)
    {
        swimConfig.Configs
            .Where(config => config
                .GetType()
                .GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IKeyBindingConfig<>)))
            .SelectMany(config => (IEnumerable<KeyBinding>) config
                .GetType()
                .GetProperty(nameof(IKeyBindingConfig<Enum>.Bindings))?
                .GetValue(config))
            .ForEach(RegisterKeyBinding);
    }

    private void RegisterKeyBinding(KeyBinding keyBinding)
    {
        var command = keyBinding.GetCommand();
        var fullEnumName = command.GetType().FullName; 
        var fullCommandName = $"{fullEnumName}.{command}"; 

        _hotkeyManager.AddOrReplace(
            fullCommandName,
            keyBinding.Key,
            keyBinding.Mods,
            (_, _) => _whenHotKeyFired.OnNext(command)
        );
    }
}