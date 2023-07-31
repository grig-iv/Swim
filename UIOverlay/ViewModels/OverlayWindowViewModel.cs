using System;
using System.Reactive;
using System.Reactive.Linq;
using Core;
using Core.Modules.WorkspaceModule;
using Optional.Unsafe;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UIOverlay.ViewModels
{
    public class OverlayWindowViewModel : ReactiveObject
    {
        public OverlayWindowViewModel(Swim swim)
        {
            var workSpaceModule = swim
                .GetModule<WorkSpaceManager>()
                .ValueOrFailure();

            workSpaceModule
                .WhenWorkSpaceChanged
                .ToPropertyEx(this, x => x.CurrentWorkSpaceConfig);

            Observable
                .Merge(
                    workSpaceModule.WhenWorkSpaceChanged.Select(_ => Unit.Default),
                    workSpaceModule.WhenWindowChanged.Select(_ => Unit.Default))
                .Select(_ => Observable
                    .Return(true)
                    .Concat(Observable.Return(false).Delay(TimeSpan.FromSeconds(2))))
                .Switch()
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, x => x.IsVisible);
        }

        [ObservableAsProperty] public WorkSpace CurrentWorkSpaceConfig { get; }
        [ObservableAsProperty] public bool IsVisible { get; }
    }
}