using System.Linq;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Optional.Unsafe;
using Utils;

namespace Core.Modules.WorkspaceModule;

public class Workspace
{
    private readonly IDesktopService _desktopService;
    private readonly CircularList<TargetManager> _orderedTargets;

    public Workspace(WorkspaceConfig wsConfig, IDesktopService desktopService)
    {
        _desktopService = desktopService;
        Name = wsConfig.Name;

        _orderedTargets = new CircularList<TargetManager>();
        wsConfig.Windows
            .Select(t => new TargetManager(t, desktopService))
            .ForEach(_orderedTargets.Add);

        LastTarget = _orderedTargets.First();
    }

    public string Name { get; }

    private TargetManager LastTarget { get; set; }

    public bool TryActivate()
    {
        if (LastTarget.TryActivate())
            return true;

        var newTarget = _orderedTargets
            .CycleFrom(LastTarget)
            .FirstOrNone(t => t.TryActivate());

        newTarget.MatchSome(t => LastTarget = t);

        return newTarget.HasValue;
    }

    public bool HasWindow(IWindow window)
    {
        return _orderedTargets.Any(t => t.Target.IsMatch(window));
    }

    public void CycleWindows(CycleDirection direction)
    {
        var maybeForegroundWindow = _desktopService.GetForegroundWindow();
        if (!maybeForegroundWindow.HasValue)
            return;

        var firstWindow = default(IWindow);
        var windows = _desktopService.GetWindows();
        var foregroundWindow = maybeForegroundWindow.ValueOrFailure();

        if (direction == CycleDirection.Backward)
            windows = windows.Reverse();

        _orderedTargets
            .CycleFrom(LastTarget)
            .SelectMany(target => windows.Where(target.Target.IsMatch))
            .Do(window => firstWindow ??= window)
            .SkipWhile(window => !window.Equals(foregroundWindow))
            .Skip(1)
            .FirstOrNone()
            .ValueOr(firstWindow)
            .Focus();
    }
}