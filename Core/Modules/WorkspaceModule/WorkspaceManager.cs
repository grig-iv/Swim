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
            .OfType<WorkspaceCommand>()
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
            .GetForegroundWindow()
            .SelectMany(window => _workspaces
                .CycleFrom(LastWorkspace)
                .FirstOrNone(ws => ws.HasWindow(window)));
    }

    private void HandleCommand(WorkspaceCommand command)
    {
        switch (command)
        {
            case WorkspaceCommand.NextWorkspace:
                CycleWorkspaces(CycleDirection.Forward);
                break;
            case WorkspaceCommand.PrevWorkspace:
                CycleWorkspaces(CycleDirection.Backward);
                break;
            case WorkspaceCommand.NextWindow:
                CycleWindows(CycleDirection.Forward);
                break;
            case WorkspaceCommand.PrevWindow:
                CycleWindows(CycleDirection.Backward);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
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