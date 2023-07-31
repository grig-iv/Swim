using System.Collections.Generic;
using Optional;

namespace Domain
{
    public interface IDesktopService
    {
        IEnumerable<Window> GetWindows();
        Option<Window> GetForegroundWindow();
    }
}