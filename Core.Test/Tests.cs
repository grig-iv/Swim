using System.Collections.Generic;
using Core.Configurations;
using Xunit;

namespace Core.Test
{
    public class KeybindnigsConfig
    {
        public IEnumerable<KeyBinding> Bindings { get; set; }
    }

    public class KeyBinding
    {
        public string Mods { get; set; }
        public string Key { get; set; }
        public Command Command { get; set; }
    }

    public class MyCommand : Command
    {
        public int Arg { get; set; }
    }

    public class Command
    {
    }

    public class Tests
    {
        [Fact]
        public void Test1()
        {
            var configParser = new ConfigParser();

            var config = @"
KeyBindings:
  WorkSpaces:
  -- { mods: Alt|Ctrl, Key: Y, Command:  NextWorkSpace }
  -- { mods: Alt|Ctrl, Key: U, Command:  PrevWorkSpace }
";
        }
        
        [Fact]
    }
}