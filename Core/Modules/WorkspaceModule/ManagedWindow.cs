using System;
using System.Reactive;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;

namespace Core.Modules.WorkspaceModule
{
    public class ManagedWindow
    {
        public ManagedWindow(IWindow window, TargetWindow target)
        {
            Window = window;
            Target = target;
        }

        public IObservable<Unit> WhenDestroyed => Window.WhenDestroyed;
        
        public TargetWindow Target { get; }
        public IWindow Window { get; }
    }
}