using System.Collections.Generic;
using Core.Configurations;

namespace Core.Modules.WorkspaceModule.Configurations;

public class WorkspaceManagerConfig : IKeyBindingConfig<WorkspaceCommand>
{
    public IEnumerable<WorkspaceConfig> Workspaces { get; set; }
    public IEnumerable<KeyBinding<WorkspaceCommand>> Bindings { get; set; }
}