using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Vanara.PInvoke;
using static Vanara.PInvoke.User32;

namespace Domain.WinHook
{
    public class WinEventObservable : IObservable<WinEvent>, IDisposable
    {
        private readonly Subject<WinEvent> _subject;
        private readonly HWINEVENTHOOK _hook;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly WinEventProc _winEventDelegate;

        public WinEventObservable()
        {
            _subject = new Subject<WinEvent>();
            _winEventDelegate = WinEventProc;

            _hook = SetWinEventHook(
                EventConstants.EVENT_OBJECT_CREATE,
                EventConstants.EVENT_OBJECT_HIDE,
                IntPtr.Zero,
                _winEventDelegate,
                0,
                0,
                WINEVENT.WINEVENT_SKIPOWNTHREAD
            );

            if (_hook.IsNull)
            {
                throw new Exception("Failed to set hook");
            }
        }

        public IDisposable Subscribe(IObserver<WinEvent> observer)
        {
            return _subject.Subscribe(observer);
        }

        private void WinEventProc(
            HWINEVENTHOOK hWinEventHook,
            uint eventType,
            HWND hwnd,
            int idObject,
            int idChild,
            uint dwEventThread,
            uint dwmsEventTime)
        {
            try
            {
                if (!IsTopLevelWindow(idObject, idChild))
                    return;

                _subject.OnNext(new WinEvent
                {
                    Type = (WinEventType) eventType,
                    Handle = hwnd
                });
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        private bool IsTopLevelWindow(int idObject, int idChild)
        {
            return idObject == ObjectIdentifiers.OBJID_WINDOW && idChild == 0;
        }

        public void Dispose()
        {
            UnhookWinEvent(_hook);
            _subject?.Dispose();
        }
    }
}