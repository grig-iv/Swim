using System;
using System.Windows.Input;

namespace Core.Configurations
{
    public class KeyBinding<TCommand>
        where TCommand : Enum
    {
        public ModifierKeys Mods { get; set; }
        public Key Key { get; set; }
        public TCommand Command { get; set; }
        public string Args { get; set; }
    }
}