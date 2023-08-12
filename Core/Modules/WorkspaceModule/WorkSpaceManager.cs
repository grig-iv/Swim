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

public class WorkSpaceManager
{
    private readonly IDesktopService _desktopService;
    private readonly Subject<WorkSpace> _whenWorkSpaceChanged;
    private readonly CircularList<WorkSpace> _workSpaces;

    public WorkSpaceManager(
        IConfigProvider configProvider,
        IDesktopService desktopService,
        IUserEventPublisher userEventPublisher
    )
    {
        _desktopService = desktopService;
        _whenWorkSpaceChanged = new Subject<WorkSpace>();
        _workSpaces = new CircularList<WorkSpace>();

        configProvider.WhenConfigChanged.Subscribe(config => UpdateWorkSpaces(config, desktopService));

        userEventPublisher
            .OfType<WorkSpaceCommand>()
            .Subscribe(HandleWorkSpaceCommand);
    }

    public IObservable<WorkSpace> WhenWorkSpaceChanged => _whenWorkSpaceChanged
        .AsObservable()
        .DistinctUntilChanged();

    private WorkSpace LastWorkSpace { get; set; }

    private void GoToNext()
    {
        var currWorkSpace = GetCurrentWorkSpace();

        _workSpaces
            .EnumerateForwardFrom(currWorkSpace)
            .Skip(1)
            .Append(currWorkSpace)
            .FirstOrNone(ws => ws.TryActivate())
            .MatchSome(ws =>
            {
                LastWorkSpace = ws;
                _whenWorkSpaceChanged.OnNext(ws);
            });
    }

    private void GoToPrevious()
    {
        var currWorkSpace = GetCurrentWorkSpace();

        _workSpaces
            .EnumerateBackwardFrom(currWorkSpace)
            .Skip(1)
            .Append(currWorkSpace)
            .FirstOrNone(ws => ws.TryActivate())
            .MatchSome(ws =>
            {
                LastWorkSpace = ws;
                _whenWorkSpaceChanged.OnNext(ws);
            });
    }

    private WorkSpace GetCurrentWorkSpace()
    {
        return _desktopService
            .GetForegroundWindow()
            .SelectMany(window => _workSpaces
                .EnumerateForwardFrom(LastWorkSpace)
                .FirstOrNone(ws => ws.HasWindow(window)))
            .ValueOr(LastWorkSpace);
    }

    private void HandleWorkSpaceCommand(WorkSpaceCommand command)
    {
        switch (command)
        {
            case WorkSpaceCommand.NextWorkSpace:
                GoToNext();
                break;
            case WorkSpaceCommand.PrevWorkSpace:
                GoToPrevious();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }

    private void UpdateWorkSpaces(SwimConfig config, IDesktopService desktopService)
    {
        _workSpaces.Clear();

        config
            .GetConfig<WorkspaceManagerConfig>()
            .MatchSome(moduleConfig => moduleConfig.Workspaces
                .Select(wsConfig => new WorkSpace(wsConfig, desktopService))
                .ForEach(_workSpaces.Add));
    }
}