using System;
using System.Collections.Generic;
using Optional;

namespace Domain
{
    public interface IDesktopService : IDisposable
    {
        IObservable<IWindow> WhenWindowCreated { get; }
        IObservable<IWindow> WhenForegroundWindowChanged { get; }

        IEnumerable<IWindow> GetWindows();
        Option<IWindow> GetForegroundWindow();
    }
}