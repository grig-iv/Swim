using System;
using Core.Modules.WorkspaceModule.Configurations;
using Domain;
using Optional.Collections;

namespace Core.Modules.WorkspaceModule;

public class TargetManager
{
    private readonly IDesktopService _desktopService;

    public TargetManager(Target target, IDesktopService desktopService)
    {
        _desktopService = desktopService;
        Target = target;
    }

    public Target Target { get; }

    public bool TryActivate()
    {
        var maybeWindow = _desktopService
            .GetWindows()
            .FirstOrNone(Target.IsMatch);

        maybeWindow.MatchSome(window =>
        {
            window.SetForeground();
            switch (window.GetState())
            {
                case WindowState.Minimized:
                    window.Restore();
                    break;
                case WindowState.Maximized:
                    window.Maximize();
                    break;
                case WindowState.Normal:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });

        return maybeWindow.HasValue;
    }
}