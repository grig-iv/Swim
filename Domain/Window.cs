using System;
using System.Diagnostics;
using System.Text;
using Optional;
using Optional.Linq;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace Domain
{
    public class Window  : IWindow
    {
        private readonly Lazy<Process> _lazyProcess;

        public Window(HWND handle)
        {
            Handle = handle;

            _lazyProcess = new Lazy<Process>(() =>
            {
                GetWindowThreadProcessId(handle, out var processId);
                return Process.GetProcessById((int) processId);
            });
        }

        public HWND Handle { get; }
        public string ProcessName => _lazyProcess.Value.ProcessName;

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

        public Option<Window> GetOwner()
        {
            return GetWindow(Handle, GetWindowCmd.GW_OWNER)
                .SomeWhen(h => !h.IsNull)
                .Select(h => new Window(h));
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
    }

    public interface IWindow
    {
        string ProcessName { get; }

        string GetTitle();
        string GetClassName();
        Option<Window> GetOwner();
        WindowStylesEx GetStyleEx();

        void SetForeground();
        
        bool IsVisible();

        void Minimize();
        void Maximize();
        void Normalize();
        void Restore();
        
        void Close();
    }
}