using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using Domain.WinHook;
using Optional;
using Optional.Collections;
using Optional.Linq;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace Domain
{
    public class DesktopService : IDesktopService
    {
        private readonly Subject<IWindow> _whenWindowCreated;
        private readonly Subject<IWindow> _whenForegroundWindowChanged;
        private readonly WinEventObservable _winEventObservable;
        private readonly Dictionary<HWND, Window> _windows;
        private readonly object _lock;

        public DesktopService()
        {
            _lock = new object();
            _whenWindowCreated = new Subject<IWindow>();
            _whenForegroundWindowChanged = new Subject<IWindow>();
            _windows = new Dictionary<HWND, Window>();

            _winEventObservable = new WinEventObservable();
            _winEventObservable.Subscribe(winEvent => HandleWinEvent(winEvent.Type, winEvent.Handle));

            InitWindows();
        }

        public IEnumerable<IWindow> GetWindows()
        {
            lock (_lock)
            {
                return _windows.Values.Where(IsTopLevelDesktopWindow);
            }
        }

        public Option<IWindow> GetForegroundWindow()
        {
            return User32.GetForegroundWindow()
                .SomeWhen(hwnd => !hwnd.IsNull)
                .Select(h => new Window(h))
                .Select(w => (IWindow) w);
        }

        private void HandleWinEvent(WinEventType eventType, HWND handle)
        {
            switch (eventType)
            {
                case WinEventType.WindowsCreated:
                    WindowCreated(handle);
                    break;
                case WinEventType.WindowsDestroyed:
                    WindowDestroyed(handle);
                    break;
                case WinEventType.WindowsSetForeground:
                    break;
            }
        }

        private void WindowCreated(HWND handle)
        {
            var window = new Window(handle);

            lock (_lock)
            {
                _windows[handle] = window;
            }
        }

        private void WindowDestroyed(HWND handle)
        {
            lock (_lock)
            {
                _windows
                    .GetValueOrNone(handle)
                    .MatchSome(window =>
                    {
                        window.OnDestroy();
                        _windows.Remove(handle);
                    });
            }
        }

        private bool IsTopLevelDesktopWindow(Window window)
        {
            if (!window.IsVisible() || window.GetOwner().HasValue)
                return false;

            var styleEx = window.GetStyleEx();
            var isToolWindow = styleEx.HasFlag(WindowStylesEx.WS_EX_TOOLWINDOW);
            if (isToolWindow)
                return false;

            return true;
        }

        private void InitWindows()
        {
            lock (_lock)
            {
                EnumWindows((handle, _) =>
                {
                    _windows[handle] = new Window(handle);
                    return true;
                }, IntPtr.Zero);
            }
        }

        public void Dispose()
        {
            _winEventObservable.Dispose();
            _whenWindowCreated?.Dispose();
            _whenForegroundWindowChanged?.Dispose();
        }
    }
}