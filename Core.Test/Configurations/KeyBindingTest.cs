using System.Collections.Generic;
using System.Windows.Input;
using Core.Configurations;
using FluentAssertions;
using Xunit;

namespace Core.Test.Configurations
{
    public class KeyBindingTest
    {
        private readonly ConfigParser _parser;

        public KeyBindingTest()
        {
            _parser = new ConfigParser();
            _parser.RegisterConfig("KeyBindingsWrapper", typeof(KeyBindingsWrapper));
        }

        [Fact]
        public void KeyBindingsConfig_ShouldBeDeserialized()
        {
            const string config = @"
KeyBindingsWrapper:
  Bindings:
    - { Mods: Alt, Key: A, Command: CommandA }
    - { Mods: Alt|Control, Key: B, Command: CommandB, Args: 'args' }
";

            var expectedParsedConfig = new
            {
                KeyBindingsWrapper = new KeyBindingsWrapper
                {
                    Bindings = new[]
                    {
                        new KeyBinding<Command>
                        {
                            Mods = ModifierKeys.Alt,
                            Key = Key.A,
                            Command = Command.CommandA,
                        },
                        new KeyBinding<Command>
                        {
                            Mods = ModifierKeys.Alt | ModifierKeys.Control,
                            Key = Key.B,
                            Command = Command.CommandB,
                            Args = "args"
                        }
                    }
                }
            };

            _parser
                .Parse<KeyBindingsWrapper>(config)
                .Should()
                .BeEquivalentTo(expectedParsedConfig.KeyBindingsWrapper);
        }

        public class KeyBindingsWrapper
        {
            public IEnumerable<KeyBinding<Command>> Bindings { get; set; }
        }

        public enum Command
        {
            CommandA,
            CommandB,
        }
    }
}