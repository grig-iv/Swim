using System;
using System.Collections.Generic;
using Optional;

namespace Domain;

public interface IDesktopService : IDisposable
{
    IEnumerable<IWindow> GetWindows();
    Option<IWindow> GetForegroundWindowOrNone();
}