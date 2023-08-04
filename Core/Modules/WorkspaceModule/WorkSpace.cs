using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Utils;

namespace Core.Modules.WorkspaceModule
{
    public class WorkSpace
    {
        private readonly List<TargetWindow> _orderedTargets;
        private readonly Dictionary<TargetWindow, List<ManagedWindow>> _targetToWindowList;

        public WorkSpace(WorkspaceConfig wsConfig, IDesktopService desktopService)
        {
            Name = wsConfig.Name;

            _orderedTargets = wsConfig.Windows.ToList();
            _targetToWindowList = _orderedTargets.ToDictionary(x => x, _ => new List<ManagedWindow>());

            desktopService.WhenWindowCreated
                .StartWith(desktopService.GetWindows())
                .Subscribe(window => _orderedTargets
                    .Where(target => target.IsMatch(window))
                    .Select(target => new ManagedWindow(window, target))
                    .ForEach(AddManagedWindow));

            desktopService.WhenForegroundWindowChanged
                .Select(w => _orderedTargets.Any(t => t.IsMatch(w)))
                .Subscribe(x => HasForegroundWindow = x);

            CurrentWindow = Windows.FirstOrNone();
        }

        public IEnumerable<ManagedWindow> Windows => _orderedTargets.SelectMany(t => _targetToWindowList[t]);

        public string Name { get; }

        public bool HasForegroundWindow { get; private set; }
        public bool HasOpenWindows => Windows.Any();
        public Option<ManagedWindow> CurrentWindow { get; private set; }

        public void Activate()
        {
            CurrentWindow
                .Select(mw => mw.Window)
                .MatchSome(window =>
                {
                    window.SetForeground();
                    window.Maximize();
                });
        }

        private void AddManagedWindow(ManagedWindow managedWindow)
        {
            _targetToWindowList[managedWindow.Target].Add(managedWindow);
            
            managedWindow.WhenDestroyed.Subscribe(_ => RemoveManagedWindow(managedWindow));
        }

        private void RemoveManagedWindow(ManagedWindow managedWindow)
        {
            _targetToWindowList[managedWindow.Target].Remove(managedWindow);
        }
    }
}