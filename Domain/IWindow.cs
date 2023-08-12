using System;
using System.Reactive;
using Optional;

namespace Domain;

public interface IWindow
{
    IObservable<Unit> WhenDestroyed { get; }
        
    string ProcessName { get; }
    bool IsDestroyed { get; }

    string GetTitle();
    string GetClassName();
    Option<IWindow> GetOwner();
    WindowState GetState();

    void SetForeground();
        
    bool IsVisible();

    void Minimize();
    void Maximize();
    void Normalize();
    void Restore();
        
    void Close();
}