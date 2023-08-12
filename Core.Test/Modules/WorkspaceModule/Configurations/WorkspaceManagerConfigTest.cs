using Core.Configurations;
using Core.Modules.WorkspaceModule.Configurations;
using Core.Test.Configurations;
using FluentAssertions;
using Xunit;

namespace Core.Test.Modules.WorkspaceModule.Configurations
{
    public class WorkspaceManagerConfigTest
    {
        private readonly ConfigParser _parser;

        public WorkspaceManagerConfigTest()
        {
            _parser = new ConfigParser();
            _parser.RegisterConfig("WorkspaceManager", typeof(WorkspaceManagerConfig));
        }

        [Fact]
        public void ConfigParser_ShouldParseWorkspaceManagerConfig()
        {
            const string config = @"
WorkspaceManager:
  Workspaces:
    - Name: WEB
      Windows:
        - Process: firefox
    - Name: DEV
      Windows:
        - Process: rider64
        - Process: code
";

            var expectedConfig = new WorkspaceManagerConfig
            {
                Workspaces = new[]
                {
                    new WorkspaceConfig
                    {
                        Name = "WEB",
                        Windows = new[] {new Target {Process = "firefox"}}
                    },
                    new WorkspaceConfig
                    {
                        Name = "DEV",
                        Windows = new[]
                        {
                            new Target {Process = "rider64"},
                            new Target {Process = "code"}
                        }
                    },
                },
            };

            _parser
                .Parse<WorkspaceManagerConfig>(config)
                .Should()
                .BeEquivalentTo(expectedConfig);
        }
    }
}