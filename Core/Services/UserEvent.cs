namespace Core.Services;

public class UserCommand<TCommand> : UserCommand
{
    public UserCommand(TCommand command) : base(command)
    {
    }

    public UserCommand(TCommand command, string args) : base(command, args)
    {
    }

    public new TCommand Command => (TCommand) base.Command;
}

public abstract class UserCommand
{
    public UserCommand(object command) : this(command, string.Empty)
    {
    }

    public UserCommand(object command, string args)
    {
        Command = command;
        Args = args;
    }

    public object Command { get; }
    public string Args { get; }
}