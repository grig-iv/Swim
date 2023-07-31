using System.Collections.Generic;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;

namespace Core.Modules.WorkspaceModule
{
    public class WorkSpace
    {
        private readonly List<ManagedWindow> _windows;

        public WorkSpace(WorkspaceConfig wsConfig, IDesktopService desktopService)
        {
            _windows = new List<ManagedWindow>();
        }

        public IReadOnlyList<ManagedWindow> Windows => _windows;
        
        public string Name { get; }
        
        public bool HasForegroundWindow { get; private set; }
        public bool HasOpenWindows { get; private set; }

        public void Activate()
        {
            throw new System.NotImplementedException();
        }
    }
}