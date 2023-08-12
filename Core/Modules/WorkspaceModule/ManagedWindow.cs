using Domain;

namespace Core.Modules.WorkspaceModule;

public class ManagedWindow
{
    private readonly IWindow _window;

    public ManagedWindow(IWindow window)
    {
        _window = window;
    }

    public bool IsDestroyed => _window.IsDestroyed;

    public void Focus()
    {
        _window.SetForeground();
        _window.Maximize();
    }
}