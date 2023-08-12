using System.Linq;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional.Collections;
using Utils;

namespace Core.Modules.WorkspaceModule;

public class WorkSpace
{
    private readonly CircularList<TargetManager> _orderedTargets;

    public WorkSpace(WorkspaceConfig wsConfig, IDesktopService desktopService)
    {
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
            .EnumerateForwardFrom(LastTarget)
            .FirstOrNone(t => t.TryActivate());

        newTarget.MatchSome(t => LastTarget = t);

        return newTarget.HasValue;
    }

    public bool HasWindow(IWindow window)
    {
        return _orderedTargets.Any(t => t.Target.IsMatch(window));
    }
}