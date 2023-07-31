using System;
using System.Collections.Generic;
using Optional;
using Optional.Linq;
using Vanara.PInvoke;

namespace Domain
{
    public class DesktopService : IDesktopService
    {
        public IEnumerable<Window> GetWindows()
        {
            var result = new List<Window>();

            User32.EnumWindows((handle, _) =>
            {
                var window = new Window(handle);
                if (!window.IsVisible() || window.GetOwner().HasValue)
                    return true;

                var styleEx = window.GetStyleEx();
                var isToolWindow = styleEx.HasFlag(User32.WindowStylesEx.WS_EX_TOOLWINDOW);
                if (isToolWindow)
                    return true;

                result.Add(new Window(handle));
                return true;
            }, IntPtr.Zero);

            return result;
        }

        public Option<Window> GetForegroundWindow()
        {
            return User32
                .GetForegroundWindow()
                .SomeWhen(hwnd => !hwnd.IsNull)
                .Select(hwnd => new Window(hwnd));
        }
    }
}