using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Optional;
using Optional.Linq;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace Domain;

public class Window : IWindow
{
    private readonly Subject<Unit> _whenDestroyed;
    private readonly Lazy<Process> _lazyProcess;

    public Window(HWND handle)
    {
        _whenDestroyed = new Subject<Unit>();
        Handle = handle;

        _lazyProcess = new Lazy<Process>(() =>
        {
            GetWindowThreadProcessId(handle, out var processId);
            return Process.GetProcessById((int) processId);
        });
    }

    public IObservable<Unit> WhenDestroyed => _whenDestroyed.AsObservable();

    public string ProcessName => _lazyProcess.Value.ProcessName;
    public bool IsDestroyed { get; private set; }
    internal HWND Handle { get; }

    public string GetTitle()
    {
        var titleLength = GetWindowTextLength(Handle);
        var titleSb = new StringBuilder(titleLength + 1);
        GetWindowText(Handle, titleSb, titleLength + 1);
        return titleSb.ToString();
    }

    public string GetClassName()
    {
        var sb = new StringBuilder(256);
        User32.GetClassName(Handle, sb, sb.Capacity);
        return sb.ToString();
    }

    public Option<IWindow> GetOwner()
    {
        return GetWindow(Handle, GetWindowCmd.GW_OWNER)
            .SomeWhen(h => !h.IsNull)
            .Select(h => (IWindow) new Window(h));
    }

    public WindowState GetState()
    {
        var placement = new WINDOWPLACEMENT();
        GetWindowPlacement(Handle, ref placement);
        
        switch (placement.showCmd)
        {
            case ShowWindowCommand.SW_SHOWMINIMIZED:
            case ShowWindowCommand.SW_MINIMIZE:
            case ShowWindowCommand.SW_SHOWMINNOACTIVE:
            case ShowWindowCommand.SW_FORCEMINIMIZE:
                return WindowState.Minimized;

            case ShowWindowCommand.SW_SHOWMAXIMIZED:
                return WindowState.Maximized;

            case ShowWindowCommand.SW_HIDE:
                return WindowState.Hidden;

            case ShowWindowCommand.SW_SHOWNORMAL:
            case ShowWindowCommand.SW_SHOWNOACTIVATE:
            case ShowWindowCommand.SW_SHOW:
            case ShowWindowCommand.SW_SHOWNA:
            case ShowWindowCommand.SW_RESTORE:
            case ShowWindowCommand.SW_SHOWDEFAULT:
            default:
                return WindowState.Normal;
        }
    }

    public WindowStylesEx GetStyleEx()
    {
        return (WindowStylesEx) GetWindowLong(Handle, WindowLongFlags.GWL_EXSTYLE);
    }

    public void SetForeground()
    {
        SetForegroundWindow(Handle);
    }

    public bool IsVisible()
    {
        return IsWindowVisible(Handle);
    }

    public void Minimize()
    {
        ShowWindow(Handle, ShowWindowCommand.SW_SHOWMINIMIZED);
    }

    public void Maximize()
    {
        ShowWindow(Handle, ShowWindowCommand.SW_SHOWMAXIMIZED);
    }

    public void Normalize()
    {
        ShowWindow(Handle, ShowWindowCommand.SW_SHOWNORMAL);
    }

    public void Restore()
    {
        ShowWindow(Handle, ShowWindowCommand.SW_RESTORE);
    }

    public void Close()
    {
        CloseWindow(Handle);
    }

    public override bool Equals(object obj)
    {
        if (obj is Window other)
        {
            return other.Handle.Equals(this.Handle);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Handle.GetHashCode();
    }

    internal void OnDestroy()
    {
        IsDestroyed = true;

        _whenDestroyed.OnNext(Unit.Default);
        _whenDestroyed.OnCompleted();
        _whenDestroyed.Dispose();
    }
}