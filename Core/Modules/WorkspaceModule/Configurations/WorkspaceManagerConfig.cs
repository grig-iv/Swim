using System.Collections.Generic;
using Core.Configurations;

namespace Core.Modules.WorkspaceModule.Configurations
{
    public class WorkspaceManagerConfig : IModuleConfig<WorkSpaceCommand>
    {
        public IEnumerable<WorkspaceConfig> Workspaces { get; }
        public IEnumerable<KeyBinding<WorkSpaceCommand>> Bindings { get; set; }
    }
}