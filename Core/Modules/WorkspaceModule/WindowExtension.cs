using System;
using Domain;

namespace Core.Modules.WorkspaceModule;

public static class WindowExtension
{
    public static void Focus(this IWindow window)
    {
        window.SetForeground();
        switch (window.GetState())
        {
            case WindowState.Minimized:
                window.Restore();
                break;
            case WindowState.Maximized:
                window.Maximize();
                break;
            case WindowState.Normal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}