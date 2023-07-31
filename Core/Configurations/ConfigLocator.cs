using System;
using System.IO;
using Optional;
using Optional.Linq;

namespace Core.Configurations
{
    public class ConfigLocator : IConfigLocator
    {
        private const string ConfigFileName = "config.yaml";

        public Option<Stream> FindConfig()
        {
            return FindInXdgConfig()
                .Else(FindInAppData)
                .Select(fs => (Stream) fs);
        }

        private Option<FileStream> FindInXdgConfig()
        {
            var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var xdgConfigPath = Path.Combine(userDir, ".config/swim", ConfigFileName);

            return File
                .Exists(xdgConfigPath)
                .SomeWhen(exit => exit)
                .Select(_ => File.OpenRead(xdgConfigPath));
        }

        private Option<FileStream> FindInAppData()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appDataPath = Path.Combine(appDataDir, "swim", ConfigFileName);

            return File
                .Exists(appDataPath)
                .SomeWhen(exit => exit)
                .Select(_ => File.OpenRead(appDataPath));
        }
    }
}