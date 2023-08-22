using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Core.Configurations;

namespace Core.Modules.WorkspaceModule.Configurations;

public class WorkspaceManagerConfig : IKeyBindingConfig<WorkspaceCommand>
{
    private readonly KeyBindingCollection<WorkspaceCommand> _bindings;

    public WorkspaceManagerConfig()
    {
        _bindings = new KeyBindingCollection<WorkspaceCommand>(
            GetDefaultBindings()
        );
    }

    public IEnumerable<WorkspaceConfig> Workspaces { get; set; }

    public IEnumerable<KeyBinding<WorkspaceCommand>> Bindings
    {
        get => _bindings;
        set => _bindings.OverrideDefaults(value);
    }

    private static IEnumerable<KeyBinding<WorkspaceCommand>> GetDefaultBindings()
    {
        return Enumerable
            .Range(0, 10)
            .Select(i => new KeyBinding<WorkspaceCommand>
            {
                Key = (Key) Enum.Parse(typeof(Key), $"D{i}"),
                Mods = ModifierKeys.Windows | ModifierKeys.Alt,
                Command = WorkspaceCommand.GoToWorkspace,
                Args = i.ToString()
            });
    }
}