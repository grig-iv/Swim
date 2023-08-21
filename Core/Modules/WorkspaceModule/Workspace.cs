using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Optional.Unsafe;
using Utils;

namespace Core.Modules.WorkspaceModule;

[DebuggerDisplay("{Name}")]
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
    private Option<IWindow> MaybeLastWindow { get; set; }

    public bool TryActivate()
    {
        var lastWindow = MaybeLastWindow
            .Where(w => !w.IsDestroyed)
            .ValueOrDefault();
        if (lastWindow != null)
        {
            lastWindow.Focus();
            return true;
        }
            
        
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
        return _orderedTargets.Any(t => t.IsMatch(window));
    }

    public void CycleWindows(CycleDirection direction)
    {
        /*
         * find current target
         * if target has next windows for cycling then activate that windows
         * else get first cycle target with windows and activate windows on it
         * activated target goes to last target
         */

        var foregroundWindow = _desktopService
            .GetForegroundWindowOrNone()
            .ValueOrDefault();
        if (foregroundWindow == null)
        {
            TryActivate();
            MaybeLastWindow = _desktopService.GetForegroundWindowOrNone();
            return;
        }

        var currTarget = _orderedTargets
            .CycleFrom(LastTarget, direction)
            .FirstOrNone(target => target.IsMatch(foregroundWindow))
            .ValueOrDefault();
        if (currTarget == null)
            return;

        var windows = _desktopService.GetWindows().ToList();
        var targetWindows = GetTargetWindows(currTarget, windows, direction);
        var maybeNextWindow = targetWindows
            .SkipWhile(w => !w.Equals(foregroundWindow))
            .Skip(1)
            .FirstOrNone();

        maybeNextWindow.Match(
            nextWindow =>
            {
                LastTarget = currTarget;
                MaybeLastWindow = Option.Some(nextWindow);
                nextWindow.Focus();
            },
            () => _orderedTargets
                .CycleFrom(currTarget, direction)
                .Skip(1)
                .SkipWhile(t =>
                {
                    var maybeWindow = GetTargetWindows(t, windows, direction).FirstOrNone();
                    
                    maybeWindow.MatchSome(w =>
                    {
                        LastTarget = t;
                        MaybeLastWindow = Option.Some(w);
                        w.Focus();
                    });

                    return !maybeWindow.HasValue;
                })
                .FirstOrNone()
                .MatchNone(() =>
                {
                    MaybeLastWindow = Option.None<IWindow>();
                })
        );
    }

    private IEnumerable<IWindow> GetTargetWindows(
        TargetManager target,
        IEnumerable<IWindow> windows,
        CycleDirection direction)
    {
        if (direction == CycleDirection.Forward)
            return windows.Where(target.IsMatch);

        return windows
            .Where(target.IsMatch)
            .Reverse();
    }
}