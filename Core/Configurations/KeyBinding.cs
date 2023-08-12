using System;
using System.Windows.Input;

namespace Core.Configurations;

public class KeyBinding<TCommand> : KeyBinding
    where TCommand : Enum
{
    public TCommand Command { get; set; }

    public override object GetCommand() => Command;
}

public abstract class KeyBinding
{
    public ModifierKeys Mods { get; set; }
    public Key Key { get; set; }
    public string Args { get; set; }

    public abstract object GetCommand();
}