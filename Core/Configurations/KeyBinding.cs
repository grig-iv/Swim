using System;
using System.Windows.Input;
using Core.Services;

namespace Core.Configurations;

public abstract class KeyBinding
{
    public ModifierKeys Mods { get; set; }
    public Key Key { get; set; }
    public string Args { get; set; }

    public abstract object GetCommand();
    public abstract UserCommand GetUserCommand();
}

public class KeyBinding<TCommand> : KeyBinding
    where TCommand : Enum
{
    public TCommand Command { get; set; }

    public override object GetCommand() => Command;
    
    public override UserCommand GetUserCommand()
    {
        return new UserCommand<TCommand>(Command, Args);
    }
}
