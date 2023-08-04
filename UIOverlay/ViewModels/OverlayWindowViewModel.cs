using System;
using System.Reactive;
using System.Reactive.Linq;
using Core.Modules.WorkspaceModule;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UIOverlay.ViewModels
{
    public class OverlayWindowViewModel : ReactiveObject
    {
        public OverlayWindowViewModel(WorkSpaceManager workSpaceManager)
        {
            workSpaceManager
                .WhenWorkSpaceChanged
                .ToPropertyEx(this, x => x.CurrentWorkSpaceConfig);

            Observable
                .Merge(
                    workSpaceManager.WhenWorkSpaceChanged.Select(_ => Unit.Default),
                    workSpaceManager.WhenWindowChanged.Select(_ => Unit.Default))
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