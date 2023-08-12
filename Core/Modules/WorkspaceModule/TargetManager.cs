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
       var maybeWindow =  _desktopService
            .GetWindows()
            .FirstOrNone(Target.IsMatch);
       
       maybeWindow.MatchSome(window =>
       {
           window.SetForeground();
           window.Maximize();
       });

       return maybeWindow.HasValue;
    }
}