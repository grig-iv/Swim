using System;
using Domain;
using Vanara.PInvoke;

namespace CliUtils
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            User32.EnumWindows((hwnd, _) =>
            {
                var window = new Window(hwnd);
                if (!window.IsVisible() || window.GetOwner().HasValue)
                    return true;

                var styleEx = window.GetStyleEx();
                var isToolWindow = styleEx.HasFlag(User32.WindowStylesEx.WS_EX_TOOLWINDOW);
                if (isToolWindow)
                    return true;
                
                Console.WriteLine("{0} | {1} | {2}", window.GetTitle(), window.GetClassName(), window.ProcessName);

                return true;
            }, IntPtr.Zero);
        }
    }
}