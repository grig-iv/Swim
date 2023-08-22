using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Utils;

namespace Core.Configurations;

public class KeyBindingCollection<TCommand> : IEnumerable<KeyBinding<TCommand>>
    where TCommand : Enum
{
    private readonly Dictionary<(ModifierKeys, Key), KeyBinding<TCommand>> _bindings;

    public KeyBindingCollection(IEnumerable<KeyBinding<TCommand>> defaultBindings)
    {
        _bindings = defaultBindings.ToDictionary(GetKeyGesture);
    }

    public void OverrideDefaults(IEnumerable<KeyBinding<TCommand>> bindings)
    {
        bindings.ForEach(b => _bindings[GetKeyGesture(b)] = b);
    }

    public IEnumerator<KeyBinding<TCommand>> GetEnumerator()
    {
        return _bindings.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    private (ModifierKeys, Key) GetKeyGesture(KeyBinding binding) => (binding.Mods, binding.Key);
}