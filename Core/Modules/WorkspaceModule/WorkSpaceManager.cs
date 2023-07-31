using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Core.Configurations;
using Core.Modules.WorkspaceModule.Configurations;
using Core.Services;
using Domain;
using Optional.Collections;
using Utils;

namespace Core.Modules.WorkspaceModule
{
    public class WorkSpaceManager
    {
        private readonly Subject<WorkSpace> _whenWorkSpaceChanged;
        private readonly List<WorkSpace> _workSpaces;

        public WorkSpaceManager(
            IConfigProvider configProvider,
            IDesktopService desktopService,
            IUserEventPublisher userEventPublisher
        )
        {
            _whenWorkSpaceChanged = new Subject<WorkSpace>();
            _workSpaces = new List<WorkSpace>();

            configProvider.WhenConfigChanged.Subscribe(config =>
            {
                UpdateWorkSpaces(config, desktopService);
                UpdateCurrWorkSpace();
            });

            userEventPublisher
                .OfType<WorkSpaceCommand>()
                .Subscribe(HandleWorkSpaceCommand);
        }

        public IObservable<WorkSpace> WhenWorkSpaceChanged => _whenWorkSpaceChanged
            .AsObservable()
            .DistinctUntilChanged();

        public IObservable<IWindow> WhenWindowChanged { get; set; }

        public WorkSpace CurrentWorkSpace { get; private set; }

        public void GoToNext()
        {
            CurrentWorkSpace = EnumerateCircular(CurrentWorkSpace, isInverse: false)
                .FirstOrNone(ws => ws.HasOpenWindows)
                .ValueOr(CurrentWorkSpace);

            CurrentWorkSpace.Activate();
            _whenWorkSpaceChanged.OnNext(CurrentWorkSpace);
        }

        public void GoToPrevious()
        {
            CurrentWorkSpace = EnumerateCircular(CurrentWorkSpace, isInverse: true)
                .FirstOrNone(ws => ws.HasOpenWindows)
                .ValueOr(CurrentWorkSpace);

            CurrentWorkSpace.Activate();
            _whenWorkSpaceChanged.OnNext(CurrentWorkSpace);
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

        private void UpdateCurrWorkSpace()
        {
            CurrentWorkSpace = _workSpaces
                .FirstOrNone(ws => ws.HasForegroundWindow)
                .ValueOr(_workSpaces.First);

            _whenWorkSpaceChanged.OnNext(CurrentWorkSpace);
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

        private IEnumerable<WorkSpace> EnumerateCircular(WorkSpace from, bool isInverse = false)
        {
            var startIndex = _workSpaces.IndexOf(from);
            var step = isInverse ? -1 : 1;

            for (int index = startIndex, count = 0; count < _workSpaces.Count; index += step, count++)
            {
                index %= _workSpaces.Count; // Ensure index is within the bounds
                yield return _workSpaces[index];
            }
        }
    }
}