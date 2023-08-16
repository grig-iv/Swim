using System;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional.Collections;

namespace Core.Modules.WorkspaceModule;

public class TargetManager
{
    private readonly IDesktopService _desktopService;
    private readonly Target _target;

    public TargetManager(Target target, IDesktopService desktopService)
    {
        _desktopService = desktopService;
        _target = target;
    }

    public bool TryActivate()
    {
        var maybeWindow = _desktopService
            .GetWindows()
            .FirstOrNone(_target.IsMatch);

        maybeWindow.MatchSome(w => w.Focus());

        return maybeWindow.HasValue;
    }

    public bool IsMatch(IWindow window)
    {
        return _target.IsMatch(window);
    } 
}