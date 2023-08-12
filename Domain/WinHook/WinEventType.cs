using static Vanara.PInvoke.User32;

namespace Domain.WinHook;

public enum WinEventType : uint
{
    WindowsCreated = EventConstants.EVENT_OBJECT_CREATE,
    WindowsDestroyed = EventConstants.EVENT_OBJECT_DESTROY,
    WindowsSetForeground = EventConstants.EVENT_SYSTEM_FOREGROUND,
}