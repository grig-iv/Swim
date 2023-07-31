using System;
using System.Collections.Generic;
using Core.Modules.WorkspaceModule;

namespace Core.Configurations
{
    public interface IModuleConfig<TCommand> where TCommand : Enum
    {
        IEnumerable<KeyBinding<TCommand>> Bindings { get; set; }
    }
}