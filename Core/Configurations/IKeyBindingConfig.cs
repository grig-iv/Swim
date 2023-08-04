using System;
using System.Collections.Generic;

namespace Core.Configurations
{
    public interface IKeyBindingConfig<TCommand> where TCommand : Enum
    {
        IEnumerable<KeyBinding<TCommand>> Bindings { get; set; }
    }
}