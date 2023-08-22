using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core.Configurations;
using Core.Modules.WorkspaceModule.Configurations;
using Core.Services;
using Domain;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Utils;

namespace Core.Modules.WorkspaceModule;

public class WorkspaceManager
{
    private readonly IDesktopService _desktopService;
    private readonly Subject<Workspace> _whenWorkspaceChanged;
    private readonly CircularList<Workspace> _workspaces;

    public WorkspaceManager(
        IConfigProvider configProvider,
        IDesktopService desktopService,
        IUserEventPublisher userEventPublisher
    )
    {
        _desktopService = desktopService;
        _whenWorkspaceChanged = new Subject<Workspace>();
        _workspaces = new CircularList<Workspace>();

        configProvider.WhenConfigChanged.Subscribe(config => UpdateWorkspaces(config, desktopService));

        userEventPublisher
            .OfType<UserCommand<WorkspaceCommand>>()
            .Subscribe(HandleCommand);
    }

    public IObservable<Workspace> WhenWorkspaceChanged => _whenWorkspaceChanged.AsObservable();

    private Workspace LastWorkspace { get; set; }

    private void CycleWorkspaces(CycleDirection direction)
    {
        var currWorkSpace = GetCurrentWorkspace().ValueOr(LastWorkspace);

        _workspaces
            .CycleFrom(currWorkSpace, direction)
            .Skip(1)
            .Append(currWorkSpace)
            .FirstOrNone(ws => ws.TryActivate())
            .MatchSome(ws =>
            {
                LastWorkspace = ws;
                _whenWorkspaceChanged.OnNext(ws);
            });
    }

    private void CycleWindows(CycleDirection direction)
    {
        GetCurrentWorkspace().MatchSome(ws => ws.CycleWindows(direction));
    }

    private Option<Workspace> GetCurrentWorkspace()
    {
        return _desktopService
            .GetForegroundWindowOrNone()
            .SelectMany(window => _workspaces
                .CycleFrom(LastWorkspace)
                .FirstOrNone(ws => ws.HasWindow(window)));
    }

    private void GoToWorkspace(string workspaceId)
    {
        var isWsIndex = int.TryParse(workspaceId, out var wsIndex);
        if (isWsIndex)
        {
            _workspaces
                .ElementAtOrNone(wsIndex - 1)
                .MatchSome(ws => ws.TryActivate());
            return;
        }
        
        _workspaces
            .FirstOrNone(w => string.Equals(w.Name, workspaceId, StringComparison.InvariantCultureIgnoreCase))
            .MatchSome(ws => ws.TryActivate());
    }

    private void HandleCommand(UserCommand<WorkspaceCommand> userCommand)
    {
        switch (userCommand.Command)
        {
            case WorkspaceCommand.NextWorkspace:
                CycleWorkspaces(CycleDirection.Forward);
                break;
            case WorkspaceCommand.PrevWorkspace:
                CycleWorkspaces(CycleDirection.Backward);
                break;
            case WorkspaceCommand.GoToWorkspace:
                GoToWorkspace(userCommand.Args);
                break;
            case WorkspaceCommand.NextWindow:
                CycleWindows(CycleDirection.Forward);
                break;
            case WorkspaceCommand.PrevWindow:
                CycleWindows(CycleDirection.Backward);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(userCommand), userCommand, null);
        }
    }

    private void UpdateWorkspaces(SwimConfig config, IDesktopService desktopService)
    {
        _workspaces.Clear();

        config
            .GetConfig<WorkspaceManagerConfig>()
            .MatchSome(moduleConfig => moduleConfig.Workspaces
                .Select(wsConfig => new Workspace(wsConfig, desktopService))
                .ForEach(_workspaces.Add));
    }
}