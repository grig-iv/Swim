using System.Collections.Generic;
using Core.Configurations;

namespace Core.Modules.WorkspaceModule.Configurations;

public class WorkspaceManagerConfig : IKeyBindingConfig<WorkSpaceCommand>
{
    public IEnumerable<WorkspaceConfig> Workspaces { get; set; }
    public IEnumerable<KeyBinding<WorkSpaceCommand>> Bindings { get; set; }
}