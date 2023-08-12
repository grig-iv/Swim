using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core.Configurations;
using Core.Modules.WorkspaceModule.Configurations;
using Core.Services;
using Domain;
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

    private void GoToNextWorkspace()
    {
        var currWorkSpace = GetCurrentWorkspace();

        _workspaces
            .EnumerateForwardFrom(currWorkSpace)
            .Skip(1)
            .Append(currWorkSpace)
            .FirstOrNone(ws => ws.TryActivate())
            .MatchSome(ws =>
            {
                LastWorkspace = ws;
                _whenWorkspaceChanged.OnNext(ws);
            });
    }

    private void GoToPrevWorkspace()
    {
        var currWorkSpace = GetCurrentWorkspace();

        _workspaces
            .EnumerateBackwardFrom(currWorkSpace)
            .Skip(1)
            .Append(currWorkSpace)
            .FirstOrNone(ws => ws.TryActivate())
            .MatchSome(ws =>
            {
                LastWorkspace = ws;
                _whenWorkspaceChanged.OnNext(ws);
            });
    }

    private Workspace GetCurrentWorkspace()
    {
        return _desktopService
            .GetForegroundWindow()
            .SelectMany(window => _workspaces
                .EnumerateForwardFrom(LastWorkspace)
                .FirstOrNone(ws => ws.HasWindow(window)))
            .ValueOr(LastWorkspace);
    }

    private void HandleCommand(WorkspaceCommand command)
    {
        switch (command)
        {
            case WorkspaceCommand.NextWorkspace:
                GoToNextWorkspace();
                break;
            case WorkspaceCommand.PrevWorkspace:
                GoToPrevWorkspace();
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