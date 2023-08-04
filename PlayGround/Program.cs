using System;
using Vanara.PInvoke;

namespace PlayGround
{
    class Program
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0x0000;
        private const uint EVENT_OBJECT_CREATE = 0x8000;
        private const uint EVENT_OBJECT_DESTROY = 0x8001;

        private static void Main(string[] args)
        {
            var hook = User32.SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_DESTROY, IntPtr.Zero, WinEventProc, 0, 0,
                WINEVENT_OUTOFCONTEXT);
            if (hook.IsNull)
            {
                Console.WriteLine("Failed to set hook");
                return;
            }

            MSG msg;
            while (User32.GetMessage(out msg, IntPtr.Zero, 0, 0) == 1)
            {
                User32.TranslateMessage(in msg);
                User32.DispatchMessage(in msg);
            }
        }

        private static void WinEventProc(User32.HWINEVENTHOOK hWinEventHook, uint eventType, HWND hwnd, int idObject,
            int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            switch (eventType)
            {
                case EVENT_OBJECT_CREATE:
                    Console.WriteLine($"Window created: {hwnd}");
                    break;
                case EVENT_OBJECT_DESTROY:
                    Console.WriteLine($"Window destroyed: {hwnd}");
                    break;
            }
        }
    }
}