using System;
using System.Reactive.Linq;
using Core.Modules.WorkspaceModule;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace UIOverlay.ViewModels;

public class OverlayWindowViewModel : ReactiveObject
{
    public OverlayWindowViewModel(WorkspaceManager workspaceManager)
    {
        workspaceManager
            .WhenWorkspaceChanged
            .ToPropertyEx(this, x => x.CurrentWorkspaceConfig);

        workspaceManager.WhenWorkspaceChanged
            .Select(_ => Observable
                .Return(true)
                .Concat(Observable.Return(false).Delay(TimeSpan.FromSeconds(2))))
            .Switch()
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToPropertyEx(this, x => x.IsVisible);
    }

    [ObservableAsProperty] public Workspace CurrentWorkspaceConfig { get; }
    [ObservableAsProperty] public bool IsVisible { get; }
}