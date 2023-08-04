using System;
using System.Reactive;
using Optional;
using Vanara.PInvoke;

namespace Domain
{
    public interface IWindow
    {
        IObservable<Unit> WhenDestroyed { get; }
        
        string ProcessName { get; }

        string GetTitle();
        string GetClassName();
        Option<Window> GetOwner();
        User32.WindowStylesEx GetStyleEx();

        void SetForeground();
        
        bool IsVisible();

        void Minimize();
        void Maximize();
        void Normalize();
        void Restore();
        
        void Close();
    }
}