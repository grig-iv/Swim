using System.Collections.Generic;

namespace Core.Modules.WorkspaceModule.Configurations;

public class WorkspaceConfig
{
    public string Name { get; set; }
    public IEnumerable<Target> Windows { get; set; }
}