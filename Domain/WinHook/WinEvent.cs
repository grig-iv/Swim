using Vanara.PInvoke;

namespace Domain.WinHook;

public struct WinEvent
{
    public WinEventType Type { get; set; }
    public HWND Handle { get; set; }
}