using System;
using Domain;

namespace Core.Modules.WorkspaceModule.Configurations;

public class Target
{
    public string Process { get; set; }
    public string Title { get; set; }

    public bool IsTargetByProcessName => !string.IsNullOrWhiteSpace(Process);
    public bool IsTargetByWindowTitle => !string.IsNullOrWhiteSpace(Title);

    public bool IsMatch(IWindow window)
    {
        if (IsTargetByProcessName && !IsMatchProcess(window.ProcessName))
        {
            return false;
        }

        if (IsTargetByWindowTitle && !IsMatchWindowTitle(window.GetTitle()))
        {
            return false;
        }

        return true;
    }

    private bool IsMatchProcess(string processName)
    {
        return string.Equals(Process, processName, StringComparison.InvariantCultureIgnoreCase);
    }

    private bool IsMatchWindowTitle(string title)
    {
        return string.Equals(Title, title, StringComparison.InvariantCultureIgnoreCase);
    }
}